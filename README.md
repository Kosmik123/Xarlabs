# Xarlabs Demo

## Setup instructions
The project is created using Unity 6 (6000.0.45f1). It's highly recommended to open it with the same version of the engine, but most likely any version of Unity 6 should work.

### Scenes 
There are two scenes in the project: 
1) **Example** scene contains basic arrangement of two objects (*Object A* and *Object B*). 
2) **XR** scene contains scaled down versions of objects from Example scene and a setup for XR visualisation of the scene (XR Origin and XR Simulator). There are also components which make objects get attracted to player hands by pressing grab button (G key in simulator).


### Procedural Mesh
In order to use procedurally created mesh two objects are needed:
1. **ProceduralMesh** component on a GameObject with MeshFilter. This component controls generation of the mesh and assigns it to the MeshFilter.
2. **ProceduralMeshAsset** asset. It has to be an object of type deriving from ProceduralMeshAsset class and implementing the `BuildMesh` method. It controls the structure and appearance of the generated mesh.

Upon assigning the ProceduralMeshAsset object to the ProceduralMesh component, the mesh will be assigned to the MeshFilter and automatically refreshed in case of any change. Mesh will also be refreshed on game start. It can be also refreshed manually by choosing "Refresh" option from component context menu. Disabling the component disables the automatic refreshing.

Alternatively the mesh can be exported as a static *.mesh* file and applied manually to a MeshFilter (or even MeshCollider). To do that there is an "Export" context menu option in every ProceduralMeshAsset object. This way the mesh can be assigned to appropriate components without using ProceduralMesh component.

While refreshing the ProceduralMesh the warning is called, because of assigning sharedMesh property of MeshFilter in `OnValidate`. As far as I know it can't be avoided, but it doesn't break any functionality. 

#### DropProceduralMeshAsset
**DropProceduralMeshAsset** is a concrete class of ProceduralMeshAsset which creates a drop-shaped mesh with a cone on one side and a spherical surface on the other. There are several properties that specify the details of the shape:
1) Resolution - a Vector2Int value that represents longitudinal and latitudinal resolution of the mesh
2) Radius - radius of the spherical surface
3) Tip length - length distance of the cone tip from the sphere designated by the spherical surface. The distance of the cone tip from the sphere center is equal the sum of radius and tip length.
4) Direction - a vector specifying the direction of the cone part from the center. 

### Lissajous Animation
**LissajousMovement** component makes the GameObject it's added to move along three-dimensional Lissajous curve in local space. The curve can be specified by adjusting four parameters typical for parametric equation of sine curve: 
1) Amplitude (A)
2) Frequency (f)
3) Phase shift (φ)
4) Offset (C)

```X = A sin(2πft + φ) + C```, 
where **X** is the result position and **t** is current time.

The formula is applied for each axis which results in chaotic movement for different parameters.

While adjusting the parameters the preview of the curve is displayed in the scene in form of a yellow line and a moving sphere gizmo visualizing the movement of the object.
 

### Object Rotation
Rotation of object is handled by **LimitedSpeedLookAt** component. It rotates the GameObject it's added to towards the target transform with a specified angular speed. The target and angular speed can be set in inspector and also changed in runtime by setting corresponding properties.


### Color Change Based on Angle
**AngleColorChanger** component controls the color of a specified MeshRenderer material relative to the facing direction. When the object with this component is facing towards the target object the leftmost color of the gradient will be applied. When the object is facing away from the target the rightmost color of the gradient will be applied. In situations in between those two the color will be correctly evaluated.

The MeshRenderer, the gradient and the target object can be specified in the inspector.


### Mesh Vertex Animation
Organic movement of mesh vertices is achieved by using a custom shader created in Shadergraph named **Organic**.
The materials of this shader can be configured with 3 properties:
1. Color - color of the mesh. It's needed specifically for dynamic color changes of the mesh.
2. Noise strength - what distance from the default location can the vertices be moved.
3. Speed - the speed of of changes.

## Description of the implementation
### Procedural Mesh 
The Procedural Mesh Creation system is based on two classes: (1) ProceduralMesh which is a component and (2) ProceduralMeshAsset which is an abstract class deriving from ScriptableObject class. Developers can define their own procedural meshes by extending ProceduralMeshAsset class and implementing `BuildMesh` method.

ProceduralMeshAsset contains an internal event `OnChanged` which ProceduralMesh it's assigned to listens to. It is invoked in assets `OnValidate` method. This makes the scene mesh instance refresh each time the procedural mesh asset properties change.

#### DropProceduralMeshAsset
`BuildMesh` method of DropProceduralMeshAsset consists of 3 steps: 
1) Calculation of important vectors and angles which are needed to correctly generate mesh vertices and UV values.
2) Populating vertices, triangles and UVs lists. This step is separated into two parts represented by two local functions: (1) generation of cone part and (2) generation of spherical part.
3) Assigning the vertices, triangles and UVs to the mesh, and finalizing the mesh creation.


### Lissajous Animation
The Lissajous movement is implemented in very simple way. There is a public static method `CalculatePosition` which calculates the new postition based on formula parameters and time. Then the result of the method is assigned to the local position of the object. Thanks to the use of Unity Mathematics package the formula form is very simple and verbose. The same method is used to calculate the gizmo position.

Because the `CalculatePosition` method is static and public in can be easily used in Editor scripts as well. When visualizing the curve in the scene view Lissajous curve parameters are loaded from serialized properties of edited object and then the method is applied to calculate a position for different points. The curve is then drawn as a series of lines connecting these points.


### Object Rotation
The implementation of the LimitedSpeedLookAt component uses `Vector3.RotateTowards` static method to calculate a slightly rotated direction. Then assigns it to transform.forward property resulting in gradual rotation of the object. 

Of course it could be implemented manually by calculating the cross product of the two vectors (current direction and target direction), making it a rotation axis and then rotating the vector by a small angle about this axis. However using `Vector3.RotateTowards` seems to be more elegant approach (and possibly more performant).


### Color Change Based on Angle
AngleColorChanger uses another pretty straight forward approach. First the dot product is calculated between direction towards target and current forward direction resulting in value in range between 1 and -1, where value equal 1  means object is facing the target and -1 means the object is backwards to the target. This dot value is then mapped to a progress value ranging between 0 and 1 using `InverseLerp`, which can be used to evaluate the color from the gradient. Resulting color is then set to the MeshRenderers material. 

By using `material` property instead of `sharedMaterial` the material copy is instantiated and assigned to the MeshRenderer. This ensures that only the specified object will change the color and potentially other objects using the same material won't be affected by the color change.


### Mesh Vertex Animation
The Organic shader is created using Shadergraph. The perlin noise values are obtained from *Gradient Noise* node which is basically perlin noise implemented in Shadergraph. The noise is moved in circles thanks to sine and cosine functions applied to time value and *Tiling And Offset* node. Then mesh vertices are moved along corresponding normals in `Vertex` function. In `Fragment` function only the color is applied to *Base Color* output. 


## Assumptions and challenges
### Assumptions
1) In some components I added public setters and getters to enable changing them in runtime (even if it wasn't specified) and in others I didn't, according to my intuition of which values are likely to be changed in runtime in case of further project development. If needed they always can be added later.
2) Exact shape of the cone with sphere wasn't specified, so I assumed the cone surface should be tangent to the sphere surface
3) For Lissajous animation I chose the most universal formula including all parameters and all axes to calculate the movement. If any of them is not needed it can just be kept with value zero.
4) UV of the drop shape is separated into 2 halfs. The upper half is projected on the cone and the lower half on the sphere. 

### Challenges
1) Making the cone surface tangent to the sphere seemed challenging at first. However after drawing the outline of the whole shape, and marking the lengths and angles of the resulting right triangle, the solution became apparent.
2) Initially I planned to skip Bonus AR/VR Integration, because of lack of VR headset. But thanks to XR Simulator provided with Unity XR packages testing XR setup became possible. However the project might have bugs that can't be detected with just simulator, so testing with a real headset should be still conducted.  

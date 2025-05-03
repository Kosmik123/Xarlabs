# Xarlabs Demo

## Setup instructions

### Scenes 
There are two scenes in the project: 
1) **Example** scene which contains basic arrangement of two objects (*Object A* and *Object B*). 

### Procedural Mesh
In order to use procedurally created mesh two objects are needed:
1. **ProceduralMesh** component on a GameObject with MeshFilter. This component controls generation of the mesh and assigns it to the MeshFilter.
2. **ProceduralMeshAsset** asset. It has to be an object of type deriving from ProceduralMeshAsset class and implementing the BuildMesh method. It controls the structure and appearance of the generated mesh.

Upon assigning the ProceduralMeshAsset object to the ProceduralMesh component, the mesh will be assigned to the MeshFilter and automatically refreshed in case of any change. Mesh will also be refreshed on game start. It can be also refreshed manually by choosing "Refresh" option from component context menu. Disabling the component disables the automatic refreshing.

Alternatively the mesh can be exported as a static *.mesh* file and applied manually to a MeshFilter (or even MeshCollider). To do that there is an "Export" context menu option in every ProceduralMeshAsset object. This way the mesh can be assigned to appropriate components without using ProceduralMesh component.

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


## Description of the implementation
### Procedural Mesh
The Procedural Mesh Creation system is based on two classes: (1) ProceduralMesh which is a component and (2) ProceduralMeshAsset which is an abstract class deriving from ScriptableObject class. Developers can define their own procedural meshes by extending ProceduralMeshAsset class and implementing BuildMesh method.

ProceduralMeshAsset contains an internal event **OnChanged** which ProceduralMesh it's assigned to listens to. It is invoked in assets OnValidate method. This makes the scene mesh instance refresh each time the procedural mesh asset properties change.

#### DropProceduralMeshAsset
BuildMesh method of DropProceduralMeshAsset consists of 3 steps: 
1) Calculation of important vectors and angles which are needed to correctly generate mesh vertices and UV values.
2) Populating vertices, triangles and UVs lists. This step is separated into two parts represented by two local functions: (1) generation of cone part and (2) generation of spherical part.
3) Assigning the vertices, triangles and UVs to the mesh, and finalizing the mesh creation.


### Lissajous Animation
The Lissajous movement is implemented in very simple way. There is a public static method CalculatePosition which calculates the new postition based on formula parameters and time. Then the result of the method is assigned to the local position of the object. Thanks to the use of Unity Mathematics package the formula form is very simple and verbose. The same method is used to calculate the gizmo position.

Because the CalculatePosition method is static and public in can be easily used in Editor scripts as well. When visualizing the curve in the scene view Lissajous curve parameters are loaded from serialized properties of edited object and then the method is applied to calculate a position for different points. The curve is then drawn as a series of lines connecting these points.


### Object Rotation



### Color Change Based on Angle


## Assumptions and challenges




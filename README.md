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

### Lissajous Movement
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


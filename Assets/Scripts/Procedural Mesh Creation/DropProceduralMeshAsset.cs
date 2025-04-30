using System.ComponentModel;
using UnityEngine;
using UnityEngine.Pool;

namespace ProceduralMeshCreation
{
    [CreateAssetMenu(menuName = "Procedural Mesh Creation/Drop Procedural Mesh")]
    public class DropProceduralMeshAsset : ProceduralMeshAsset
    {
        [SerializeField]
        private Vector2Int resolution;

        [SerializeField, Min(0)]
        private float radius;
        [SerializeField, Min(0)]
        private float tipLength;

        [SerializeField]
        private Vector3 direction = Vector3.up;

        public override void BuildMesh(Mesh mesh)
        {
            var vertices = ListPool<Vector3>.Get();
            var triangles = ListPool<int>.Get();

            Vector3 tipDirection = direction.normalized;
            Vector3 equatorRadiusAxis = GetPerpendicularDirection(tipDirection);
            Vector3 rotationAxis = Vector3.Cross(tipDirection, equatorRadiusAxis).normalized;

            float tipDistanceFromCenter = radius + tipLength;
            float tangentAngle = Mathf.Asin(radius / tipDistanceFromCenter) * Mathf.Rad2Deg;
            float longitudeDelta = 360f / resolution.x;
            Vector3 coneBaseRadiusDirection = Quaternion.AngleAxis(-tangentAngle, rotationAxis) * equatorRadiusAxis;

            Cone();
            void Cone()
            {
                var tipEnd = tipDirection * tipDistanceFromCenter;
                vertices.Add(tipEnd);

                for (int i = 0; i < resolution.x; i++)
                {
                    float angle = longitudeDelta * i;
                    var radialVertex = Quaternion.AngleAxis(angle, tipDirection) * coneBaseRadiusDirection * radius;
                    vertices.Add(radialVertex);

                    int next = i == resolution.x - 1
                        ? 1
                        : i + 2;
                    AddTriangle(0, i + 1, next);
                }
            }

            Sphere();
            void Sphere()
            {
                float latitudeFullAngle = 90 + tangentAngle;
                float latitudeDelta = latitudeFullAngle / resolution.y;
                Vector3 pole = -tipDirection * radius;
                for (int j = 1; j < resolution.y; j++)
                {
                    float latitude = j * latitudeDelta;
                    Vector3 latitudeRadiusDirection = Quaternion.AngleAxis(latitude, rotationAxis) * coneBaseRadiusDirection;
                    for (int i = 0; i < resolution.x; i++)
                    {
                        float angle = longitudeDelta * i;
                        var radialVertex = Quaternion.AngleAxis(angle, tipDirection) * latitudeRadiusDirection * radius;
                        vertices.Add(radialVertex);

                        bool isLast = i == resolution.x - 1;
                        int right = i + 1;
                        int left = right % resolution.x + 1;
                        int top = (j - 1) * resolution.x;
                        int bottom = j * resolution.x;

                        int topLeft = top + left;
                        int topRight = top + right;
                        int bottomLeft = bottom + left;
                        int bottomRight = bottom + right;

                        AddQuad(bottomLeft, bottomRight, topLeft, topRight);
                    }
                }

                vertices.Add(pole);
                int lastIndex = vertices.Count - 1;
                int baseIndex = lastIndex - resolution.x;
                for (int i = 0; i < resolution.x; i++)
                {
                    int right  = baseIndex + i;
                    int left = (i + 1) % resolution.x + baseIndex;
                    AddTriangle(left, right, lastIndex);
                }
            }

            mesh.Clear();
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateNormals();

            ListPool<int>.Release(triangles);
            ListPool<Vector3>.Release(vertices);

            void AddTriangle(int a, int b, int c)
            {
                triangles.Add(a);
                triangles.Add(b);
                triangles.Add(c);
            }

            void AddQuad(int bottomLeft, int bottomRight, int topLeft, int topRight)
            {
                AddTriangle(bottomLeft, topLeft, bottomRight);
                AddTriangle(bottomRight, topLeft, topRight);
            }
        }

        private static Vector3 GetPerpendicularDirection(Vector3 vector)
        {
            Vector3 helperAxis = Mathf.Abs(Vector3.Dot(vector, Vector3.right)) > 0.99f
                ? Vector3.forward
                : Vector3.right;

            Vector3 perpendicular = Vector3.Cross(vector, helperAxis).normalized;
            return perpendicular;
        }

        protected override void OnValidate()
        {
            resolution.x = Mathf.Max(3, resolution.x);
            resolution.y = Mathf.Max(3, resolution.y);
            base.OnValidate();
        }
    }
}

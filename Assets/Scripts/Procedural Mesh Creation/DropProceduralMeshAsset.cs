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
            Vector3 tipDirection = direction.normalized;
            Vector3 equatorRadiusAxis = GetPerpendicularDirection(tipDirection);
            Vector3 rotationAxis = Vector3.Cross(tipDirection, equatorRadiusAxis).normalized;

            // Cone

            float tipDistanceFromCenter = radius + tipLength;
            float tangentAngle = Mathf.Asin(radius / tipDistanceFromCenter) * Mathf.Rad2Deg;

            Vector3 coneBaseRadiusDirection = Quaternion.AngleAxis(-tangentAngle, rotationAxis) * equatorRadiusAxis;

            var vertices = ListPool<Vector3>.Get();
            var triangles = ListPool<int>.Get();

            var tipEnd = tipDirection * tipDistanceFromCenter;
            vertices.Add(tipEnd);

            float angleDelta = 360f / resolution.x;
            for (int i = 0; i < resolution.x; i++)
            {
                float angle = angleDelta * i;
                var radialVertex = Quaternion.AngleAxis(angle, tipDirection) * coneBaseRadiusDirection * radius;
                vertices.Add(radialVertex);

                int next = i == resolution.x - 1
                    ? 1
                    : i + 2;
                AddTriangle(0, i + 1, next);
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

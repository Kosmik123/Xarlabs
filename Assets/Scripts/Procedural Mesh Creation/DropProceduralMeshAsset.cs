using System.Collections.Generic;
using UnityEngine;

namespace ProceduralMeshCreation
{
    [CreateAssetMenu(menuName = "Procedural Mesh Creation/Drop Procedural Mesh")]
    public class DropProceduralMeshAsset : ProceduralMeshAsset
    {
        [SerializeField]
        private Vector2Int resolution;

        [SerializeField]
        private float radius;
        [SerializeField]
        private float tipLength;

        [SerializeField]
        private Vector3 direction = Vector3.up;

        public override void BuildMesh(Mesh mesh)
        {
            Vector3 tipDirection = direction.normalized;
            Vector3 rotationAxis = Mathf.Abs(Vector3.Dot(tipDirection, Vector3.right)) > 0.99f
                ? Vector3.forward
                : Vector3.right;

            Vector3 radiusDirection = Vector3.Cross(tipDirection, rotationAxis).normalized;

            var vertices = new List<Vector3>();
            var triangles = new List<int>();
            
            var tipEnd = tipDirection * (radius + tipLength);
            vertices.Add(tipEnd);

            float angleDelta = 360f / resolution.x;
            for (int i = 0; i < resolution.x; i++)
            {
                float angle = angleDelta * i;
                var radialVertex = Quaternion.AngleAxis(angle, tipDirection) * radiusDirection * radius;
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
            
            
            
            void AddTriangle(int a, int b, int c)
            {
                triangles.Add(a);
                triangles.Add(b);
                triangles.Add(c);
            }
        }

        protected override void OnValidate()
        {
            resolution.x = Mathf.Max(3, resolution.x);
            resolution.y = Mathf.Max(3, resolution.y);
            base.OnValidate();
        }
    }
}

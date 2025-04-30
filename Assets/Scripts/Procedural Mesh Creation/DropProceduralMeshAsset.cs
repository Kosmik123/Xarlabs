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

        public override void BuildMesh(Mesh mesh)
        {
            var vertices = new List<Vector3>();
            var triangles = new List<int>();
            
            
            var tipEnd = Vector3.up * (radius + tipLength);
            vertices.Add(tipEnd);

            float angleDelta = 360f / resolution.x;
            for (int i = 0; i < resolution.x; i++)
            {
                float angle = angleDelta * i;
                var radialVertex = Quaternion.AngleAxis(angle, Vector3.up) * Vector3.forward * radius;
                vertices.Add(radialVertex);

                int next = i == resolution.x - 1 
                    ? 1 
                    : i + 2;
                AddTriangle(0, i + 1, next);
            }


            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();

            void AddTriangle(int a, int b, int c)
            {
                triangles.Add(a);
                triangles.Add(b);
                triangles.Add(c);
            }
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            resolution.x = Mathf.Max(3, resolution.x);
            resolution.y = Mathf.Max(3, resolution.y);

        }

    }
}

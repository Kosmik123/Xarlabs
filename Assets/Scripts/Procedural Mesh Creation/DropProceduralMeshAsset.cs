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

        }

        protected override void OnValidate()
        {
            base.OnValidate();

            resolution.x = Mathf.Max(3, resolution.x);
            resolution.y = Mathf.Max(3, resolution.y);

        }

    }
}

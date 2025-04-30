using UnityEngine;

namespace ProceduralMeshCreation
{
    [RequireComponent(typeof(MeshFilter))]
    public class ProceduralMesh : MonoBehaviour
    {
        private MeshFilter _meshFilter;
        public MeshFilter MeshFilter
        {
            get
            {
                if (_meshFilter == null)
                    _meshFilter = GetComponent<MeshFilter>();
                return _meshFilter;
            }
        }

        private Mesh mesh;
        public Mesh Mesh
        {
            get
            {
                if (mesh == null)
                    mesh = new Mesh();
                return mesh;
            }
        }

        [SerializeField]
        private ProceduralMeshAsset proceduralMeshAsset;
#if UNITY_EDITOR
        private ProceduralMeshAsset previousProceduralMeshAsset;
#endif

        private void OnEnable()
        {
            Refresh();
        }

        [ContextMenu("Refresh Mesh")]
        private void Refresh() => Refresh(proceduralMeshAsset);

        private void Refresh(ProceduralMeshAsset meshAsset)
        {
#if UNITY_EDITOR
            if (this == null && meshAsset)
            {
                meshAsset.OnChanged -= Refresh;
                return;
            }
#endif

            if (enabled && meshAsset)
            {
                meshAsset.BuildMesh(Mesh);
                Mesh.name = meshAsset.name + " Generated";
                MeshFilter.sharedMesh = Mesh;
            }
        }

        private void OnValidate()
        {
#if UNITY_EDITOR
            if (previousProceduralMeshAsset)
                previousProceduralMeshAsset.OnChanged -= Refresh;

            if (proceduralMeshAsset)
            {
                proceduralMeshAsset.OnChanged -= Refresh;
                proceduralMeshAsset.OnChanged += Refresh;
            }
            Refresh(proceduralMeshAsset);
            previousProceduralMeshAsset = proceduralMeshAsset;
#endif
        }
    }
}

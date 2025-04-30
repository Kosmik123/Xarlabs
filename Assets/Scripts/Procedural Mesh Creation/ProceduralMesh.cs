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
        [SerializeField]
        private ProceduralMeshAsset proceduralMeshAsset;
#if UNITY_EDITOR
        private ProceduralMeshAsset previousProceduralMeshAsset;
#endif

        private void Refresh(ProceduralMeshAsset meshAsset)
        {

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

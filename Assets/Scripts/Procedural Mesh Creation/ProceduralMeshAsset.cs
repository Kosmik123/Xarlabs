using UnityEditor;
using UnityEngine;

namespace ProceduralMeshCreation
{
    public abstract class ProceduralMeshAsset : ScriptableObject
    {
        internal event System.Action<ProceduralMeshAsset> OnChanged;

        public abstract void BuildMesh(Mesh mesh);

#if UNITY_EDITOR
        [ContextMenu("Export")]
        private void ExportMesh()
        {
            var mesh = new Mesh();
            BuildMesh(mesh);
            mesh.name = name;

            var meshAssetPath = AssetDatabase.GetAssetPath(this);
            var directoryPath = meshAssetPath[..meshAssetPath.LastIndexOf('/')];
            AssetDatabase.CreateAsset(mesh, $"{directoryPath}/{mesh.name} {System.Guid.NewGuid()}.mesh");
            AssetDatabase.SaveAssets();
        }
#endif

        protected virtual void OnValidate()
        {
            OnChanged?.Invoke(this);
        }
    }
}

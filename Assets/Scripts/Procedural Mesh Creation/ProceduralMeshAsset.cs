using UnityEditor;
using UnityEngine;

namespace ProceduralMeshCreation
{
    [CreateAssetMenu(menuName = "Procedural Mesh Creation/Procedural Mesh Asset")]
    public class ProceduralMeshAsset : ScriptableObject
    {
        internal event System.Action<ProceduralMeshAsset> OnChanged;
        

        protected virtual void OnValidate()
        {
            OnChanged?.Invoke(this);
        }
    }
}

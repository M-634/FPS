using UnityEditor;
using UnityEngine;

namespace Musashi
{
    [CustomEditor(typeof(MeshCombine))]
    public class CustomMeshCombine : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var t = target as MeshCombine;
            {
                if (GUILayout.Button("Apply"))
                {
                    t.ExcuteCombineMesh();
                }
            }
        }

        [MenuItem("Tools/MeshCombine")]
        static void CreatMeshCombine()
        {
            var obj = new GameObject("MeshCombine");
            obj.AddComponent<MeshCombine>();
        }
    }


}
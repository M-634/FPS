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
    }
}
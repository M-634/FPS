using UnityEditor;
using UnityEngine;

namespace Musashi
{
    [CustomEditor(typeof(MeshCombine))]
    public class MeshCombineCustomInspector : Editor
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
        static void CreateEmptyObject()
        {
            var obj = new GameObject("MeshCombine");
            obj.AddComponent<MeshCombine>();
        }
    }


    [CustomEditor(typeof(SkinMeshRendererToMeshRenderer))]
    public class SkinMeshRendererToMeshRendererCustomInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var t = target as SkinMeshRendererToMeshRenderer;
            if (GUILayout.Button("Apply"))
            {
                t.Excute();
            }
        }

        [MenuItem("Tools/SkinMeshRendererToMeshRenderer")]
        static void CreateEmptyObject()
        {
            var obj = new GameObject("SkinMeshRendererToMeshRenderer");
            obj.AddComponent<SkinMeshRendererToMeshRenderer>();
        }
    }

}
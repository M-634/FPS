using UnityEngine;
using UnityEditor;
namespace Musashi
{
    [InitializeOnLoad]
    public static class EditScene
    {
        static EditScene()
        {
            SceneView.duringSceneGui += OnGui;
        }

        private static void OnGui(SceneView sceneView)
        {
            Handles.BeginGUI();
            ShowButtons(sceneView.position.size);
            Handles.EndGUI();
        }


        /// <summary>
        /// ボタンの描画関数
        /// </summary>
        private static void ShowButtons(Vector2 sceneSize)
        {
            //var buttonSize = 40;

            // 画面下部、水平、中央寄せをコントロールする Rect
            var rect = new Rect(
              sceneSize.x * 5 / 6,
              sceneSize.y / 7,
              160,
              30);

            if (GUI.Button(rect, "Switch PostProcess"))
            {
                RenderSettings.fog = !RenderSettings.fog;
            }
        }
    }
}




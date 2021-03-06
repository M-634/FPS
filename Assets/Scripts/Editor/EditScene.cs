using UnityEngine;
using UnityEditor;
namespace Musashi
{
    [InitializeOnLoad, CustomEditor(typeof(PlayerCamaraControl))]
    public class EditScene : Editor
    {
        PlayerCamaraControl camaraControl = null;

        private void OnEnable()
        {
            camaraControl = (PlayerCamaraControl)target;
        }

        EditScene()
        {
            SceneView.duringSceneGui += OnGui;
        }

        private void OnGui(SceneView sceneView)
        {
            Handles.BeginGUI();
            ShowButtons(sceneView.position.size);
            Handles.EndGUI();
        }


        /// <summary>
        /// ボタンの描画関数
        /// </summary>
        private void ShowButtons(Vector2 sceneSize)
        {
            var rect = new Rect(
              sceneSize.x * 5 / 6,
              sceneSize.y / 7,
              160,
              30);

            if (GUI.Button(rect, "Switch PostProcess"))
            {
                camaraControl.EditeScene();
            }
        }
    }
}




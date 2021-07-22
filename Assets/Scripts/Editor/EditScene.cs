using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering.PostProcessing;
using Musashi.Player;

namespace Musashi
{
    /// <summary>
    /// シーン編集用のウィンドウを作成する
    /// </summary>
    public class EditScene : EditorWindow
    {
        int takeDamage;
        private PlayerHealthControl playerHealth;
        public PlayerHealthControl PlayerHealth
        {
            get
            {
                if (!playerHealth)
                {
                    playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealthControl>();
                }
                return playerHealth;
            }
        }
        private bool onPostProcess;
        public bool OnPostProcess
        {
            get => onPostProcess;
            set
            {
                if (onPostProcess != value)
                {
                    onPostProcess = value;
                    var postProcessVolume = Camera.main.GetComponent<PostProcessVolume>();
                    postProcessVolume.enabled = value;
                    RenderSettings.fog = value;
                }
            }
        }



        [MenuItem("Window/EditScene")]
        static void Open()
        {
            var window = GetWindow<EditScene>();
            window.titleContent = new GUIContent("EditScene");
        }

        private void OnGUI()
        {
            GUILayout.Label("Base Settings", EditorStyles.boldLabel);
            GUILayout.Space(1);
            OnPostProcess = EditorGUILayout.Toggle("OnPostProcess", OnPostProcess);

            GUILayout.Label("Test RunTime", EditorStyles.boldLabel);
            takeDamage = EditorGUILayout.IntSlider("Take Damage", value: takeDamage, 1, 100);
            if (GUILayout.Button("Damage Player"))
            {
                if (Application.isPlaying)
                {
                    PlayerHealth.OnDamage(takeDamage);
                }
            }
            if(GUILayout.Button("Reset HP"))
            {
                playerHealth.ResetHP();
            }
        }
    }
}




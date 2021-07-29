using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Musashi
{
    /// <summary>
    /// ゲームシステムに関わるクラスや処理をまとめるクラス
    /// テスト時含め、常にシーンに常駐している。
    /// </summary>
    public class GameManager : SingletonMonoBehaviour<GameManager>
    {
        [SerializeField] SoundManager soundManager;
        [SerializeField] SceneLoader sceneLoder;
        [SerializeField] Configure configure;
        [SerializeField] TimeManager timeManager;

        public SoundManager SoundManager => soundManager;
        public SceneLoader SceneLoder => sceneLoder;
        public Configure Configure => configure;
        public TimeManager TimeManager => timeManager;
        public bool IsGameClear { get; private set; }
        public bool CanProcessPlayerMoveInput { get; set; }
        public bool ShowConfig => configure.IsActive;
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(this.gameObject);
            //QualitySettings.vSyncCount = 0; // VSyncをOFFにする
            //Application.targetFrameRate = 60;//60フレームに固定する
        }

        // Start is called before the first frame update
        void Start()
        {
            if (sceneLoder.GetActiveSceneBuildIndex == 0)
            {
                sceneLoder.LoadScene(SceneInBuildIndex.Title);//titleへ
            }
            else
            {
                sceneLoder.UnLoadScene(SceneInBuildIndex.Init);//initシーンを破棄する
            }
            InitializeConfigure();
        }

        public void ExitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public void GameOver()
        {
            Debug.Log("GameOver");
            IsGameClear = false;
            SceneLoder.LoadScene(SceneInBuildIndex.Result, UnlockCusor);
        }

        public void GameClear()
        {
            Debug.Log("GameClear");
            IsGameClear = true;
            SceneLoder.LoadScene(SceneInBuildIndex.Result, UnlockCusor);
            GameEventManager.Instance.Excute(GameEventType.Goal);
        }

        public void LockCusor()
        {
            if (sceneLoder.GetActiveSceneBuildIndex == (int)SceneInBuildIndex.Title) return;
            // Lock cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            CanProcessPlayerMoveInput = true;
        }

        public void UnlockCusor()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            CanProcessPlayerMoveInput = false;
        }

        public void SwichConfiguUI()
        {
            if (Configure.IsActive)
            {
                Configure.Close();
            }
            else
            {
                Configure.Show();
            }
        }

        private void InitializeConfigure()
        {
            Configure.ShowAction += UnlockCusor;
            Configure.CloseAction += () =>
            {
                if (sceneLoder.GetActiveSceneBuildIndex == (int)SceneInBuildIndex.Title)
                {
                    FindObjectOfType<Title>().SetOptionButtonSelected();//修正箇所:現在は、応急処置でFind関数を使っている。設計を検討中...
                }
                else
                {
                    LockCusor();
                }
            };
            Configure.Close();
        }
    }
}
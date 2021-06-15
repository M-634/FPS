using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Musashi
{
    /// <summary>
    /// ゲームに常駐するクラスをまとめる。
    /// 初期化シーンに置いておく
    /// </summary>
    public class GameManager : SingletonMonoBehaviour<GameManager>
    {
        [SerializeField] SoundManager soundManager;
        [SerializeField] SceneLoader sceneLoder;
        [SerializeField] Configure configure;
        public SoundManager SoundManager => soundManager;
        public SceneLoader SceneLoder => sceneLoder;
        public Configure Configure => configure;
        public bool HaveShowConfigure => configure.gameObject.activeSelf;
        public bool IsGameClear { get; private set; }

        public bool CanProcessInput { get; private set; }

        public int DefeatNumberOfEnemySpwaner { get; set; }//記録
        public int SumOfEnemySpwaner { get; set; } //記録

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(this.gameObject);
            QualitySettings.vSyncCount = 0; // VSyncをOFFにする
            Application.targetFrameRate = 60;//60フレームに固定する
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
            configure.gameObject.SetActive(false);//設定画面を隠す
        }

        public void ExitGame()
        {
            Debug.Log("a");
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
            GameEventManager.Instance.Excute(GameEventType.EndGame);
        }

        public void GameClear()
        {
            Debug.Log("GameClear");
            IsGameClear = true;
            SceneLoder.LoadScene(SceneInBuildIndex.Result, UnlockCusor);
            GameEventManager.Instance.Excute(GameEventType.EndGame);
        }

        //other setting Method
        public void LockCusor()
        {
            if (sceneLoder.GetActiveSceneBuildIndex == (int)SceneInBuildIndex.Title) return;
            // Lock cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            CanProcessInput = true;
        }

        public void UnlockCusor()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            CanProcessInput = false;
        }

        public void SwichConfiguUI()
        {
            if (configure.gameObject.activeSelf)
            {
                CloseConfigure();
            }
            else
            {
                ShowConfigure();
            }
        }

        public void ShowConfigure()
        {
            UnlockCusor();
            configure.gameObject.SetActive(true);
        }

        public void CloseConfigure()
        {
            if (sceneLoder.GetActiveSceneBuildIndex != (int)SceneInBuildIndex.Title)
            {
                LockCusor();
            }
            configure.gameObject.SetActive(false);
        }
    }
}
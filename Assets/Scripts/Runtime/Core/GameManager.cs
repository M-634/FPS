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
        public bool IsGameClear { get; private set; }
        public bool CanProcessInput { get; private set; }

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
            if (sceneLoder.GetActiveSceneBuildIndex == (int)SceneInBuildIndex.Title)
            {
                FindObjectOfType<Title>().SetOptionButtonSelected();//修正箇所:現在は、応急処置でFind関数を使っている。設計を検討中...
            }
            else
            {
                LockCusor();
            }
            configure.gameObject.SetActive(false);
        }
    }
}
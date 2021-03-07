using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ゲームに常駐するクラスをまとめる。
/// 初期化シーンに置いておく
/// </summary>
public class GameManager : MonoBehaviour
{
    [SerializeField] SceneLoder sceneLoder;
    [SerializeField] Configure configure;
    public SceneLoder SceneLoder { get => sceneLoder; }
    public bool IsGameClear { get; private set; }

    //singlton
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (!instance)
            {
                instance = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
                if (!instance)
                {
                    Debug.LogWarning("GameManagerが存在しません");
                }
            }
            return instance;
        }
    }


    private void Awake()
    {
        if (this != Instance) Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);
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

    public void LockCusor()
    {
        if (sceneLoder.GetActiveSceneBuildIndex == (int)SceneInBuildIndex.Title) return;
        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void UnlockCusor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
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
    }

    public void ShowConfigure()
    {
        UnlockCusor();
        configure.gameObject.SetActive(true);
    }

    public void CloseConfigure()
    {
        if (sceneLoder.GetActiveSceneBuildIndex == (int)SceneInBuildIndex.MainGame)
            LockCusor();

        configure.gameObject.SetActive(false);
    }
}

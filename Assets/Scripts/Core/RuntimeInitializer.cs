using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// test時に初期化シーンをどのシーンからでも読み込む
/// </summary>
public class RuntimeInitializer 
{

#if UNITY_EDITOR
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void OnBeforeSceneLoadRuntimeMethod() 
    {
        if (SceneManager.GetActiveScene().buildIndex == (int)SceneInBuildIndex.Init) return;
        SceneManager.LoadScene((int)SceneInBuildIndex.Init, LoadSceneMode.Additive);
    }
#endif
}

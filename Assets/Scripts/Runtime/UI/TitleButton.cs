using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace Musashi
{
    /// <summary>
    /// ボタンを押したらシーンをロードするか、設定画面を開くか閉じるボタン。
    /// </summary>
    public sealed class TitleButton : BaseButton
    {
        [SerializeField] SceneInBuildIndex buildIndex;
  
        private void Start()
        {
            button.onClick.AddListener(() => GameManager.Instance.SceneLoder.LoadScene(buildIndex));
        }
    }
}

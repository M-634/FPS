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
        [SerializeField] bool isOptionsButton;

        private void Start()
        {
            //OnClickButtonEvents.AddListener(() => GameManager.Instance.SceneLoder.LoadScene(buildIndex));
            button.onClick.AddListener(() => GameManager.Instance.SceneLoder.LoadScene(buildIndex));
        }

        /// <summary>
        /// オプション画面を閉じたら、オプションボタンを選択状態にする
        /// </summary>
        public void SetSelected()
        {
            if (!isOptionsButton) return;
            button.Select();
        }
    }
}

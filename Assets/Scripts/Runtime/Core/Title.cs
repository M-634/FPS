using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Musashi
{
    /// <summary>
    /// タイトルシーンを制御するクラス
    /// </summary>
    public class Title : MonoBehaviour
    {
        [SerializeField] Button initButton;
        [SerializeField] Button optionButton;

        private void Start()
        {
            GameManager.Instance.UnlockCusor();
            if (initButton)
            {
                initButton.Select();
            }
        }

        /// <summary>
        /// オプション画面を閉じたら、オプションボタンを選択状態にする
        /// </summary>
        public void SetOptionButtonSelected()
        {
            if (optionButton)
            {
                optionButton.Select();
            }
        }
    }
}

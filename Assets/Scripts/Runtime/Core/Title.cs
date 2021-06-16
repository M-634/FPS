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

        private void Start()
        {
            if (initButton)
            {
                initButton.Select();
            }
        }
    }
}

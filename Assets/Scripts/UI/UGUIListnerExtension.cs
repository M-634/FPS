using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;

namespace Musashi
{
    /// <summary>
    /// UGUIの拡張クラス
    /// </summary>
    public static class UGUIListnerExtension
    {
        public static void ShowUIWithCanvasGroup(this CanvasGroup canvasGroup)
        {
            canvasGroup.interactable = true;
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }

        public static void HideUIWithCanvasGroup(this CanvasGroup canvasGroup)
        {
            canvasGroup.interactable = false;
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
        }
    }
}

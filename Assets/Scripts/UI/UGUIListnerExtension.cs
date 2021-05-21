using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;

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

        public static void FadeImage(this Image image, FadeType type,float duration,UnityAction callback = null)
        {
            image.enabled = true;
          
            Color endValue;
            if (type == FadeType.In)
            {
                image.color = Color.black;
                endValue = Color.clear;
            }
            else
            {
                image.color = Color.clear;
                endValue = Color.black;
            }

             DOTween.To(() => image.color, (x) => image.color = x, endValue, duration)
                   .OnComplete(() =>
                   {
                       callback?.Invoke();
                       image.enabled = false;
                   });
        }
    }

    public enum FadeType
    {
        In,Out
    }
}

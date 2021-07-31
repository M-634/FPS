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
        /// <summary>
        /// 表示タイムを0:00.00形式で表示する
        /// </summary>
        /// <param name="text"></param>
        /// <param name="timer"></param>
        public static void ChangeTheTimeDisplayFormat(this TextMeshProUGUI textMeshPro, float timer)
        {
            int minutes = (int)timer / 60;
            float seconds = timer - minutes * 60;
            textMeshPro.text = string.Format("{0}:{1:00.00}", minutes, seconds);
        }

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

        public static void Fade(this Image image, FadeType type, float duration,bool endImageEnabled = false, UnityAction callback = null)
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
                      image.enabled = endImageEnabled;
                      callback?.Invoke();
                  });
        }

        /// <summary>
        /// image colorの不透明度を設定する関数
        /// </summary>
        /// <param name="image"></param>
        /// <param name="alpha">0f:透明 1f:不透明</param>
        public static void SetOpacityImageColor(this Image image,float alpha)
        {
            var c = image.color;
            image.color = new Color(c.r, c.g, c.b, alpha);
        }


        /// <summary>
        /// スライダーの値が変化した時のイベントを登録する関数。
        /// </summary>
        public static void SetSliderValueChangedEvent(this Slider slider, UnityAction<float> sliderCallBack)
        {
            if (!slider)
            {
                Debug.LogError("Sliderがアタッチされていない");
                return;
            }

            if (slider.onValueChanged != null)
            {
                slider.onValueChanged.RemoveAllListeners();
            }

            slider.onValueChanged.AddListener(sliderCallBack);
        }

        /// <summary>
        /// スライダーの初期値を設定する関数。値は、０～１に正規化される
        /// </summary>
        public static void SetInitializeSliderValue(this Slider slider, float initValue = 0.5f, float minValue = 0f, float maxValue = 1f)
        {
            if (!slider)
            {
                Debug.LogError("Sliderがアタッチされていない");
                return;
            }

            slider.minValue = minValue / maxValue;
            slider.maxValue = 1f;
            slider.value = initValue / maxValue;
        }

   
        /// <summary>
        /// トグルが切り替わった時に呼ばれるイベントを登録する関数
        /// </summary>
        public static void SetToggleValueChangedEvent(this Toggle toggle,UnityAction<bool> toggleCallBack)
        {
            if (!toggle)
            {
                Debug.LogError("Toggleがアタッチされていない");
                return;
            } 

            if(toggle.onValueChanged != null)
            {
                toggle.onValueChanged.RemoveAllListeners();
            }
            toggle.onValueChanged.AddListener(toggleCallBack);
        }
    }

    public enum FadeType
    {
        In, Out
    }
}

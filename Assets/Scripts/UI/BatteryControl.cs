using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;

namespace Musashi
{
    public class BatteryControl : MonoBehaviour
    {
        [SerializeField] float drainTime = 15.0f;
        [SerializeField] float duration = 0.01f;
        [SerializeField] GameObject nightVisionOverlay;
        [SerializeField] UnityEvent unityEvent;
        float currentBatteryPower = 1.0f;
        Image fillImage;
        bool powerOff;
        bool isFlashing;
        private void Start()
        {
            fillImage = GetComponent<Image>();
            fillImage.fillAmount = 1.0f;
        }

        private void OnEnable()
        {
            if (powerOff)
            {
                //unityEvent?.Invoke();
                //GameEventManeger.Instance.Excute(GameEventType.ChangePostProcess);
                InteractiveMessage.WarningMessage(InteractiveMessage.NoBatteryText);
            }
        }

        private void Update()
        {
            if (powerOff) return;

            fillImage.fillAmount -= 1.0f / drainTime * Time.deltaTime;
            currentBatteryPower = fillImage.fillAmount;
            if (currentBatteryPower < 0.3f && ! isFlashing)
            {
                isFlashing = true; 
                fillImage.DOColor(Color.clear, duration).
                    SetEase(Ease.Linear).
                    SetLoops(-1,LoopType.Yoyo);
            }

            if (currentBatteryPower < 0.1f)
            {
                powerOff = true;
                fillImage.DOKill();
                fillImage.color = Color.white;
                isFlashing = false;
                //unityEvent?.Invoke();
                //GameEventManeger.Instance.Excute(GameEventType.ChangePostProcess);
            }
        }

        public void ChargeBattery()
        {
            powerOff = false;
            fillImage.fillAmount = 1.0f;
            currentBatteryPower = 1.0f;
        }

    }
}

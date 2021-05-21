using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Musashi
{
    /// <summary>
    ///ゲーム開始時に00;00 形式でタイマーを表示する
    /// </summary>
    public class Timer : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI timerText;
        private IEnumerator currutine;
        bool onTime;

        private void StartTimer()
        {
            currutine = TimerCorutine();
            StartCoroutine(currutine);
        }

        IEnumerator TimerCorutine()
        {
            float timer = 0;
            onTime = true;

            while (onTime)
            {
                timer += Time.deltaTime;
                int minutes =(int)timer / 60;
                float seconds = timer - minutes * 60;
                timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
                yield return null;
            }
        }

        private void OnEnable()
        {
            if (!timerText) return;
            GameEventManager.Instance.Subscribe(GameEventType.StartGame,StartTimer);
            GameEventManager.Instance.Subscribe(GameEventType.EndGame, () => onTime = false);
        }

        private void OnDisable()
        {
            if (!timerText) return;
            GameEventManager.Instance.UnSubscribe(GameEventType.StartGame,StartTimer);
            GameEventManager.Instance.UnSubscribe(GameEventType.EndGame, () => onTime = false);
            currutine = null;
        }
    }
}

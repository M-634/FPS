using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Musashi
{
    /// <summary>
    ///ゲーム開始時に0:00.00 形式でタイマーを表示する
    /// </summary>
    public class Timer : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI timerText;
        [SerializeField] TextMeshProUGUI bestTime;
        [SerializeField] GameObject timerPanel;


        private IEnumerator currutine;
        bool onTime;
        float lastTime;

        private void Start()
        {
            timerPanel.SetActive(false);
        }

        private void StartTimer()
        {
            timerPanel.SetActive(true); 
            currutine = TimerCorutine();
            StartCoroutine(currutine);
        }

        private void EndTimer()
        {
            onTime = false;
            RecordResult.Instance.SetRecordTime(lastTime);
            timerPanel.SetActive(false, 2);
        }

        IEnumerator TimerCorutine()
        {
            float timer = 0;
            onTime = true;

            while (onTime)
            {
                timer += Time.deltaTime;
                timerText.ChangeTheTimeDisplayFormat(timer); 
                yield return null;
            }
            lastTime = timer;
        }

        private void OnEnable()
        {
            GameEventManager.Instance.Subscribe(GameEventType.StartGame, StartTimer);
            GameEventManager.Instance.Subscribe(GameEventType.EndGame, EndTimer);
        }

        private void OnDisable()
        {
            GameEventManager.Instance.UnSubscribe(GameEventType.StartGame, StartTimer);
            GameEventManager.Instance.UnSubscribe(GameEventType.EndGame, EndTimer);
            currutine = null;
        }
    }
}

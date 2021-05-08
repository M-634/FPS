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

        IEnumerator TimerCorutine()
        {
            float timer = 0;
            timerText.text = "";
            timerText.enabled = true;
            onTime = true;

            while (onTime)
            {
                timer += Time.deltaTime;
                int minutes =(int)timer / 60;
                float seconds = timer - minutes * 60;
                timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
                yield return null;
            }

            timerText.enabled = false;
        }

        private void OnEnable()
        {
            if (!timerText) return;
            currutine = TimerCorutine();
            GameEventManeger.Instance.Subscribe(GameEventType.StartGame, () => StartCoroutine(currutine));//error
            GameEventManeger.Instance.Subscribe(GameEventType.EndGame, () => onTime = false);
        }

        private void OnDisable()
        {
            if (!timerText) return;
            //GameEventManeger.Instance.UnSubscribe(GameEventType.StartGame, () => StartCoroutine(TimerCorutine()));
            //GameEventManeger.Instance.UnSubscribe(GameEventType.EndGame, () => onTime = false);
        }

        ////タイムリミット
        //m_timeLimit -= Time.deltaTime;
        //    //分と秒とミリ秒を設定
        //    int minutes = (int)m_timeLimit / 60;
        //float seconds = m_timeLimit - minutes * 60;
        //float mseconds = m_timeLimit * 1000 % 1000;
        //m_UISetActiveControl.TimerText.text = string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, mseconds);
        ////m_UISetActiveControl.TimerText.TimerInfo(m_timeLimit);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;

namespace Musashi
{
    /// <summary>
    /// ゲーム内の時間に関係する処理をまとめたクラス
    /// </summary>
    public class TimeManager : MonoBehaviour
    {
        [Header("Speed Run Game timer settings")]
        [SerializeField] TextMeshProUGUI timerText;
        [SerializeField] TextMeshProUGUI bestTimeText;
        [SerializeField] GameObject timerPanel;

        float defultFixedTimestep;
        private IEnumerator currutine;
        bool onTime;

        private void Start()
        {
            timerPanel.SetActive(false);
            defultFixedTimestep = Time.fixedDeltaTime;
        }

        public void StartTimer()
        {
            timerPanel.SetActive(true);
            RecordResult.Instance.DisplayBestTime(bestTimeText);
            currutine = TimerCorutine();
            StartCoroutine(currutine);
        }

        public void EndTimer()
        {
            onTime = false;
            timerPanel.DelaySetActive(false, 2f, this.GetCancellationTokenOnDestroy());
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
            RecordResult.Instance.DisplayResult(timer);
        }

        public void ChangeTimeScale(float value = 1f)
        {
            value = Mathf.Clamp01(value);
            Time.timeScale = value;
            Time.fixedDeltaTime = defultFixedTimestep * Time.timeScale;
        }


        //private void OnEnable()
        //{
        //    GameEventManager.Instance.Subscribe(GameEventType.StartGame, StartTimer);
        //    GameEventManager.Instance.Subscribe(GameEventType.Goal, EndTimer);
        //}

        //private void OnDisable()
        //{
        //    GameEventManager.Instance.UnSubscribe(GameEventType.StartGame, StartTimer);
        //    GameEventManager.Instance.UnSubscribe(GameEventType.Goal, EndTimer);
        //    currutine = null;
        //}
    }
}

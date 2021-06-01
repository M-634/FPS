using System.Collections;
using TMPro;
using UnityEngine;

namespace Musashi
{
    /// <summary>
    /// タイムと敵の撃破数を記録し、UIに表示する。 (スコアを出す)
    /// </summary>
    public class RecordResult : SingletonMonoBehaviour<RecordResult>
    {
        [SerializeField] TextMeshProUGUI timerText;
        [SerializeField] TextMeshProUGUI eliminateNumerOfEnenmyText;

        private int sumNumberOfEnemy;//敵の合計数
        private int eliminateNumerOfEnenmy;//敵の撃破数

        private float lastTime;
        private float bestTime;

        private void Start()
        {
            InitRecord();
        }

        private void InitRecord()
        {
            sumNumberOfEnemy = 0;
            eliminateNumerOfEnenmy = 0;
        }

        private void CountSumOfEnemy()
        {
            sumNumberOfEnemy++;
        }

        private void CountEliminateNumerOfEnenmy()
        {
            eliminateNumerOfEnenmy++;
        }

        /// <summary>
        /// Timerクラスから呼ばれる関数.
        /// timeを記録する
        /// </summary>
        /// <param name="timer"></param>
        public void  SetRecordTime(float timer)
        {
            lastTime = timer;
            DisplayResult();
        }

        private void DisplayResult()
        {
            eliminateNumerOfEnenmyText.text = eliminateNumerOfEnenmy.ToString() + " / " + sumNumberOfEnemy.ToString();
            timerText.ChangeTheTimeDisplayFormat(lastTime);
            InitRecord();
        }

  
        private void OnEnable()
        {
            GameEventManager.Instance.Subscribe(GameEventType.EnemySpwan, CountSumOfEnemy);
            GameEventManager.Instance.Subscribe(GameEventType.EnemyDie, CountEliminateNumerOfEnenmy);
        }

        private void OnDisable()
        {
            GameEventManager.Instance.UnSubscribe(GameEventType.EnemySpwan, CountSumOfEnemy);
            GameEventManager.Instance.UnSubscribe(GameEventType.EnemyDie, CountEliminateNumerOfEnenmy);
        }

        private void OnDestroy()
        {
            GameEventManager.Instance.UnSubscribe(GameEventType.EnemySpwan, CountSumOfEnemy);
            GameEventManager.Instance.UnSubscribe(GameEventType.EnemyDie, CountEliminateNumerOfEnenmy);
        }
    }
}
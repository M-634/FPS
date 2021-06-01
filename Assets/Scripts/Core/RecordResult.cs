using System.Collections;
using TMPro;
using UnityEngine;

namespace Musashi
{
    /// <summary>
    /// タイムと敵の撃破数を記録し、リザルトボードに表示する。 (スコアを出す)
    /// </summary>
    public class RecordResult : SingletonMonoBehaviour<RecordResult>
    {
        [SerializeField] TextMeshProUGUI timerText;
        [SerializeField] TextMeshProUGUI newRecordText;//enabledが切り替わらない

        [SerializeField] TextMeshProUGUI eliminateNumerOfEnenmyText;
        [SerializeField] TextMeshProUGUI allTargetsEliminatedText;//enabledが切り替わらない

        [SerializeField] Color bestTimeColor;
        [SerializeField] Color defultTimeColor;

        private int sumNumberOfEnemy;//敵の合計数
        private int eliminateNumerOfEnenmy;//敵の撃破数

        private float bestTime = int.MaxValue;

        private void Start()
        {
            InitRecord();
        }

        private void InitRecord()
        {
            sumNumberOfEnemy = 0;
            eliminateNumerOfEnenmy = 0;
            newRecordText.enabled = false;
            allTargetsEliminatedText.enabled = false;
        }

        private void CountSumOfEnemy()
        {
            sumNumberOfEnemy++;
        }

        private void CountEliminateNumerOfEnenmy()
        {
            eliminateNumerOfEnenmy++;
        }

        private bool CompareBestTime(float timer)
        {
            if (timer < bestTime)
            {
                bestTime = timer;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Timerクラスから呼ばれる関数.
        /// 結果をリザルトボードのUIに表示する
        /// </summary>
        /// <param name="timer"></param>
        public void DisplayResult(float timer)
        {
            //timer
            if (CompareBestTime(timer))
            {
                timerText.color = bestTimeColor;
                newRecordText.enabled = true;
                newRecordText.SetAllDirty();
            }
            else
            {
                timerText.color = defultTimeColor;
                newRecordText.enabled = false;
            }
            timerText.ChangeTheTimeDisplayFormat(timer);

            //eliminated enemy
            if (eliminateNumerOfEnenmy == sumNumberOfEnemy)
            {
                allTargetsEliminatedText.enabled = true;
            }
            else
            {
                allTargetsEliminatedText.enabled = false;
            }
            eliminateNumerOfEnenmyText.text = eliminateNumerOfEnenmy.ToString() + " / " + sumNumberOfEnemy.ToString();

            //init
            InitRecord();
        }

        /// <summary>
        /// Timerクラスから呼ばれる関数
        /// ベストタイムをタイマーUIに表示する
        /// </summary>
        /// <param name="textMeshPro"></param>
        public void DisplayBestTime(TextMeshProUGUI textMeshPro)
        {
            if (bestTime == int.MaxValue)
            {
                textMeshPro.ChangeTheTimeDisplayFormat(0f);
            }
            else
            {
                textMeshPro.ChangeTheTimeDisplayFormat(bestTime);
            }
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
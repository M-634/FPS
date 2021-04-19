using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Musashi
{
    public class EnemyCounter : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI countText;
        const string baseText = "ENEMY LEFT :";

        int totalEnemyNumber;

        // Start is called before the first frame update
        void Start()
        {
            totalEnemyNumber = GameObject.FindGameObjectsWithTag("Enemy").Length;
            countText.text = baseText + totalEnemyNumber.ToString();
        }

        /// <summary>
        /// 敵がやられたら呼ばれる
        /// </summary>
        public void UpdateEnemyCounter()
        {
            totalEnemyNumber--;
            if (totalEnemyNumber < 1)
            {
                totalEnemyNumber = 0;
                //GameClear!!
                GameManager.Instance.GameClear();
            }

            countText.text = baseText + totalEnemyNumber.ToString();
        }

        private void OnEnable()
        {
            GameEventManeger.Instance.Subscribe(GameEventType.EnemyDie, UpdateEnemyCounter);
        }

        private void OnDisable()
        {
            GameEventManeger.Instance.UnSubscribe(GameEventType.EnemyDie, UpdateEnemyCounter); 
        }
    }
}

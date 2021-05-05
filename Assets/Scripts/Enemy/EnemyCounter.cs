using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Musashi
{
    public class EnemyCounter : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI enemyDefeatText;
        [SerializeField] TextMeshProUGUI enemySpwanerText;
        const string defeatText = "ENEMY DEFEAT :";
        const string spwanerText = "Left Spwaner :";

        private int defeatEnemyOfNumber;
        private int leftSpwanerOfNumber;
        public int DefeatEnemyOfNumber {
            get { return defeatEnemyOfNumber; }
            private set {
                defeatEnemyOfNumber = value;
                if(enemyDefeatText)
                    enemyDefeatText.text = defeatText + DefeatEnemyOfNumber.ToString();
            }
        }
        public int LeftSpwanerOfNumber {
            get {return leftSpwanerOfNumber; }
            private set { 
                leftSpwanerOfNumber = value;
                if (enemySpwanerText)
                    enemySpwanerText.text = spwanerText + LeftSpwanerOfNumber.ToString();
            }
        }

        bool hasEndGame = false;

        private void Start()
        {
            DefeatEnemyOfNumber = 0;
            LeftSpwanerOfNumber = GameManager.Instance.SumOfEnemySpwaner;
        }

        private void UpdateEnemyCounter()
        {
            if (hasEndGame) return;
            DefeatEnemyOfNumber++;
        }

        private void UpdateSpwanerCounter()
        {
            if (hasEndGame) return;
            LeftSpwanerOfNumber--;
            if (LeftSpwanerOfNumber == 0)
            {
                GameManager.Instance.GameClear();
                hasEndGame = true;
            }
        }


        private void OnEnable()
        {
            GameEventManeger.Instance.Subscribe(GameEventType.EnemyDie, UpdateEnemyCounter);
            GameEventManeger.Instance.Subscribe(GameEventType.SpawnDie, UpdateSpwanerCounter);
        }

        private void OnDisable()
        {
            GameEventManeger.Instance.UnSubscribe(GameEventType.EnemyDie, UpdateEnemyCounter);
            GameEventManeger.Instance.UnSubscribe(GameEventType.SpawnDie, UpdateSpwanerCounter);
        }
    }
}

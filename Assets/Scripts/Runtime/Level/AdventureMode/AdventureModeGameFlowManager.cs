using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Musashi.Player;
using UnityEngine.UI;

namespace Musashi.Level.AdventureMode
{
    public class AdventureModeGameFlowManager : MonoBehaviour
    {
        [SerializeField] GameObject playerPrefab;
        [SerializeField, Range(1, 10)] int playerRemaingLives = 5;
        [SerializeField] Transform initSpwanPoint;
        [SerializeField] SavePointTrigger[] savePoints;
        [SerializeField] float spwanYOffset = 0.5f;
        [SerializeField] bool debugMode = false;
        [SerializeField] int debugSpwan;
        [SerializeField] GameObject[] playerRemaingLivesIcons;
        [SerializeField] UnityEventWrapper OnInitPlayerSpawnEvent;

        PlayerTranslate playerTranslate;
        PlayerHealthControl playerHealth;

        public const string SAVEPOINTTAG = "SavePoint";
        public Transform CurrentSavePoint { get; private set; }
        public int CurrentPlayerRemaingLives
        {
            get => playerRemaingLives;
            set
            {
                playerRemaingLives = value;
                if (playerRemaingLives == 0)
                {
                    GameManager.Instance.GameOver();
                }
            }
        }

        void Start()
        {
            //set save point triggers 
            var getSavePoints = GameObject.FindGameObjectsWithTag(SAVEPOINTTAG);
            savePoints = new SavePointTrigger[getSavePoints.Length];
            foreach (var point in getSavePoints)
            {
                if (point.TryGetComponent(out SavePointTrigger trigger))
                {
                    if (!savePoints[trigger.GetSpwanIndex])
                    {
                        savePoints[trigger.GetSpwanIndex] = trigger;
                        trigger.OnUpdateSavePoint += UpdateSavepoint;
                    }
                    else
                    {
                        Debug.LogWarning($"{savePoints[trigger.GetSpwanIndex].name}と{trigger.name}が同じインデックスを指定しています。");
                    }
                }
            }

            //set spwan point
            if (debugMode)
            {
                CurrentSavePoint = savePoints[debugSpwan].GetSpwan;
            }
            else
            {
                CurrentSavePoint = initSpwanPoint;
            }

            //spwan player
            SpwanPlayer();

            //set event
            if (OnInitPlayerSpawnEvent != null)
            {
                OnInitPlayerSpawnEvent.Invoke();
            }

        }

        /// <summary>
        /// プレイヤーがSavePointのColliderと接触したら呼ばれる関数
        /// </summary>
        /// <param name="index"></param>
        public void UpdateSavepoint(int index)
        {
            CurrentSavePoint = savePoints[index].GetSpwan;
            Debug.Log("save pointを更新しました。");
        }

        private void SpwanPlayer()
        {
            var instance = Instantiate(playerPrefab, CurrentSavePoint.position + new Vector3(0, spwanYOffset, 0), Quaternion.identity);
            playerTranslate = instance.GetComponentInChildren<PlayerTranslate>();
            playerHealth = instance.GetComponentInChildren<PlayerHealthControl>();
            GameEventManager.Instance.Subscribe(GameEventType.SpwanPlayer,
            () =>
            {
                CurrentPlayerRemaingLives--;//残機数を減らす
                playerRemaingLivesIcons[CurrentPlayerRemaingLives].SetActive(false);
                if (CurrentPlayerRemaingLives == 0) return;
                playerHealth.ResetHP();//体力を元に戻す
                playerTranslate.Translate(CurrentSavePoint);//リスポーン地へ移動する
            });
        }

        private void OnDestroy()
        {
            GameEventManager.Instance.UnSubscribe(GameEventType.SpwanPlayer,
            () =>
            {
                CurrentPlayerRemaingLives--;//残機数を減らす
                playerRemaingLivesIcons[CurrentPlayerRemaingLives].SetActive(false);
                if (CurrentPlayerRemaingLives == 0) return;
                playerHealth.ResetHP();//体力を元に戻す
                playerTranslate.Translate(CurrentSavePoint);//リスポーン地へ移動する
            });
        }
    }
}



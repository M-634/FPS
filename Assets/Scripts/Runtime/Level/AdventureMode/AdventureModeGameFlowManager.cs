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
        [SerializeField] Transform initSpwanPoint;
        [SerializeField] SavePointTrigger[] savePoints;
        [SerializeField] Animator updateSavePointInfo;
        [SerializeField] float spwanYOffset = 0.5f;
        [SerializeField] bool debugMode = false;
        [SerializeField] int debugSpwan;
        [SerializeField] GameObject[] playerRemaingLivesIcons;
        [SerializeField] UnityEventWrapper OnInitPlayerSpawnEvent;
        [SerializeField] PlayerDeathTrigger deathTrigger;

        int playerRemaingLives;
        PlayerTranslate playerTranslate;
        PlayerHealthControl playerHealth;

        public const string SAVEPOINTTAG = "SavePoint";
        public Transform CurrentSavePoint { get; private set; }
        public int CurrentPlayerRemaingLives => playerRemaingLives;

        void Start()
        {
            playerRemaingLives = playerRemaingLivesIcons.Length;
            Debug.Log(playerRemaingLives);
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
             InstantiatePlayer();

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
            if (updateSavePointInfo)
            {
                updateSavePointInfo.Play("TextAnimation");
            }
        }

        private void InstantiatePlayer()
        {
            var instance = Instantiate(playerPrefab, CurrentSavePoint.position + new Vector3(0, spwanYOffset, 0), Quaternion.identity);
            playerTranslate = instance.GetComponentInChildren<PlayerTranslate>();
            playerHealth = instance.GetComponentInChildren<PlayerHealthControl>();
            playerHealth.OnDeadPlayerAction += SpwanPlayer;

            if (deathTrigger)
            {
                deathTrigger.OnSpwanPlayerAction += SpwanPlayer;
            }
        }

        private void SpwanPlayer()
        {
            playerRemaingLives--;//残機数を減らす
            Debug.Log(playerRemaingLives);
            playerRemaingLivesIcons[playerRemaingLives].SetActive(false);
            if (playerRemaingLives == 0)
            {
                GameManager.Instance.GameOver();
                return;
            }
            playerHealth.ResetHP();//体力を元に戻す
            playerTranslate.Translate(CurrentSavePoint);//リスポーン地へ移動する
        }
    }
}



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
                        Debug.LogWarning($"{savePoints[trigger.GetSpwanIndex].name}��{trigger.name}�������C���f�b�N�X���w�肵�Ă��܂��B");
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
        /// �v���C���[��SavePoint��Collider�ƐڐG������Ă΂��֐�
        /// </summary>
        /// <param name="index"></param>
        public void UpdateSavepoint(int index)
        {
            CurrentSavePoint = savePoints[index].GetSpwan;
            Debug.Log("save point���X�V���܂����B");
        }

        private void SpwanPlayer()
        {
            var instance = Instantiate(playerPrefab, CurrentSavePoint.position + new Vector3(0, spwanYOffset, 0), Quaternion.identity);
            playerTranslate = instance.GetComponentInChildren<PlayerTranslate>();
            playerHealth = instance.GetComponentInChildren<PlayerHealthControl>();
            GameEventManager.Instance.Subscribe(GameEventType.SpwanPlayer,
            () =>
            {
                CurrentPlayerRemaingLives--;//�c�@�������炷
                playerRemaingLivesIcons[CurrentPlayerRemaingLives].SetActive(false);
                if (CurrentPlayerRemaingLives == 0) return;
                playerHealth.ResetHP();//�̗͂����ɖ߂�
                playerTranslate.Translate(CurrentSavePoint);//���X�|�[���n�ֈړ�����
            });
        }

        private void OnDestroy()
        {
            GameEventManager.Instance.UnSubscribe(GameEventType.SpwanPlayer,
            () =>
            {
                CurrentPlayerRemaingLives--;//�c�@�������炷
                playerRemaingLivesIcons[CurrentPlayerRemaingLives].SetActive(false);
                if (CurrentPlayerRemaingLives == 0) return;
                playerHealth.ResetHP();//�̗͂����ɖ߂�
                playerTranslate.Translate(CurrentSavePoint);//���X�|�[���n�ֈړ�����
            });
        }
    }
}



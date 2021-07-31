using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Musashi.Player;
using Cysharp.Threading.Tasks;
using System;

namespace Musashi.Level.AdventureMode
{
    /// <summary>
    /// �A�h�x���`���[���[�h����p�N���X
    /// �v���C���[�̃��X�|�[���n�_�Ǝc�@�����Ǘ�����B
    /// </summary>
    public class AdventureModeGameFlowManager : Event.CommandHandler
    {
        /// <summary>�v���C���[�v���n�u</summary>
        [SerializeField] GameObject playerPrefab;
        /// <summary>index : 0 �������n�Ƃ���</summary>
        [SerializeField] Transform[] spwanPoints;
        /// <summary>�X�|�[������Y���̍����𒲐�����</summary>
        [SerializeField] float spwanYOffset = 0.5f;
        /// <summary>���X�|�[���܂łɂ����鎞��</summary>
        [SerializeField] float respwanTimeDuration = 2f;
        /// <summary>�f�o�b�N���[�hOn/Off</summary>
        [SerializeField] bool debugMode = false;
        /// <summary>�f�o�b�N���Ƀv���C���[���X�|�[��������ꏊ(spwanPoints)��index���w�肷��</summary>
        [SerializeField, Range(0, 10)] int debugSpwanIndex;
        /// <summary>�A�h�x���`���[���[�h��p�̃v���C���[�c�@��</summary>
        [SerializeField] GameObject[] playerRemaingLivesIcons;
        /// <summary>�v���C���[���������X�|�[���������ɌĂ΂��C�x���g</summary>
        [SerializeField] UnityEventWrapper OnInitPlayerSpawnEvent;

        int playerRemaingLives;
        int currentSavePointIndex;

        PlayerTranslate playerTranslate;
        PlayerHealthControl playerHealth;

        void Start()
        {
            playerRemaingLives = playerRemaingLivesIcons.Length;

            //set savepoint
            if (debugMode)
            {
                currentSavePointIndex = debugSpwanIndex;
            }

            //spwan player
            InstantiatePlayer();

            //register each events
            reciver.Register(Event.CommandType.SpwanPlayer, SpwanPlayer);
            reciver.Register(Event.CommandType.UpdateSavePoint,UpdateSpwanPoint);

            //set event
            if (OnInitPlayerSpawnEvent != null)
            {
                OnInitPlayerSpawnEvent.Invoke();
            }

            //Debug.developerConsoleVisible = false;
        }

        private void UpdateSpwanPoint()
        {
            currentSavePointIndex++;
            if(currentSavePointIndex == spwanPoints.Length)
            {
                currentSavePointIndex = spwanPoints.Length - 1;
            }
        }

        private void InstantiatePlayer()
        {
            var player = Instantiate(playerPrefab, spwanPoints[currentSavePointIndex].position + new Vector3(0, spwanYOffset, 0), spwanPoints[currentSavePointIndex].rotation);
            playerTranslate = player.GetComponentInChildren<PlayerTranslate>();
            playerHealth = player.GetComponentInChildren<PlayerHealthControl>();
            playerHealth.OnDeadPlayerAction += SpwanPlayer;
        }

        private void SpwanPlayer()
        {
            playerRemaingLives--;//�c�@�������炷
            if (playerRemaingLives < 0)
            {
                GameManager.Instance.GameOver();
                return;
            }
            //�X�N���[�����������Â�����
            GameManager.Instance.SceneLoder.FadeScreen(FadeType.Out, 1f, true,
                async () =>
                {
                    StartSpwan();
                    await UniTask.Delay(TimeSpan.FromSeconds(respwanTimeDuration), false, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
                    EndSpwan();
                });
        }

        private void StartSpwan()
        {
            GameManager.Instance.CanProcessPlayerMoveInput = false;//�v���C���[�̓��͂��󂯕t�����I�t�ɂ���
            playerHealth.ResetHP();//�̗͂����ɖ߂�
            playerTranslate.Translate(spwanPoints[currentSavePointIndex]);//���X�|�[���n�ֈړ�����
        }

        private void EndSpwan()
        {
            //�X�N���[����FadeOut�����A�I���������͂��󂯕t����
            GameManager.Instance.SceneLoder.FadeScreen(FadeType.In, 0.1f, false,
                () =>
                {
                    GameManager.Instance.CanProcessPlayerMoveInput = true;
                    playerRemaingLivesIcons[playerRemaingLives].SetActive(false);
                });
        }
    }
}



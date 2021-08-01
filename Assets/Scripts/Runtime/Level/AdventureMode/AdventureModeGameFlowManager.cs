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
        [Header("player settings in adventure mode")]
        /// <summary>�v���C���[�v���n�u</summary>
        [SerializeField] GameObject playerPrefab;
        /// <summary> �������X�|�[���n�_</summary>
        [SerializeField] SpwanPointTrigger initSpwanPosition;
        /// <summary>�X�|�[������Y���̍����𒲐�����</summary>
        [SerializeField] float spwanYOffset = 0.5f;
        /// <summary>���X�|�[���܂łɂ����鎞��</summary>
        [SerializeField] float respwanTimeDuration = 2f;
        /// <summary>�A�h�x���`���[���[�h��p�̃v���C���[�c�@��</summary>
        [SerializeField] GameObject[] playerRemaingLivesIcons;
        /// <summary>���X�|�[���n�_���X�V�������̃C�x���g</summary>
        [SerializeField] UnityEventWrapper OnSaveSpwanPosition;

        [Header("when debug mode")]
        /// <summary>�f�o�b�N���[�hOn/Off</summary>
        [SerializeField] bool debugMode = false;
        /// <summary>�f�o�b�N���Ƀv���C���[���X�|�[��������ꏊ(spwanPoints)��id���w�肷��</summary>
        [SerializeField, Range(1, 10)] int debugSpwanID;
        /// <summary>�f�o�b�N���Ƀv���C���[���X�|�[��������Transform���A�T�C������</summary>
        [SerializeField] SpwanPointTrigger[] debugSpwanPoints;

        int playerRemaingLives;
        SpwanPointTrigger currentSpwanPointTrigger;

        PlayerTranslate playerTranslate;
        PlayerHealthControl playerHealth;

        /// <summary>
        /// �X�|�[���n�_�̃g���K�[�Ƀv���C���[���ڐG������Ă΂��֐�
        /// </summary>
        /// <param name="spwanPointTrigger"></param>
        public void UpdateSpwanPoint(SpwanPointTrigger spwanPointTrigger)
        {
            if (spwanPointTrigger.GetSpwanID == 0) return;//�������X�|�[�� 

            if (currentSpwanPointTrigger && spwanPointTrigger.GetSpwanID > currentSpwanPointTrigger.GetSpwanID)
            {
                currentSpwanPointTrigger = spwanPointTrigger;

                if (OnSaveSpwanPosition != null)
                {
                    OnSaveSpwanPosition.Invoke();
                }
            }
        }

        void Start()
        {
            playerRemaingLives = playerRemaingLivesIcons.Length;

            //set savepoint
            if (debugMode)
            {
                foreach (var p in debugSpwanPoints)
                {
                    if (p.GetSpwanID == debugSpwanID)
                    {
                        currentSpwanPointTrigger = p;
                        break;
                    }
                }
            }
            else
            {
                currentSpwanPointTrigger = initSpwanPosition;
            }

            //spwan player
            InstantiatePlayer();

            //register each events
            reciver.Register(Event.CommandType.SpwanPlayer, SpwanPlayer);
            //reciver.Register(Event.CommandType.UpdateSavePoint, () => { });
         
            //Debug.developerConsoleVisible = false;
        }

        /// <summary>
        /// �Q�[���J�n���Ƀv���C���[���C���X�^���X�����āA�K�v�ȃR���|�[�l���g���L���b�V�����Ă����֐�
        /// </summary>
        private void InstantiatePlayer()
        {
            var player = Instantiate(playerPrefab, currentSpwanPointTrigger.GetSpwanPoint.position + new Vector3(0, spwanYOffset, 0), currentSpwanPointTrigger.GetSpwanPoint.rotation);
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
            playerTranslate.Translate(currentSpwanPointTrigger.GetSpwanPoint);//���X�|�[���n�ֈړ�����
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



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Musashi.Player;
using Cysharp.Threading.Tasks;
using System;

namespace Musashi.Level.AdventureMode
{
    /// <summary>
    /// アドベンチャーモード時専用クラス
    /// プレイヤーのリスポーン地点と残機数を管理する。
    /// </summary>
    public class AdventureModeGameFlowManager : Event.CommandHandler
    {
        /// <summary>プレイヤープレハブ</summary>
        [SerializeField] GameObject playerPrefab;
        /// <summary>index : 0 を初期地とする</summary>
        [SerializeField] Transform[] spwanPoints;
        /// <summary>スポーン時のY軸の高さを調整する</summary>
        [SerializeField] float spwanYOffset = 0.5f;
        /// <summary>リスポーンまでにかかる時間</summary>
        [SerializeField] float respwanTimeDuration = 2f;
        /// <summary>デバックモードOn/Off</summary>
        [SerializeField] bool debugMode = false;
        /// <summary>デバック時にプレイヤーをスポーンさせる場所(spwanPoints)のindexを指定する</summary>
        [SerializeField, Range(0, 10)] int debugSpwanIndex;
        /// <summary>アドベンチャーモード専用のプレイヤー残機数</summary>
        [SerializeField] GameObject[] playerRemaingLivesIcons;
        /// <summary>プレイヤーが初期リスポーンした時に呼ばれるイベント</summary>
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
            playerRemaingLives--;//残機数を減らす
            if (playerRemaingLives < 0)
            {
                GameManager.Instance.GameOver();
                return;
            }
            //スクリーンをいった暗くする
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
            GameManager.Instance.CanProcessPlayerMoveInput = false;//プレイヤーの入力を受け付けをオフにする
            playerHealth.ResetHP();//体力を元に戻す
            playerTranslate.Translate(spwanPoints[currentSavePointIndex]);//リスポーン地へ移動する
        }

        private void EndSpwan()
        {
            //スクリーンをFadeOutさせ、終わったら入力を受け付ける
            GameManager.Instance.SceneLoder.FadeScreen(FadeType.In, 0.1f, false,
                () =>
                {
                    GameManager.Instance.CanProcessPlayerMoveInput = true;
                    playerRemaingLivesIcons[playerRemaingLives].SetActive(false);
                });
        }
    }
}



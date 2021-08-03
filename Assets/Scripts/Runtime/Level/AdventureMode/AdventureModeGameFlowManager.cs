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
        [Header("player settings in adventure mode")]
        /// <summary>プレイヤープレハブ</summary>
        [SerializeField] GameObject playerPrefab;
        /// <summary> 初期リスポーン地点</summary>
        [SerializeField] SpwanPointTrigger initSpwanPosition;
        /// <summary>スポーン時のY軸の高さを調整する</summary>
        [SerializeField] float spwanYOffset = 0.5f;
        /// <summary>リスポーンまでにかかる時間</summary>
        [SerializeField] float respwanTimeDuration = 2f;
        /// <summary>アドベンチャーモード専用のプレイヤー残機数</summary>
        [SerializeField] GameObject[] playerRemaingLivesIcons;
        /// <summary>リスポーン地点を更新した時のイベント</summary>
        [SerializeField] UnityEventWrapper OnSaveSpwanPosition;

        [Header("when debug mode")]
        /// <summary>デバックモードOn/Off</summary>
        [SerializeField] bool debugMode = false;
        /// <summary>デバック時にプレイヤーをスポーンさせる場所(spwanPoints)のidを指定する</summary>
        [SerializeField, Range(1, 10)] int debugSpwanID;
        /// <summary>デバック時にプレイヤーをスポーンさせるTransformをアサインする</summary>
        [SerializeField] SpwanPointTrigger[] debugSpwanPoints;

        int playerRemaingLives;
        SpwanPointTrigger currentSpwanPointTrigger;

        PlayerTranslate playerTranslate;
        PlayerHealthControl playerHealth;

        /// <summary>
        /// スポーン地点のトリガーにプレイヤーが接触したら呼ばれる関数
        /// </summary>
        /// <param name="spwanPointTrigger"></param>
        public void UpdateSpwanPoint(SpwanPointTrigger spwanPointTrigger)
        {
            if (spwanPointTrigger.GetSpwanID == 0) return;//初期リスポーン 

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
        /// ゲーム開始時にプレイヤーをインスタンス化して、必要なコンポーネントをキャッシュしておく関数
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
            playerTranslate.Translate(currentSpwanPointTrigger.GetSpwanPoint);//リスポーン地へ移動する
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



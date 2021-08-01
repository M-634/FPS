using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Musashi
{
    /// <summary>
    /// プレイヤーがボス部屋に入ってから、ボスと戦闘するまでの流れを管理するクラス
    /// </summary>
    public sealed class BossRoomTrigger : Event.OnTriggerEvent
    {
        /// <summary>カットシーンに使用するカメラ </summary>
        [SerializeField] GameObject movieCamera;
        /// <summary>カットシーンで使用するタイムライン </summary>
        [SerializeField] PlayableDirector director;
        /// <summary>実際にプレイヤーと戦うボスプレハブ</summary>
        [SerializeField] GameObject bossAIPrefab;
        /// <summary>カットシーンで使用するボスオブジェクト</summary>
        [SerializeField] GameObject bossCutSceneObj;
        /// <summary>戦闘時のプレイヤーの初期位置</summary>
        [SerializeField] Transform playerStartPos;

        Transform MainObjectOfPlayer => CollisionObject.transform.parent;
     
        protected override void Start()
        {
            base.Start();
            movieCamera.SetActive(false);
        }

        protected override void AddEnterEvent()
        {
            GameManager.Instance.SceneLoder.FadeScreen(FadeType.Out, 2f, false,
                () =>
                {
                    if (director)
                    {
                        director.Play();
                    }
                });
        }
   
        private void MovieStartAction(PlayableDirector playable)
        {
            MainObjectOfPlayer.gameObject.SetActive(false);
            bossCutSceneObj.SetActive(true);
            movieCamera.SetActive(true);
            Debug.Log("movie start");
        }

        private void EndMovieAction(PlayableDirector playable)
        {

            bossCutSceneObj.SetActive(false);
            movieCamera.SetActive(false);

            bossAIPrefab.SetActive(true);
            MainObjectOfPlayer.gameObject.SetActive(true);

            if (playerStartPos)
            {
                if(MainObjectOfPlayer.TryGetComponent(out Player.PlayerTranslate playerTranslate))
                {
                    playerTranslate.Translate(playerStartPos);
                }
            }

            Debug.Log("movie end");
        }


        private void OnEnable()
        {
            director.played += MovieStartAction;
            director.stopped += EndMovieAction;
        }

        private void OnDisable()
        {
            director.played -= MovieStartAction;
            director.stopped -= EndMovieAction;
        }
    }
}

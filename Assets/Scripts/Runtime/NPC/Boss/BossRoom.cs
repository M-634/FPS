using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Musashi
{
    /// <summary>
    /// プレイヤーがボス部屋に入ってから、ボスと戦闘するまでの流れを管理するクラス
    /// </summary>
    public class BossRoom : MonoBehaviour
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

        GameObject player;
        Vector3 spwanPosition;
        bool hasEntered;//プレイヤーがボス部屋に入ったら、フラグを立てる

        private void Start()
        {
            movieCamera.SetActive(false);
        }

        private void MovieStartAction(PlayableDirector playable)
        {
            bossCutSceneObj.SetActive(true);
            player.gameObject.SetActive(false);
            movieCamera.SetActive(true);
            Debug.Log("movie start");
        }

        private void EndMovieAction(PlayableDirector playable)
        {
            spwanPosition = bossCutSceneObj.transform.position;
            Destroy(bossCutSceneObj);

            Instantiate(bossAIPrefab, spwanPosition, Quaternion.identity);

            movieCamera.SetActive(false);

            player.gameObject.SetActive(true);
            if (playerStartPos)
            {
                player.transform.position = playerStartPos.position;
            }

            Debug.Log("movie end");
        }

        private void OnTriggerEnter(Collider other)
        {
            if (hasEntered) return;

            if (other.CompareTag("Player"))
            {
                player = other.transform.parent.gameObject;
             
                GameManager.Instance.SceneLoder.FadeScreen(FadeType.Out, 2f,false,
                    () =>
                    {
                        if (director)
                        {
                            player.SetActive(false);
                            director.Play();
                        }
                        hasEntered = true;
                    });
            }
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

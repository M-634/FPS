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
        /// <summary>実際にプレイヤーと戦うボスプレハブ</summary>
        [SerializeField] GameObject bossAIPrefab;
        [SerializeField] GameObject bossCutSceneObj;
       
        [SerializeField] PlayableDirector director;

        Vector3 spwanPosition;
        bool hasEntered;//プレイヤーがボス部屋に入ったら、フラグを立てる

        private void MovieStartAction(PlayableDirector playable)
        {
            bossCutSceneObj.SetActive(true);
            Debug.Log("movie start");
        }

        private void EndMovieAction(PlayableDirector playable)
        {
            spwanPosition = bossCutSceneObj.transform.position;
            Destroy(bossCutSceneObj);
            Instantiate(bossAIPrefab,spwanPosition,Quaternion.identity);
            Debug.Log("movie end");
        }

        private void OnTriggerEnter(Collider other)
        {
            if (hasEntered) return;

            if (other.CompareTag("Player"))
            {
                if (director)
                {
                    director.Play();
                }
                hasEntered = true;
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

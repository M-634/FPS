using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Musashi.Test
{
    public class TestEnemyTarget : MonoBehaviour, IDamageable
    {
        private void Start()
        {
            gameObject.SetActive(false);//初期時はアクティブをきる。
            GameEventManager.Instance.Subscribe(GameEventType.StartGame, Spwan);
            GameEventManager.Instance.Subscribe(GameEventType.EndGame, InitState);
        }

        private void InitState()
        {
            //敵の初期化処理を書く
        }

        private void Spwan()
        {
            gameObject.SetActive(true);
            GameEventManager.Instance.Excute(GameEventType.EnemySpwan);
        }

        public TargetType GetTargetType()
        {
            return TargetType.Defult;      
        }

        public void OnDamage(float damage)
        {
            gameObject.SetActive(false);
            GameEventManager.Instance.Excute(GameEventType.EnemyDie);
        }

        /// <summary>
        /// シーンロード時のみイベントを解除する
        /// </summary>
        private void OnDestroy()
        {
            GameEventManager.Instance.UnSubscribe(GameEventType.StartGame, Spwan);
            GameEventManager.Instance.UnSubscribe(GameEventType.EndGame, InitState);
        }
    }
}


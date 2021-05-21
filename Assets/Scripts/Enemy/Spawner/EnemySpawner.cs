using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Musashi
{
    public class EnemySpawner : BaseHealthControl, IPoolUser<EnemySpawner>
    {
        [SerializeField] Transform spwanPos;
        [SerializeField] float interval = 1f;
        [SerializeField, Range(1, 100)] int poolSize;
        [SerializeField] GameObject[] enemies;
        [SerializeField] GameObject[] dropItems;

        [SerializeField] bool debug = false;
        GameObject dropItem;

        public bool IsGenerating { get; set; } = false;
        PoolObjectManager poolObjectManager;

        protected override void Start()
        {
            base.Start();
            if (dropItems.Length > 0)
            {
                int random = Random.Range(0, dropItems.Length);
                if(dropItems[random])
                {
                    dropItem = Instantiate(dropItems[random], transform.position, Quaternion.identity);
                }
            }
            InitializePoolObject(poolSize);
        }

        public void InitializePoolObject(int poolSize = 1)
        {
            poolObjectManager = new PoolObjectManager();
            for (int i = 0; i < poolSize; i++)
            {
                SetPoolObj();
            }
        }

        public PoolObjectManager.PoolObject SetPoolObj()
        {
            var poolObj = poolObjectManager.InstantiatePoolObj();
            for (int i = 0; i < enemies.Length; i++)
            {
                if (enemies[i] == null) continue;
                var go = Instantiate(enemies[i], transform);
                poolObj.AddObj(go);
                go.SetActive(false);
            }
            return poolObj;
        }

        /// <summary>
        /// プレイヤーが一定範囲に入ったらイベントから呼ばれる関数
        /// </summary>
        public void SpwanEnemy()
        {
            IsGenerating = true;
            StartCoroutine(EnemyGeneratorCorutine());
        }

        IEnumerator EnemyGeneratorCorutine()
        {
            Debug.Log("生成スタート");
            int length = enemies.Length;

            if (length == 0) IsGenerating = false;

            while (IsGenerating)
            {
                //generate
                if (length > 0)
                {
                    int i = Random.Range(0, length);
                    if (enemies[i])
                    {
                       poolObjectManager.UsePoolObject(enemies[i], spwanPos.position, Quaternion.identity, SetPoolObj);
                    }
                }
                yield return new WaitForSeconds(interval);
            }
        }


        public override void OnDamage(float damage)
        {
            if (debug) return;

            base.OnDamage(damage);
        }


        protected override void OnDie()
        {
            IsDead = true;
            IsGenerating = false;
            GameEventManeger.Instance.Excute(GameEventType.SpawnDie);

            if (healthBarFillImage)
            {
                healthBarFillImage.transform.parent.gameObject.SetActive(false);
            }

            if (dropItem != null)
            {
                dropItem.SetActive(true);
            }

            gameObject.SetActive(false);
        }
    }
}

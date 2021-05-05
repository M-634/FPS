using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Musashi
{
    public class EnemySpawner : BaseHealthControl, IPoolUser<EnemySpawner>
    {
        [SerializeField] Transform spwanPos;
        [SerializeField] float interval = 1f;
        [SerializeField, Range(1, 10)] int poolSize;
        [SerializeField] GameObject[] enemies;
        [SerializeField] GameObject[] dropItems;
        GameObject dropItem;

        public bool IsGenerating { get; set; }
        PoolObjectManager poolObjectManager;

        protected override void Start()
        {
            base.Start();
            if (dropItems.Length > 0)
            {
                int random = Random.Range(0, dropItems.Length);
                dropItem = Instantiate(dropItems[random],transform.position,Quaternion.identity);
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
                int random = Random.Range(0, length);
                poolObjectManager.UsePoolObject(enemies[random], spwanPos.position, Quaternion.identity, SetPoolObj);
                Debug.Log("Generate");
                yield return new WaitForSeconds(interval);
            }
            Debug.Log("Stop generate");
        }

        protected override void OnDie()
        {
            IsGenerating = false;
            dropItem.SetActive(true); 
            GameEventManeger.Instance.Excute(GameEventType.SpawnDie);
            base.OnDie();
        }
    }
}

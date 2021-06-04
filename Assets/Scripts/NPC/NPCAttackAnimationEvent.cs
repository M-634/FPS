using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

namespace Musashi.NPC
{
    /// <summary>
    /// 敵の攻撃アニメーションイベントをラッパーしたクラス
    /// </summary>
    [Serializable]
    public class NPCAttackAnimationEvent : UnityEvent { }

    /// <summary>
    /// 敵の攻撃イベントを制御するベースクラス
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class BaseNPCAttackEventControl : MonoBehaviour
    {
        protected NPCAttackAnimationEvent OnAnimationEvent;

        /// <summary>
        /// アニメーションイベントから直接呼ばれる関数
        /// </summary>
        public void Excute()
        {
            if(OnAnimationEvent != null)
            {
                OnAnimationEvent.Invoke();
            }
        }
    }

    /// <summary>
    /// 敵の放つ弾を制御するクラス
    /// </summary>
    public class NPCShotAttack :BaseNPCAttackEventControl,IPoolUser<NPCShotAttack>
    {
        [SerializeField] Transform muzzle;
        [SerializeField] GameObject projectile;
        [SerializeField] GameObject muzzleFlashVFX;
        [SerializeField] Transform poolParent;

        [SerializeField,Range(1,30)] int poolSize = 5;
        PoolObjectManager poolObjectManager;

        private void Start()
        {
            OnAnimationEvent.AddListener(Shot);
            InitializePoolObject(poolSize);
        }

        private void Shot()
        {
            poolObjectManager.UsePoolObject(muzzle.position, Quaternion.identity, SetPoolObj);
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

            var b = Instantiate(projectile,poolParent);
            var mF = Instantiate(muzzleFlashVFX,poolParent);

            poolObj.AddObj(b);
            poolObj.AddObj(mF);

            poolObj.SetActiveAll(false);

            return poolObj;
        }
    }

  
    public class NPCProjectile : MonoBehaviour 
    {
        int lifeTime;
        float shotPower;
        float shotDamege;
        Rigidbody rb;

        private void Reset()
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        public void SetValues(float shotPower,float shotDamege)
        {
            this.shotPower = shotPower;
            this.shotDamege = shotDamege;
        }

        private void OnEnable()
        {
            if(rb == null)
            {
                rb = gameObject.GetComponent<Rigidbody>();
            }

            rb.velocity = transform.forward * shotPower;

            gameObject.SetActive(false, lifeTime);
        }
    }
}
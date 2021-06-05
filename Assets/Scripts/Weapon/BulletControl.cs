using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;

namespace Musashi
{
    /// <summary>
    /// 1frame前の位置から、現在の位置までにRayを飛ばして当たり判定をとる。
    /// </summary>
    public class BulletControl : MonoBehaviour
    {
        [SerializeField] int lifeTime = 1;

        float shotDamage;
        float shotPower;

        bool init = true;

        HitVFXManager hitVFXManager;
        Rigidbody rb;
        Vector3 prevPos;

        private void OnEnable()
        {
            if (rb == null)
            {
                rb = GetComponent<Rigidbody>();
            }

            prevPos = transform.position;
            rb.velocity = transform.forward * shotPower;

            if (init == false)
            {
                gameObject.SetActive(false, lifeTime);
            }
            init = false;
        }

        public void SetInfo(float shotPower, float shotDamage, HitVFXManager hitVFXManager)
        {
            this.shotDamage = shotDamage;
            this.shotPower = shotPower;
            this.hitVFXManager = hitVFXManager;
        }

        private void Update()
        {
            //hit check; 修正どころ：
            RaycastHit[] hits = Physics.RaycastAll(new Ray(prevPos, (transform.position - prevPos).normalized), (transform.position - prevPos).magnitude);

            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider.TryGetComponent(out IDamageable target))
                {
                    var type = target.GetTargetType();
                    if (type != TargetType.Defult)
                    {
                        hitVFXManager.PoolObjectManager.UsePoolObject(hitVFXManager.SelectVFX(type), hits[i].point, Quaternion.LookRotation(hits[i].normal), hitVFXManager.SetPoolObj);
                    }
                    target.OnDamage(shotDamage);
                }
                else
                {
                    hitVFXManager.PoolObjectManager.UsePoolObject(hitVFXManager.DecalVFX, hits[i].point + hits[i].normal * 0.01f, Quaternion.LookRotation(hits[i].normal * -1f), hitVFXManager.SetPoolObj);
                    hitVFXManager.AudioSource.Play(hitVFXManager.HitSFX);
                }
            }
            prevPos = transform.position;
        }
    }

    /// <summary>
    /// オブジェットプールで管理する弾丸クラス
    /// </summary>
    [System.Serializable]
    public class ObjectPoolingProjectile
    {
        [SerializeField] float power;
        [SerializeField] float damage;
        /// <summary>オブジェットのアクティブがTrue状態の時間</summary>
        [SerializeField] int lifeTime;

        /// <summary>オブジェットプール初期化時に,インスタンス化したタイミングは何もしないためのフラグ</summary>
        private bool init = true;
        public float Power => power;
        public float Damage => damage;

        public void SetActiveFalseAfterLifeTime(GameObject obj)
        {
            if (init) return;
            obj.SetActive(false, lifeTime);
            init = false;
        }
    }

    /// <summary>
    /// 弾丸を制御するクラス
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class ProjectileControl : MonoBehaviour
    {
        ObjectPoolingProjectile projectile;
        Rigidbody rb;
        Vector3 prevPos;

        /// <summary>
        /// オブジェットプールの初期化時、弾丸をインスタンス化したタイミングで
        /// この関数を呼び出す。
        /// </summary>
        public void SetProjectileInfo(ObjectPoolingProjectile projectile)
        {
            this.projectile = projectile;
        }

        /// <summary>
        ///アクティブが真になったら弾を前に飛ばす
        /// </summary>
        private void OnEnable()
        {
            if (rb == null)
            {
                rb = GetComponent<Rigidbody>();
            }

            prevPos = transform.position;
            rb.velocity = transform.forward * projectile.Power;

            projectile.SetActiveFalseAfterLifeTime(gameObject);
        }

        /// <summary>
        /// 1frame前の位置から、現在の位置までにRayを飛ばして当たり判定をとる。
        /// </summary>
        private void Update()
        {
            RaycastHit[] hits = Physics.RaycastAll(new Ray(prevPos, (transform.position - prevPos).normalized), (transform.position - prevPos).magnitude);

            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider)
                {
                    if (hits[i].collider.TryGetComponent(out IDamageable target))
                    {
                        target.OnDamage(projectile.Damage);
                        gameObject.SetActive(false);
                        return;
                        //var type = target.GetTargetType();
                        //if (type != TargetType.Defult)
                        //{
                        //    hitVFXManager.PoolObjectManager.UsePoolObject(hitVFXManager.SelectVFX(type), hits[i].point, Quaternion.LookRotation(hits[i].normal), hitVFXManager.SetPoolObj);
                        //}
                        //target.OnDamage(shotDamage);
                    }
                    gameObject.SetActive(false);
                    return;
                    //else
                    //{
                        //hitVFXManager.PoolObjectManager.UsePoolObject(hitVFXManager.DecalVFX, hits[i].point + hits[i].normal * 0.01f, Quaternion.LookRotation(hits[i].normal * -1f), hitVFXManager.SetPoolObj);
                        //hitVFXManager.AudioSource.Play(hitVFXManager.HitSFX);
                    //}
                }
            }
            prevPos = transform.position;
        }

    }
}

using UnityEngine;

namespace Musashi
{
    /// <summary>
    /// 弾丸を制御するクラス
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class ProjectileControl : MonoBehaviour
    {
        ObjectPoolingProjectileInfo projectile = default;
        Rigidbody rb;
        Vector3 prevPos;
        bool init = true;//オブジェクトプール初期化にインスタンス化するタイミングは、projectile = nullとなる為。NullReference Errorが呼ばれてしまうのを防ぐ意図がある。  

        private void Reset()
        {
            rb = GetComponent<Rigidbody>();
            rb.useGravity = false;
        }

        /// <summary>
        /// オブジェットプールの初期化時、弾丸をインスタンス化したタイミングで
        /// この関数を呼び出す。
        /// </summary>
        public void SetProjectileInfo(ObjectPoolingProjectileInfo projectile)
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

            if (init == false)
            {
                prevPos = transform.position;
                rb.velocity = projectile.Muzzle.forward * projectile.Power;
                gameObject.SetActive(false, projectile.LifeTime);
            }
            init = false;
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

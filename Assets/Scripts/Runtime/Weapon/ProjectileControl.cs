using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Musashi.Weapon
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

        /// <summary>
        /// オブジェットプールの初期化時、弾丸をインスタンス化したタイミングで
        /// この関数を呼び出す。
        /// </summary>
        public void SetProjectileInfo(ObjectPoolingProjectileInfo projectile)
        {
            this.projectile = projectile;
            rb = GetComponent<Rigidbody>();
            rb.useGravity = false;
        }

        /// <summary>
        ///アクティブが真になったら弾を前に飛ばす
        /// </summary>
        private void OnEnable()
        {
            if (projectile != null)
            {
                prevPos = transform.position;
                this.transform.forward = projectile.muzzle.forward;
                rb.velocity = projectile.muzzle.forward * projectile.power;
                gameObject.DelaySetActive(false, projectile.lifeTime, this.GetCancellationTokenOnDestroy());
            }
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
                        target.OnDamage(projectile.damage);
                    }

                    //product hit effect
                    if (HitVFXManager.Instance)
                    {
                        if (projectile.owner == ProjectileOwner.Player)
                        {
                            HitVFXManager.Instance.ProductEffectByPlayer(hits[i].collider.gameObject.layer, hits[i].point + hits[i].normal * 0.01f, hits[i].normal);
                        }
                        else
                        {
                            HitVFXManager.Instance.ProductEffectByNPC(projectile.effect, hits[i].point + hits[i].normal * 0.01f, hits[i].normal);
                        }
                    }
                    gameObject.SetActive(false);
                    return;
                }
            }
            prevPos = transform.position;
        }

    }
}

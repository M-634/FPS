using UnityEngine;

namespace Musashi
{
    public class BulletControl : MonoBehaviour
    {
        WeaponGeneralData data;
        RaycastHit hit;
        bool isHit;
        float timer;

        public void Fire(WeaponGeneralData data)
        {
            this.data = data;
            transform.position = data.muzzles[data.muzzlesIndex].position;//弾を銃口へ戻す
            HitCheck();
        }

        void HitCheck()
        {
            if (Physics.SphereCast(data.muzzles[data.muzzlesIndex].position, transform.lossyScale.x * 0.5f, data.muzzles[data.muzzlesIndex].forward, out hit))
            {
                isHit = true;
            }
        }

        private void Update()
        {
            timer += Time.deltaTime;
            transform.position += data.muzzles[data.muzzlesIndex].forward * data.shotPower * Time.deltaTime;

            if (timer > data.ammoLifeTime)
            {
                timer = 0f;
                gameObject.SetActive(false);
            }

            if (isHit)
            {
                //銃口からの移動量が、Rayでヒットした座標までの距離を超えたら当たったことにする
                //これでヒットチェックがすり抜けるのを防止できるが、弾が真っ直ぐ飛ぶことが条件である。
                //今回のプロジェクトのように、弾速が速く等速に動き、弾丸も見せたい場合に有効である
                if((transform.position - data.muzzles[data.muzzlesIndex].position).magnitude > hit.distance)
                {
                    if(hit.collider.TryGetComponent(out IDamageable damageable))
                    {
                        damageable.OnDamage(data.shotDamage);
                    }

                    timer = 0f;
                    isHit = false;
                    gameObject.SetActive(false);
                }
            }
        }
    }
}

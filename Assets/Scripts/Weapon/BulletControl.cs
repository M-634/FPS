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

        float timer;
        bool init = true;

        HitVFXManager hitVFXManager;
        Rigidbody rb;
        Vector3 prevPos;

        private void OnEnable()
        {
            if(rb == null)
                rb = GetComponent<Rigidbody>();
      
            prevPos = transform.position;
            rb.velocity = transform.forward * shotPower;

            if (init == false)
                gameObject.SetActive(false, lifeTime);
           
             init = false;
        }

        public void SetInfo(float shotPower,float shotDamage,HitVFXManager hitVFXManager)
        {
            this.shotDamage = shotDamage;
            this.shotPower = shotPower;
            this.hitVFXManager = hitVFXManager;
        }

        private void Update()
        {
            //timer += Time.deltaTime;

            //if (timer > lifeTime)
            //{
            //    timer = 0f;
            //    gameObject.SetActive(false);
            //}

            //hit check
            RaycastHit[] hits = Physics.RaycastAll(new Ray(prevPos, (transform.position - prevPos).normalized), (transform.position - prevPos).magnitude);

            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider.TryGetComponent(out IDamageable target))
                {
                    //Instantiate(bloodVFX, hits[i].point, Quaternion.LookRotation(hits[i].normal));
                    hitVFXManager.PoolObjectManager.UsePoolObject(hitVFXManager.BloodVFX, hits[i].point, Quaternion.LookRotation(hits[i].normal), hitVFXManager.SetPoolObj);
                    target.OnDamage(shotDamage);
                }
                else
                {
                    //Instantiate(decal, hits[i].point + hits[i].normal * 0.01f, Quaternion.LookRotation(hits[i].normal * -1f));
                    hitVFXManager.PoolObjectManager.UsePoolObject(hitVFXManager.DecalVFX, hits[i].point + hits[i].normal * 0.01f, Quaternion.LookRotation(hits[i].normal * -1f), hitVFXManager.SetPoolObj);
                    hitVFXManager.AudioSource.Play(hitVFXManager.HitSFX);
                }
            }
            prevPos = transform.position;
        }
    }
}

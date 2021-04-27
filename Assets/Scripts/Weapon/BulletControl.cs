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

        [SerializeField] GameObject bloodVFX;
        [SerializeField] GameObject decal;

        [SerializeField] float lifeTime = 1f;

        float shotDamage;
        float shotPower;

        float timer;

        Rigidbody rb;
        Vector3 prevPos;

        private void OnEnable()
        {
            if(rb == null)
                rb = GetComponent<Rigidbody>();
      
            prevPos = transform.position;
            rb.velocity = transform.forward * shotPower;
        }

        public void SetInfo(ref float shotPower,ref float shotDamage)
        {
            this.shotDamage = shotDamage;
            this.shotPower = shotPower;
        }

        private void Update()
        {
            timer += Time.deltaTime;

            if(timer > lifeTime)
            {
                timer = 0f;
                gameObject.SetActive(false);
            }

            //hit check
            RaycastHit[] hits = Physics.RaycastAll(new Ray(prevPos, (transform.position - prevPos).normalized), (transform.position - prevPos).magnitude);

            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider.TryGetComponent(out IDamageable target))
                {
                    Instantiate(bloodVFX, hits[i].point, Quaternion.LookRotation(hits[i].normal));
                    target.OnDamage(shotDamage);
                }
                else
                {
                    Instantiate(decal, hits[i].point + hits[i].normal * 0.01f, Quaternion.LookRotation(hits[i].normal * -1f));
                }
                gameObject.SetActive(false);
            }
            prevPos = transform.position;
        }
    }
}

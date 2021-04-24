using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace Musashi
{
    /// <summary>
    /// 1frame前の位置から、現在の位置までにRayを飛ばして当たり判定をとる
    /// </summary>
    public class BulletControl : MonoBehaviour
    {
        [SerializeField] float lifeTime = 1f;
        [SerializeField] GameObject bloodVFX;
        float shotDamage;
        float shotPower;
        Rigidbody rb;
        Vector3 prevPos;
        float timer;

        private void OnEnable()
        {
            if(!rb)
                rb = GetComponent<Rigidbody>();
            prevPos = transform.position;
            rb.velocity = transform.forward * shotPower;
        }

        private void OnDisable()
        {
            
        }


        public void SetInfo(ref float shotPower,ref float shotDamage)
        {
            this.shotDamage = shotDamage;
            this.shotPower = shotPower;
        }

        //public void AddForce(ref float shotPower,ref float shotDamage, Transform muzzle)
        //{
        //    this.shotDamage = shotDamage;
        //    rb = GetComponent<Rigidbody>();
        //    rb.velocity = muzzle.forward * shotPower;
        //    prevPos = transform.position;
        //}

        private void Update()
        {

            timer += Time.deltaTime;

            if(timer > lifeTime)
            {
                timer = 0f;
                gameObject.SetActive(false);
            }



            //RaycastHit[] hits = Physics.RaycastAll(new Ray(prevPos, (transform.position - prevPos).normalized), (transform.position - prevPos).magnitude);

            //for (int i = 0; i < hits.Length;i++)
            //{
            //    //Debug.Log(hits[i].collider.gameObject.name);
            //    if(hits[i].collider.TryGetComponent(out IDamageable target))
            //    {
            //        Instantiate(bloodVFX, hits[i].point ,Quaternion.LookRotation(hits[i].normal));
            //        target.OnDamage(shotDamage);
            //    }
            //    Destroy(gameObject);
            //}
            //prevPos = transform.position;
            //Destroy(gameObject, lifeTime);
        }
    }
}

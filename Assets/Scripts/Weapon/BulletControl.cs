using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace Musashi
{
    public class BulletControl : MonoBehaviour
    {
        [SerializeField] LayerMask targetLayer;
        [SerializeField] float rayDistance;
        [SerializeField] float lifeTime = 1f;
        float timer = 0f;
        float shotPower;
        Transform muzzle;
        public void AddForce(ref float shotPower ,Transform muzzle )
        {
            this.shotPower = shotPower;
            this.muzzle = muzzle;
        }

        public void Update()
        {
            timer += Time.deltaTime;
            transform.position += muzzle.forward * shotPower;

            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, rayDistance, targetLayer))
            {
                if(hit.collider.TryGetComponent(out IDamageable target))
                {
                    Debug.Log("当たった");
                    target.OnDamage(shotPower);
                    Destroy(gameObject)S
                }
            }

            if (timer > lifeTime) 
            {
                timer = 0f;
                //gameObject.SetActive(false);
                Destroy(gameObject);
            }
        }

        private void OnDrawGizmos()
        {
            Debug.DrawRay(transform.position, transform.forward * rayDistance , Color.white);  
        }
    }
}

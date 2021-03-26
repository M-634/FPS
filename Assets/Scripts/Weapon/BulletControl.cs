using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace Musashi
{
    public class BulletControl : MonoBehaviour
    {
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
            if (timer > lifeTime) 
            {
                timer = 0f;
                //gameObject.SetActive(false);
                Destroy(gameObject);
            }
        }
    }
}

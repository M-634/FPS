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
        public void AddForce(ref float shotPower)
        {
            this.shotPower = shotPower;
        }

        public void Update()
        {
            timer += Time.deltaTime;
            transform.position += transform.forward * shotPower;
            if (lifeTime > timer)
            {
                timer = 0f;
                gameObject.SetActive(false);
            }
        }
    }
}

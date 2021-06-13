using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Musashi
{
    /// <summary>
    /// バラバラに弾ける
    /// </summary>
    public class ZombieDeadVFX : MonoBehaviour
    {
        [SerializeField] ParticleSystem particle;
        [SerializeField, Range(0, -10)] int min;
        [SerializeField, Range(0, 10)] int max;
        [SerializeField] int lifeTime = 2;

        Rigidbody[] rigidbodies;

        // Start is called before the first frame update
        private void OnEnable()
        {
            rigidbodies = gameObject.GetComponentsInChildren<Rigidbody>();

            rigidbodies.ToList().ForEach(r =>
            {
                r.isKinematic = false;
                r.gameObject.SetActive(false, lifeTime);
                var vect = new Vector3(Random.Range(min,max),Random.Range(0,max),Random.Range(min,max));
                r.AddForce(vect, ForceMode.Impulse);
                r.AddTorque(vect, ForceMode.Impulse);
            });
            //objectpoolにまだ対応していません.
        }
    }
}

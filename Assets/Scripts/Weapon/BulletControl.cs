using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace Musashi
{
    public class BulletControl : MonoBehaviour
    {
      
        public void Update()
        {
            transform.position += transform.forward * 100f;
        }
    }
}

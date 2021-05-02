using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Musashi
{
    public class VFXControl : MonoBehaviour
    {
        [SerializeField] int lifeTime = 1;
        bool init = true;

        private void OnEnable()
        {
            if(init == false)
                gameObject.SetActive(false,lifeTime);

            init = false;
        }
    }
}
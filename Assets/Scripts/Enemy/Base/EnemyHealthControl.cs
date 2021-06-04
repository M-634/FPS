using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Musashi
{
    public class EnemyHealthControl : BaseHealthControl
    {
        [Header("DeadVFX")]
        [SerializeField] GameObject deathEffect;
      
        protected override void Start()
        {
            base.Start();

            if (deathEffect)
            {
                deathEffect.SetActive(false);
            }

            OnDieEvents.AddListener(SetDeathEffect);
        }

        private void SetDeathEffect()
        {
            if (deathEffect)
            {
                deathEffect.transform.position = transform.position;
                deathEffect.SetActive(true);
            }
        }
    }
}
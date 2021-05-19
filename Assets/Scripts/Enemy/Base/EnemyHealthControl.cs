using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Musashi
{
    public class EnemyHealthControl : BaseHealthControl
    {
        [Header("DeadVFX")]
        [SerializeField] GameObject dethEffect;
        EnemyAI owner;

        protected override void Start()
        {
            base.Start();

            owner = GetComponent<EnemyAI>();

            if (dethEffect)
            {
                dethEffect.SetActive(false);
            }
        }

        public override void OnDamage(float damage)
        {
            base.OnDamage(damage);

            if (owner)
            {
                owner.ChangeState(owner.EnemyOnDamage);
            }
        }

        protected override void OnDie()
        {
            IsDead = true;

            //Active ragdoll
            if (dethEffect)
            {
                dethEffect.transform.position = transform.position;
                dethEffect.SetActive(true);
            }


            //hide helth bar
            if (healthBarFillImage)
            {
                healthBarFillImage.transform.parent.gameObject.SetActive(false);
            }

            //hide enemy object
            transform.gameObject.SetActive(false);

            //EventManeger.Instance.Excute(EventType.EnemyDie);
        }
    }
}
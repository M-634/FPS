using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Musashi
{
    public class EnemyHealthControl : BaseHealthControl
    {
        [Header("RagDoll")]
        [SerializeField] GameObject dethEffect;
        [SerializeField] float healthBarHightOffset;

        EnemyAI owner;

        protected override void Start()
        {
            base.Start();

            owner = GetComponent<EnemyAI>();

            if (dethEffect)
                dethEffect.SetActive(false);
        }


        void Update()
        {
            //BillBord
            if (healthBarFillImage)
            {
                healthBarFillImage.transform.parent.position = transform.position + Vector3.up * healthBarHightOffset;
                healthBarFillImage.transform.parent.LookAt(Camera.main.transform.position);
            }
        }

        public override void OnDamage(float damage)
        {
            if (isDead) return;

            CurrentHp -= damage;
            if (CurrentHp <= 0)
            {
                OnDie();
            }
            else
            {
                if (owner)
                    owner.ChangeState(owner.EnemyOnDamage);
            }
        }

        protected override void OnDie()
        {
            isDead = true;

            //Active ragdoll
            if (dethEffect)
            {
                dethEffect.transform.position = transform.position;
                dethEffect.SetActive(true);
            }


            //hide helth bar
            if (healthBarFillImage)
                healthBarFillImage.transform.parent.gameObject.SetActive(false);

            //hide enemy object
            transform.gameObject.SetActive(false);

            //EventManeger.Instance.Excute(EventType.EnemyDie);
        }
    }
}
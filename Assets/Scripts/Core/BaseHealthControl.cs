using UnityEngine;
using UnityEngine.UI;

namespace Musashi
{
    public abstract class BaseHealthControl : MonoBehaviour, IDamageable
    {
        [SerializeField] protected float maxHp;
        [SerializeField] protected Image healthBarFillImage = default;

        protected float currentHp;
        protected float CurrentHp { get => currentHp;
            set
            {
                currentHp = value;
                if (healthBarFillImage)
                    healthBarFillImage.fillAmount = currentHp / maxHp;
            } 
        }
        protected bool isDead;

        protected virtual void Start()
        {
            CurrentHp = maxHp;
        }

        public virtual void OnDamage(float damage)
        {
            CurrentHp -= damage;

            //if (healthBarFillImage)
            //    healthBarFillImage.fillAmount = CurrentHp / maxHp;

            if (CurrentHp <= 0) OnDie();
        }

        protected abstract void OnDie();
    }
}
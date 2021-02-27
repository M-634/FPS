using UnityEngine;
using UnityEngine.UI;

namespace Musashi
{
    public abstract class BaseHealthControl : MonoBehaviour, IDamageable
    {
        [SerializeField] protected float maxHp;
        [SerializeField] protected Image healthBarFillImage = default;

        protected float currentHp;
        protected bool isDead;

        protected virtual void Start()
        {
            currentHp = maxHp;
        }

        public virtual void OnDamage(float damage)
        {
            currentHp -= damage;

            if (healthBarFillImage)
                healthBarFillImage.fillAmount = currentHp / maxHp;

            if (currentHp < 0) OnDie();
        }

        protected abstract void OnDie();
    }
}
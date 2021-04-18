using UnityEngine;
using UnityEngine.UI;

namespace Musashi
{
    /// <summary>
    /// 体力があるオブジェクトにアタッチするベースクラス。
    /// 設定された体力がなくなるとそのオブジェットは消える
    /// </summary>
    public abstract class BaseHealthControl : MonoBehaviour, IDamageable
    {
        [SerializeField] protected float maxHp;
        [SerializeField] protected Image healthBarFillImage = default;

        protected bool isDead;
        protected float currentHp;

        protected virtual float CurrentHp 
        {
            get => currentHp;
            set
            {
                currentHp = value;
                if (healthBarFillImage)
                    healthBarFillImage.fillAmount = currentHp / maxHp;
            } 
        }

        protected virtual void Start()
        {
            CurrentHp = maxHp;
        }

        public virtual void OnDamage(float damage)
        {
            CurrentHp -= damage;
            if (CurrentHp <= 0) OnDie();
        }

        protected abstract void OnDie();
    }
}
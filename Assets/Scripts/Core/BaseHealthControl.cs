using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Musashi
{
    /// <summary>
    /// 体力があるオブジェクトにアタッチするベースクラス。
    /// 設定された体力がなくなるとそのオブジェットは消える
    /// </summary>
    public abstract class BaseHealthControl : MonoBehaviour, IDamageable
    {
        [SerializeField] TargetType targetType;
        [SerializeField] bool useBillBord;
        [SerializeField] float healthBarHightOffset;
        [SerializeField] protected float maxHp;
        [SerializeField] protected Image healthBarFillImage;
        protected float currentHp;


        public bool IsDead { get; protected set; } = false;
        public bool UseBillBord => useBillBord;
        protected virtual float CurrentHp
        {
            get => currentHp;
            set
            {
                if (value < 0) value = 0;
                currentHp = value;
                if (healthBarFillImage)
                {
                    healthBarFillImage.fillAmount = currentHp / maxHp;
                }
            }
        }

        protected virtual void Start()
        {
            CurrentHp = maxHp;

            if (healthBarFillImage && useBillBord)
            {
                StartCoroutine(BillBoard());
            }
        }

        IEnumerator BillBoard()
        {
            while (!IsDead)
            {
                healthBarFillImage.transform.parent.position = transform.position + Vector3.up * healthBarHightOffset;
                healthBarFillImage.transform.parent.LookAt(Camera.main.transform.position);
                yield return null;
            }
        }

        public virtual void OnDamage(float damage)
        {
            if (IsDead) return;

            CurrentHp -= damage;
            if (CurrentHp <= 0)
            {
                OnDie();
            }
        }

        protected virtual void OnDie()
        {
            IsDead = true;

            if(healthBarFillImage)
            {
                healthBarFillImage.transform.parent.gameObject.SetActive(false);
            }
            gameObject.SetActive(false);
        }

        public TargetType GetTargetType()
        {
            return targetType;
        }
    }
}
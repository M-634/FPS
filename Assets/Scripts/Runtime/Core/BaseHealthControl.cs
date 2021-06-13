using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;

namespace Musashi
{
    /// <summary>
    /// ダメージ時のイベントラッパー関数
    /// </summary>
    [System.Serializable]
    public class OnDamageEnvents : UnityEvent { }

    /// <summary>
    /// 死亡時のイベントラッパー関数
    /// </summary>
    [System.Serializable]
    public class OnDieEvents : UnityEvent { }

    /// <summary>
    /// 体力があるオブジェクトにアタッチするベースクラス。
    /// ダメージ時のイベントと、体力がなくなった時（死亡時）のイベントを
    /// 継承先、もしくはインスペクター上で設定できる。
    /// 
    /// リファクタリングメモ : Start OnDamage OnDie メソッドの各 virtual キーワードを外すこと
    /// </summary>
    public abstract class BaseHealthControl : MonoBehaviour, IDamageable
    {
        [SerializeField] TargetType targetType;
        [SerializeField] bool useBillBord;
        [SerializeField] float healthBarHightOffset;
        [SerializeField] protected float maxHp;
        [SerializeField] protected Image healthBarFillImage;

        [SerializeField] protected OnDamageEnvents OnDamageEnvents = default;
        [SerializeField] protected OnDieEvents OnDieEvents = default;

        [SerializeField] bool isDebugMode;//ダメージを受け付けないフラグ

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

            OnDamageEnvents.AddListener(AddOnDamageEvent);
            OnDieEvents.AddListener(AddOnDieEvent);
        }


        protected virtual void AddOnDamageEvent() { }

        protected virtual void AddOnDieEvent() { }

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
            if (IsDead || isDebugMode) return;

            CurrentHp -= damage;

            if (CurrentHp <= 0)
            {
                OnDie();
                return;
            }

            if (OnDamageEnvents != null)
            {
                OnDamageEnvents.Invoke();
            }
        }

        protected virtual void OnDie()
        {
            IsDead = true;

            if (OnDieEvents != null)
            {
                OnDieEvents.Invoke();
            }
        }

        public TargetType GetTargetType()
        {
            return targetType;
        }
    }
}
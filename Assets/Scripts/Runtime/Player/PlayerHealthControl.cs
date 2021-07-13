using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


namespace Musashi.Player
{
    public sealed class PlayerHealthControl : BaseHealthControl
    {
        [Header("damage effect property")]
        ///<summary>被ダメージ時に出るイメージエフェクト</summary>
        [SerializeField] Image damageEffectImage = default;
        /// <summary>ダメージエフェクトのイメージカラーのアルファ値が０に戻るまでの時間</summary>
        [SerializeField] float autoHealTime = 1f;

        public  bool IsMaxHP  => CurrentHp == maxHp;
        protected override float CurrentHp 
        {
            get => base.CurrentHp;
            set
            {
                currentHp = value;
                float ratio = currentHp / maxHp;
                if (healthBarFillImage)
                {
                    healthBarFillImage.fillAmount = ratio;
                }
                if (damageEffectImage)
                {
                    float invRatio = 1 - ratio;
                    damageEffectImage.SetOpacityImageColor(invRatio);
                    DOTween.To(() => invRatio, (x) => invRatio = x, 0f, autoHealTime)
                        .OnUpdate(()=> damageEffectImage.SetOpacityImageColor(invRatio));
                }
            } 
        }

        protected override void Start()
        {
            base.Start();
            GameEventManager.Instance.Subscribe(GameEventType.Goal, ResetHP);
        }

        private void OnDestroy()
        {
            GameEventManager.Instance.UnSubscribe(GameEventType.Goal, ResetHP);
        }

        protected override void AddOnDieEvent()
        {
            GameManager.Instance.GameOver();
        }

        public bool Heal(float healPoint,float healtime)
        {
            CurrentHp += healPoint;
            return true;
            //memo 回復タイムの実装は後回しにする
        }
    }
}
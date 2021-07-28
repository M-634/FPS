using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


namespace Musashi.Player
{
    public sealed class PlayerHealthControl : BaseHealthControl
    {
        [Header("Set player's back ground hp bar image and Color")]
        [SerializeField] Image hpBackgroundBar = default;
        [SerializeField] Color takeDamgeColor = Color.red;
        [SerializeField] Color healHpColor = Color.green;
        [SerializeField] float backBarToFillBarDuration = 0.5f;

        [Header("Damage effect property")]
        ///<summary>被ダメージ時に出るイメージエフェクト</summary>
        [SerializeField] Image damageEffectImage = default;
        /// <summary>ダメージエフェクトのイメージカラーのアルファ値が０に戻るまでの時間</summary>
        [SerializeField] float autoHealTime = 1f;

        Tweener currentHealingTweener;
        PlayerCharacterStateMchine stateMchine;

        public bool IsHealing { get; set; } = false;

        protected override void Start()
        {
            stateMchine = GetComponent<PlayerCharacterStateMchine>();
            base.Start();
            hpBackgroundBar.fillAmount = CurrentHp / maxHp;
            GameEventManager.Instance.Subscribe(GameEventType.Goal, ResetHP);
        }

        private void OnDestroy()
        {
            GameEventManager.Instance.UnSubscribe(GameEventType.Goal, ResetHP);
        }

        public override void ResetHP()
        {
            base.ResetHP();
        }

        protected override void AddOnDamageEvent()
        {
            if (damageEffectImage)
            {
                float invRatio = 1 - CurrentHp / maxHp;
                damageEffectImage.SetOpacityImageColor(invRatio);
                DOTween.To(() => invRatio, (x) => invRatio = x, 0f, autoHealTime)
                    .OnUpdate(() => damageEffectImage.SetOpacityImageColor(invRatio));
            }

            if (hpBackgroundBar)
            {
                //hpバーの赤い部分を徐々に減らす
                hpBackgroundBar.color = takeDamgeColor;
                DOTween.To(() => hpBackgroundBar.fillAmount, (x) => hpBackgroundBar.fillAmount = x, healthBarFillImage.fillAmount, backBarToFillBarDuration)
                       .SetEase(Ease.Linear);
            }

            CancelHealAction();
        }

        protected override void AddOnDieEvent()
        {

            if (currentHealingTweener != null && IsHealing)
            {
                currentHealingTweener.Kill();
            }
            hpBackgroundBar.fillAmount = 0f;
            //GameManager.Instance.GameOver();
        }

        /// <summary>
        /// プレイヤーが一撃で死ぬ場合に呼ばれる関数
        /// </summary>
        public void Kill()
        {
            OnDamage(maxHp);
        }

        public void CancelHealAction()
        {
            if (currentHealingTweener != null && IsHealing)
            {
                currentHealingTweener.Kill();
            }
        }

        /// <summary>
        /// プレイヤーの体力を回復する関数
        /// </summary>
        /// <param name="healPoint">回復量</param>
        /// <param name="healtime">回復時間</param>
        /// <returns></returns>
        public bool Heal(float healPoint, float healtime)
        {
            if (CurrentHp == maxHp || IsHealing)
            {
                Debug.Log("体力が既に満タンか、回復中です。");
                return false;
            }

            if (hpBackgroundBar)
            {
                currentHealingTweener = null;
                hpBackgroundBar.color = healHpColor;
                var targetHP = CurrentHp + healPoint;
                hpBackgroundBar.fillAmount = targetHP / maxHp;

                //回復時間に沿って、徐々にHPを回復させる
                currentHealingTweener = DOTween.To(() => healthBarFillImage.fillAmount, (x) => healthBarFillImage.fillAmount = x, hpBackgroundBar.fillAmount, healtime)
                    .SetEase(Ease.Linear)
                    .OnUpdate(() => IsHealing = true)
                    .OnKill(()=>
                    {
                        IsHealing = false;
                        Debug.Log("回復を中断しました");
                    })
                    .OnComplete(() =>
                    {
                        IsHealing = false;
                        CurrentHp = targetHP;
                        Debug.Log("回復完了");
                    });
            }
            else
            {
                CurrentHp += healPoint;
            }
            return true;
        }
    }
}
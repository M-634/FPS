using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;


namespace Musashi
{
    public sealed class BossHpCore : BaseHealthControl
    {
        [SerializeField] Image healthBarRed;
        [SerializeField] float fullHpDuration = 1f;
        [SerializeField] float backBarToFillBarDuration;
        [SerializeField] UnityEvent bossDeadEvent;

        public bool invincible = true;//無敵モードフラグ(ダメージ受けない)
         
        protected override void Start()
        {
            //（fullHpDuration）秒かけて㏋ゲージを上昇させる
            DOTween.To(() => CurrentHp, (x) => CurrentHp = x, maxHp, fullHpDuration)
                   .SetEase(Ease.Linear)
                   .OnComplete(() => 
                   {
                       healthBarRed.fillAmount = 1f;
                       invincible = false;
                   });
        }


        public override void OnDamage(float damage)
        {
            if (IsDead || invincible) return;

            CurrentHp -= damage;

            if (CurrentHp <= 0)
            {
                IsDead = true;
            }

            //hpバーの赤い部分を徐々に減らす
            DOTween.To(() => healthBarRed.fillAmount, (x) => healthBarRed.fillAmount = x, healthBarFillImage.fillAmount, backBarToFillBarDuration)
                   .SetEase(Ease.Linear)
                   .OnComplete(() =>
                   {
                       if (IsDead)
                       {
                           OnDie();
                       }
                   });
        }


        protected override void OnDie()
        {
            //IsDead = true;

            if (bossDeadEvent != null)
            {
                bossDeadEvent.Invoke();
            }
        }
    }
}


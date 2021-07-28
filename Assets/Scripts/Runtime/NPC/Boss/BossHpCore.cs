using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;


namespace Musashi.NPC
{
    public sealed class BossHpCore : BaseHealthControl
    {
        [SerializeField] Image healthBarRed;

        [SerializeField] float fullHpDuration = 1f;
        [SerializeField] float backBarToFillBarDuration;

        protected override void Start()
        {
            base.Start();
            //出現時に（fullHpDuration）秒かけて㏋ゲージを上昇させる
            isInvincibleMode = true;
            DOTween.To(() => CurrentHp, (x) => CurrentHp = x, maxHp, fullHpDuration)
                   .SetEase(Ease.Linear)
                   .OnComplete(() =>
                   {
                       healthBarRed.fillAmount = 1f;
                       isInvincibleMode = false;
                   });
        }

        public override void ResetHP()
        {
            IsDead = false;
        }

        protected override void AddOnDamageEvent()
        {
            if (healthBarFillImage && healthBarRed)
            {
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
        }
    }
}


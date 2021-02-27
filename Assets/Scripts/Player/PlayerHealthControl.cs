using UnityEngine;

namespace Musashi
{
    public class PlayerHealthControl : BaseHealthControl
    {
        public override void OnDamage(float damage)
        {
            if (isDead) return;

            base.OnDamage(damage);
            Debug.Log("ダメージを喰らった！");
        }

        protected override void OnDie()
        {
            isDead = true;
            GameManager.Instance.GameOver();
        }
    }
}
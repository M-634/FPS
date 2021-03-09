using UnityEngine;

namespace Musashi
{
    public class PlayerHealthControl : BaseHealthControl
    {
        public  bool IsMaxHP { get => currentHp == maxHp; }
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

        public void Heal(float healPoint,float healtime)
        {
            Debug.Log($"{healPoint}を{healtime}かけて回復した");
        }

        private void OnEnable()
        {
            EventManeger.Instance.Subscribe(Heal);
        }

        private void OnDisable()
        {
            EventManeger.Instance.UnSubscribe(Heal);
        }
    }
}
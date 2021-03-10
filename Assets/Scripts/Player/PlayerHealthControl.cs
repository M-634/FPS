using UnityEngine;

namespace Musashi
{
    public class PlayerHealthControl : BaseHealthControl
    {
        public  bool IsMaxHP { get => CurrentHp == maxHp; }

        public override void OnDamage(float damage)
        {
            if (isDead) return;

            base.OnDamage(damage);
            Debug.Log($"{damage}ダメージを喰らった！");
        }

        protected override void OnDie()
        {
            isDead = true;
            GameManager.Instance.GameOver();
        }

        public void Heal(float healPoint,float healtime)
        {
            CurrentHp += healPoint;
            //memo 回復タイムの実装は後回しにする
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
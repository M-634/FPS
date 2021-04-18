using UnityEngine;
using UnityEngine.UI;


namespace Musashi
{
    public class PlayerHealthControl : BaseHealthControl
    {
        [SerializeField] Image damageEffectImage;
   
        protected override float CurrentHp 
        {
            get => base.CurrentHp;
            set
            {
                currentHp = value;
                float ratio = currentHp / maxHp;
                if (healthBarFillImage)
                    healthBarFillImage.fillAmount = ratio;
                if (damageEffectImage)
                    damageEffectImage.color = new Color(1, 1, 1, 1 - ratio);
            } 
        }
        public  bool IsMaxHP  => CurrentHp == maxHp;

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

        //private void OnEnable()
        //{
        //    EventManeger.Instance.Subscribe(Heal);
        //}

        //private void OnDisable()
        //{
        //    EventManeger.Instance.UnSubscribe(Heal);
        //}
    }
}
using UnityEngine;
using UnityEngine.UI;


namespace Musashi.Player
{
    public class PlayerHealthControl : BaseHealthControl
    {
        [SerializeField] Image damageEffectImage;
   
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
                    damageEffectImage.color = new Color(1, 1, 1, 1 - ratio);
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
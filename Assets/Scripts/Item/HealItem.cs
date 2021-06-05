using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Musashi.Player;

namespace Musashi.Item
{
    public class HealItem : Item
    {
        [SerializeField] float healPoint = 30f;
        [SerializeField] float healtime = 60f;

        protected override void Start()
        {
            base.Start();
            OnUseEvent += CanHealPlayer;
        }

        public bool CanHealPlayer()
        {
            var healthControl = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealthControl>();

            if (healthControl)
            {
                if (healthControl.IsMaxHP)
                {
                    InteractiveMessage.WarningMessage(InteractiveMessage.HPISFull);
                }
                else
                {
                    return healthControl.Heal(healPoint, healtime);//回復時間を実装する
                }
            }
            return false;
        }
    }
}

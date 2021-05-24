using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Musashi
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
            PlayerHealthControl healthControl = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealthControl>();

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Musashi
{
    public class HealItem : BaseItem
    {
        [SerializeField] float healPoint = 30f;
        [SerializeField] float healtime = 60f;

        public override void OnPicked()
        {
            base.OnPicked();
        }

        public override bool CanUseItem()
        {
            var playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealthControl>();
            if (playerHealth.IsMaxHP)
            {
                InteractiveMessage.WarningMessage(InteractiveMessage.HPISFull);
                canUseItem = false;
            }
            else
            {
                canUseItem = true;
                EventManeger.Instance.Excute(healtime, healPoint);
            }
            Destroy(gameObject, 0.1f);
            return canUseItem;
        }
    }
}

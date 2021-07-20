using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Musashi.Player;

namespace Musashi.Item
{
    [RequireComponent(typeof(PickUp))]
    public class HealItem : BaseItem
    {
        [SerializeField] float healPoint = 30f;
        [SerializeField] float healtime = 60f;

        PickUp pickUp;

        private void Start()
        {
            pickUp = GetComponent<PickUp>();
            pickUp.OnPickEvent += PickUp_OnPickEvent;
        }

        private void PickUp_OnPickEvent(Transform player)
        {
            Ower = player;
            if(player.TryGetComponent(out PlayerItemInventory inventory))
            {
                if (inventory.AddItem(this))
                {
                    pickUp.HavePicked = true;
                    gameObject.SetActive(false);
                }
            }
        }

        public override bool UseItem()
        {
            if(Ower.TryGetComponent(out PlayerHealthControl healthControl))
            {
                return healthControl.Heal(healPoint, healtime);
            }
            Debug.LogError("PlayerHealthControlがプレイヤーにアタッチされていません。");
            return false;
        }
    }
}

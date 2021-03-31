using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Musashi
{
    public abstract class BaseWeapon : BaseItem
    {
        protected bool hasPlayer = false;
        public abstract void Attack();

        public override void OnPicked()
        {
            canPickUp = Inventory.Instance.CanGetWeapon(this);
            if (canPickUp)
            {
                GameManager.Instance.SoundManager.PlaySE(SoundName.PickUP);
                hasPlayer = true;
            }
        }

        public override void Drop()
        {
            base.Drop();
            hasPlayer = false;
        }


        private void OnEnable()
        {
            EventManeger.Instance.Subscribe(EventType.OpenInventory, () => canUseItem = false);
            EventManeger.Instance.Subscribe(EventType.CloseInventory, () => canUseItem = true);
        }

        private void OnDisable()
        {
            EventManeger.Instance.UnSubscribe(EventType.OpenInventory, () => canUseItem = false);
            EventManeger.Instance.UnSubscribe(EventType.CloseInventory, () => canUseItem = true);
        }
    }
}

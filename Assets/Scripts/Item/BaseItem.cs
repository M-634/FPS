using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Musashi
{
    public enum KindOfItem
    {
        //defult
        None = 0,
        //health
        HealthKit, Apple,
        //other
        Battery, Key, AmmoBox,
    }

    public abstract class BaseItem : CanPickUpObject
    {
        public KindOfItem kindOfItem;
        protected bool canPickUp = true;
        protected bool canUseItem = true;

        public override void OnPicked(GameObject player)
        {
            canPickUp = player.GetComponent<PlayerItemInventory>().CanGetItem(this, 1);
            if (canPickUp)
            {
                GameManager.Instance.SoundManager.PlaySE(SoundName.PickUP);
                Destroy(gameObject);
            }
        }

        public virtual bool CanUseObject(GameObject player)
        {
            return canUseItem;
        }
    }
}
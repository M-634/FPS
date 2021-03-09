using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Musashi
{
    public abstract class BaseItem : MonoBehaviour, IPickUpObjectable
    {
        public KindOfItem kindOfitem;
        public bool canPickUp = true;

        public virtual void OnPicked()
        {
            canPickUp = Inventory.Instance.CanGetItem(this);
            if (canPickUp)
                Destroy(gameObject);
        }

        public virtual void UseItem() { }
    }


    /// <summary>
    /// 武器やアイテムの中で拾って使うもの
    /// </summary>
    public interface IPickUpObjectable
    {
        void OnPicked();
        void UseItem();
    }

    public enum KindOfItem
    {
        //defult
        None = 0,
        //health
        HealthKit, Apple,
        //weapon
        ShotGun, HandGun, AssaultRifle, Grenade, Axe, CrossBow,
        //other
        Battery, Key,
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Musashi
{
    public abstract class BaseItem : MonoBehaviour, IPickUpObjectable
    {
        [SerializeField] protected KindOfItem item;

        public virtual void OnPicked()
        {
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
        //health
        HealthKit, Apple,
        //weapon
        ShotGun, HandGun,AssaultRifle,Grenade,Axe,CrossBow,
        //other
        Battery,Key,
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Musashi
{
    public enum KindOfWeapon
    {
        //defult
        None = 0,
        //Gun
        ShotGun, HandGun, AssaultRifle,
        //other
        Grenade, Axe, CrossBow,
    }

    public abstract class BaseWeapon : MonoBehaviour
    {
        public KindOfWeapon kindOfWeapon;
        protected bool canUseWeapon = true;

        public virtual void Attack() { }

        private void OnEnable()
        {
            EventManeger.Instance.Subscribe(EventType.OpenInventory, () => canUseWeapon = false);
            EventManeger.Instance.Subscribe(EventType.CloseInventory, () => canUseWeapon = true);
        }

        private void OnDisable()
        {
            EventManeger.Instance.UnSubscribe(EventType.OpenInventory, () => canUseWeapon = false);
            EventManeger.Instance.UnSubscribe(EventType.CloseInventory, () => canUseWeapon = true);
        }
    }
}

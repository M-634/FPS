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
    }
}

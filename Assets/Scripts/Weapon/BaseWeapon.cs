using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Musashi
{
    public enum WeaponType
    {
        ShotGun, HandGun, AssaultRifle,
    }

    public abstract class BaseWeapon : MonoBehaviour
    {
        public WeaponType weaponType;
    }
}

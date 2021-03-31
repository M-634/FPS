using System.Collections;
using UnityEngine;
using System;

namespace Musashi
{
    [Serializable]
    [CreateAssetMenu(fileName ="WeaponData",menuName ="CreateWeaponData")]
    public class WeaponData : ScriptableObject
    {
        [SerializeField] KindOfWeapon kindOfWeapon;
        [SerializeField] Sprite icon;
        [SerializeField] BaseWeapon weaponPrefab;
        [SerializeField] BaseWeapon pickUpWeaponPrefab;

        public KindOfWeapon KindOfWeapon { get => kindOfWeapon; }
        public Sprite Icon { get => icon;  }
        public BaseWeapon WeaponPrefab { get=> weaponPrefab;  }
        public BaseWeapon PickUpWeaonPrefab { get => pickUpWeaponPrefab; }
    }
}

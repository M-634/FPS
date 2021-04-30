using System.Collections;
using UnityEngine;
using System;

namespace Musashi
{
    [Serializable]
    [CreateAssetMenu(fileName ="WeaponData",menuName ="CreateWeaponData")]
    public class WeaponData : ScriptableObject
    {
        [SerializeField] WeaponType kindOfWeapon;
        [SerializeField] Sprite icon;
        [SerializeField] BaseWeapon weaponPrefab;
        [SerializeField] PickUpWeapon pickUpWeaponPrefab;

        public WeaponType KindOfWeapon => kindOfWeapon;
        public Sprite Icon => icon;
        public BaseWeapon WeaponPrefab => weaponPrefab;
        public PickUpWeapon PickUpWeaonPrefab => pickUpWeaponPrefab;
    }
}

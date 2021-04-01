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
        [SerializeField] PickUpWeapon pickUpWeaponPrefab;

        public KindOfWeapon KindOfWeapon => kindOfWeapon;
        public Sprite Icon => icon;
        public BaseWeapon WeaponPrefab => weaponPrefab;
        public PickUpWeapon PickUpWeaonPrefab => pickUpWeaponPrefab;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Musashi
{

    [CreateAssetMenu(fileName = "WeaponDataBase", menuName = "CreateWeaponDataBase")]
    public class WeaponDataBase : ScriptableObject
    {
        [SerializeField] List<WeaponData> weaponDataList = new List<WeaponData>();

        public List<WeaponData> WeaponDataList { get => weaponDataList; }
    }
}

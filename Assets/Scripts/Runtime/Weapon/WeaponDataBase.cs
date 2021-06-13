using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Musashi.Weapon
{

    [CreateAssetMenu(fileName = "WeaponDataBase", menuName = "CreateWeaponDataBase")]
    public class WeaponDataBase : ScriptableObject
    {
        [SerializeField] List<WeaponSettingSOData> weaponDataList = new List<WeaponSettingSOData>();
        public List<WeaponSettingSOData> WeaponDataList  => weaponDataList; 
    }
}

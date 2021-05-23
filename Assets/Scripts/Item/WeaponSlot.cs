using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace Musashi
{
    public class WeaponSlot : SlotBase
    {
        public int slotNumber;
        PlayerWeaponManager weaponManager;
        public void SetWeaponSlot(int slotNumber,PlayerWeaponManager weaponManager)
        {
            this.slotNumber = slotNumber;
            this.weaponManager = weaponManager;
        }

        //public override void DropItem()
        //{
        //    weaponManager.PutAwayCurrentWeapon();
        //    base.DropItem();
        //}
    }
}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace Musashi
{
    public class WeaponSlot : SlotBase
    {
        int slotNumber;
        PlayerWeaponManager weaponManager;
        public void SetWeaponSlot(int slotNumber,PlayerWeaponManager weaponManager)
        {
            this.slotNumber = slotNumber;
            this.weaponManager = weaponManager;
        }

        public override void ResetInfo()
        {
            base.ResetInfo();
            //装備中の武器を解除する
            weaponManager.EquipmentWeaponSetActiveFalse();
        }
    }
}

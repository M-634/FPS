using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace Musashi
{
    public class WeaponSlot : SlotBase
    {
        public int slotNumber;
        public override bool IsFilled => IsEmpty == false;
        public override void SetInfo(ItemData getItemData)
        {
            base.SetInfo(getItemData);
        }

        /// <summary>
        /// スロットをクリックして武器を装備する
        /// </summary>
        public override void UseItemInSlot()
        {
            if (IsEmpty) return;

            playerInteractive.EquipmentWeaponByShotCutKeyOrInventory(slotNumber);
            Inventory.Instance.OpenAndCloseInventory();
        }
    }
}

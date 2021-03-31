using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace Musashi
{
    public class WeaponSlot : SlotBase
    {
        public int slotNumber;
        public int activeWeaponIndex;//activeされるweapon配列のインデックス
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

            ItemInventory.Instance.OpenAndCloseInventory();
        }
    }
}

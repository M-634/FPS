using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace Musashi
{
    public class WeaponSlot : SlotBase
    {

        public override bool IsFilled => IsEmpty == false;
        public override void SetInfo(ItemData getItemData)
        {
            base.SetInfo(getItemData);
        }

        /// <summary>
        /// 武器を装備する
        /// </summary>
        public override void UseItemInSlot()
        {
            if (IsEmpty) return;

            //既に装備していて...
            if (playerInteractive.CurrentHaveWeapon)
            {
                //同一武器なら何もしない(ここ修正ポイントインスタンスIdで比べる)
                if (playerInteractive.CurrentHaveWeapon.KindOfItem == currentItemData.KindOfItem) return;
            }

            playerInteractive.EquipmentWeapon(currentItemData.KindOfItem);
            Inventory.Instance.OpenAndCloseInventory();
        }
    }
}

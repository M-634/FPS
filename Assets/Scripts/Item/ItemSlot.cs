using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace Musashi
{
    public class ItemSlot : SlotBase 
    {
        PlayerItemInventory itemInventory;
      
        /// <summary>
        /// 初期化時に PlayerItemInventoryクラスから呼ばれる関数
        /// </summary>
        /// <param name="_itemInventory"></param>
        public void SetItemSlot(PlayerItemInventory _itemInventory)
        {
            itemInventory = _itemInventory;
        }

     
        public void AddItemInSlot(Item getItem)
        {
            if (IsFilled) return;
            int temp = StacSizeInSlot + getItem.StacSize;

            if (temp > maxStacSizeInSlot)
                temp = maxStacSizeInSlot;

            StacSizeInSlot = temp;
            itemsInSlot.Enqueue(getItem);
        }

        public override void DropObject()
        {
            if (IsEmpty) return;
            //var item = Instantiate(CurrentItemData.ItemPrefab, playerCamera.position + playerCamera.forward * 2f, Quaternion.identity);
            //item.Drop(playerCamera);
            //stackNumber--;
            //stack.text = stackNumber.ToString() + " / " + currentItemData.MaxStackNumber.ToString();
            //IsFilled = false;

            //if (stackNumber == 0)
            //    ResetInfo();
        }

        //public override void ResetInfo()
        //{
        //    //currentItemData = null;
        //    icon.sprite = null;
        //    //stackNumber = 0;
        //    //stack.text = "";
        //}
    }
}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace Musashi
{
    public class ItemSlot : SlotBase 
    {
        PlayerItemInventory itemInventory;
        private ItemData currentItemData;
        public ItemData CurrentItemData => currentItemData;

        public override bool IsEmpty => currentItemData == null;

        int stackNumber = 0;

        /// <summary>
        /// 初期化時に PlayerItemInventoryクラスから呼ばれる関数
        /// </summary>
        /// <param name="_itemInventory"></param>
        public void SetItemSlot(PlayerItemInventory _itemInventory)
        {
            itemInventory = _itemInventory;
        }

        public override void SetInfo<T>(T getData)
        {
            currentItemData = getData as ItemData;
            if(currentItemData.Icon)
                icon.sprite = currentItemData.Icon;

            stackNumber++;
            stack.text = stackNumber.ToString() + " / " + currentItemData.MaxStackNumber.ToString();
        }

        public void AddItemInSlot(int getNumber)
        {
            if (IsFilled) return;
            stackNumber += getNumber;
            if (stackNumber >= currentItemData.MaxStackNumber)
            {
                IsFilled = true;
                stackNumber = currentItemData.MaxStackNumber;
            }
            stack.text = stackNumber.ToString() + " / " + currentItemData.MaxStackNumber.ToString();
        }

        public override void UseObject(GameObject player)
        {
            if (IsEmpty) return;

            var item = Instantiate(CurrentItemData.ItemPrefab);

            //アイテムが使用できたならスタック数を減らす
            if (item.CanUseObject(player))
            {
                stackNumber--;
                stack.text = stackNumber.ToString() + " / " + currentItemData.MaxStackNumber.ToString();
                IsFilled = false;

                if (stackNumber == 0)
                    ResetInfo();

                itemInventory.OpenAndCloseInventory();
            }
        }

        public override void DropObject(Vector3 worldPoint)
        {
            if (IsEmpty) return;
            var item = Instantiate(CurrentItemData.ItemPrefab,worldPoint,Quaternion.identity);
            item.Drop();

            stackNumber--;
            stack.text = stackNumber.ToString() + " / " + currentItemData.MaxStackNumber.ToString();
            IsFilled = false;

            if (stackNumber == 0)
                ResetInfo();
        }

        public override void ResetInfo()
        {
            currentItemData = null;
            icon.sprite = null;
            stackNumber = 0;
            stack.text = "";
        }
    }
}

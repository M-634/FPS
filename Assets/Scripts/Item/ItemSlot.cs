using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace Musashi
{
    public class ItemSlot : SlotBase 
    {
        private ItemData currentItemData;
        public ItemData CurrentItemData => currentItemData;

        public override bool IsEmpty => currentItemData == null;

        int stackNumber = 0;
        public override void SetInfo<T>(T getData)
        {
            currentItemData = getData as ItemData;
            icon.sprite = currentItemData.Icon;

            stackNumber++;
            stack.text = stackNumber.ToString() + " / " + currentItemData.MaxStackNumber.ToString();
        }

        public void AddItemInSlot()
        {
            if (IsFilled) return;
            stackNumber++;
            stack.text = stackNumber.ToString() + " / " + currentItemData.MaxStackNumber.ToString();

            if (stackNumber == currentItemData.MaxStackNumber) IsFilled = true;
        }

        public override void UseObject()
        {
            if (IsEmpty) return;

            var item = Instantiate(CurrentItemData.ItemPrefab);

            //アイテムが使用できたならスタック数を減らす
            if (item.CanUseObject())
            {
                stackNumber--;
                stack.text = stackNumber.ToString() + " / " + currentItemData.MaxStackNumber.ToString();
                IsFilled = false;

                if (stackNumber == 0)
                    ResetInfo();

                ItemInventory.Instance.OpenAndCloseInventory();
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

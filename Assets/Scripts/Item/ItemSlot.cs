using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace Musashi
{
    public class ItemSlot : SlotBase 
    {
        int stackNumber = 0;
        public override void SetInfo(ItemData getItemData)
        {
            base.SetInfo(getItemData);
            stackNumber++;
            stack.text = stackNumber.ToString() + " / " + getItemData.MaxStackNumber.ToString();
        }

        public override void AddItemInSlot()
        {
            if (IsFilled) return;
            stackNumber++;
            stack.text = stackNumber.ToString() + " / " + currentItemData.MaxStackNumber.ToString();

            if (stackNumber == currentItemData.MaxStackNumber) IsFilled = true;
        }

        public override void UseItemInSlot()
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

        public override void ThrowAwayItem(Vector3 worldPoint)
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
            base.ResetInfo();
            stackNumber = 0;
            stack.text = "";
        }
    }
}

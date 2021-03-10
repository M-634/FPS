using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Musashi
{
    public class Slot : MonoBehaviour
    {
        [SerializeField] Image icon;
        [SerializeField] TextMeshProUGUI stack;
        int stackNumber = 0;

        private ItemData currentItemData;
        public Image Icon { get => icon; set => icon = value; }
        public ItemData CurrentItemData { get => currentItemData; }

        public bool IsEmpty { get => stackNumber == 0; }
        public bool IsFilled 
        {
            get 
            {
                if (!currentItemData) return false;
                return stackNumber == currentItemData.MaxStackNumber; 
            }
        }


        public void SetInfo(ItemData getItemData)
        {
            icon.sprite = getItemData.Icon;
            stackNumber++;
            stack.text = stackNumber.ToString() + " / " + getItemData.MaxStackNumber.ToString();
            currentItemData = getItemData;
        }

        public void AddItemInSlot()
        {
            if (IsFilled) return;
            stackNumber++;
            stack.text = stackNumber.ToString() + " / " + currentItemData.MaxStackNumber.ToString();
        }

        public void UseItemInSlot()
        {
            if (IsEmpty) return;

            var item = Instantiate(CurrentItemData.ItemPrefab);

            //アイテムが使用できたならスタック数を減らす
            if (item.CanUseItem())
            {
                stackNumber--;
                stack.text = stackNumber.ToString() + " / " + currentItemData.MaxStackNumber.ToString();

                if (stackNumber == 0)
                    ResetInfo();
            }
        }

        public void ResetInfo()
        {
            icon.sprite = null;
            stackNumber = 0;
            stack.text = "";
            currentItemData = null;
        }
    }
}

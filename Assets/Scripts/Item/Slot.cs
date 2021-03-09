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
        public ItemData CurrentItemData { get =>currentItemData;}

        public bool IsEmpty { get; set; } = true;
        public bool IsFilled { get; set; } = false;

       
        public void SetInfo(ItemData getItemData)
        {
            icon.sprite = getItemData.Icon;
            IsEmpty = false;
            stackNumber++;
            stack.text = stackNumber.ToString() + " / " + getItemData.MaxStackNumber.ToString();
            currentItemData = getItemData;
        }

        public void AddItemInSlot()
        {
            stackNumber++;

            if (stackNumber == currentItemData.MaxStackNumber)
                IsFilled = true;
               
            stack.text = stackNumber.ToString() + " / " + currentItemData.MaxStackNumber.ToString();
        }

        public void UseItemInSlot()
        {
            if (IsEmpty) return;

            var item = Instantiate(CurrentItemData.ItemPrefab);
            item.UseItem();

            IsFilled = false;
            stackNumber--;
            stack.text = stackNumber.ToString() + " / " + currentItemData.MaxStackNumber.ToString();

            if (stackNumber == 0) 
                ResetInfo();
        }

        
        public void ResetInfo()
        {
            icon.sprite = null;
            IsEmpty = true;
            IsFilled = false;
            stackNumber = 0;
            stack.text = "";
            currentItemData = null;
        }
    }
}

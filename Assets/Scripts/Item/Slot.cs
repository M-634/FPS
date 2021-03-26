using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Musashi
{
    public class Slot : MonoBehaviour
    {
        [SerializeField] Image icon;
        [SerializeField] Image Outline;
        [SerializeField] TextMeshProUGUI keyCode;
        [SerializeField] Color highLightColor;
        [SerializeField] TextMeshProUGUI stack;
        int stackNumber = 0;

        PlayerInteractive playerInteractive;
        public bool IsSelected
        {
            get { return Outline.color == highLightColor; }
            set
            {
                if (value == true)
                {
                    Outline.color = highLightColor;
                    //スロット内が武器なら装備する
                    if (!IsEmpty && CurrentItemData.IsWeapon)
                    {
                        playerInteractive.EquipmentWeapon(CurrentItemData.KindOfItem);
                    }
                }
                else
                {
                    Outline.color = Color.black;
                    //現在武器を装備しているなら外す
                    if (!IsEmpty && CurrentItemData.IsWeapon)
                    {
                        playerInteractive.RemoveEquipment();
                    }
                }
            }
        }

        private ItemData currentItemData;
        public Image Icon { get => icon; set => icon = value; }
        public string KeyCode { set => keyCode.text = value; }
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


        public void Start()
        {
            playerInteractive = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInteractive>();
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

            //スロットないのアイテムが武器かどうかを判定する
            if (CurrentItemData.IsWeapon)
            {
               // playerInteractive.CurrentHaveWeapon.Attack();
                return;
            }


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

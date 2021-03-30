using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace Musashi
{
    /// <summary>
    /// インベントリースロットの抽象クラス
    /// ＜キーマウスの場合＞
    /// カーソルを合わせるとハイライトとアイテムの説明が出てくる。
    /// スロットを左クリックしたらインベントリーを閉じてアイテムを使用する。
    /// 右クリックしたらアイテムを捨てる。
    /// </summary>
    public abstract class SlotBase : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
    {
        [SerializeField] protected Image icon;
        [SerializeField] protected Image Outline;
        [SerializeField] protected TextMeshProUGUI keyCode;
        [SerializeField] protected Color highLightColor;
        [SerializeField] protected TextMeshProUGUI stack;
        protected int stackNumber = 0;

        protected PlayerInteractive playerInteractive;
        //public bool IsSelected
        //{
        //    get { return Outline.color == highLightColor; }
        //    set
        //    {
        //        if (value == true)
        //        {
        //            Outline.color = highLightColor;
        //            //スロット内が武器なら装備する
        //            if (!IsEmpty && CurrentItemData.IsWeapon)
        //            {
        //                playerInteractive.EquipmentWeapon(CurrentItemData.KindOfItem);
        //            }
        //        }
        //        else
        //        {
        //            Outline.color = Color.black;
        //            //現在武器を装備しているなら外す
        //            if (!IsEmpty && CurrentItemData.IsWeapon)
        //            {
        //                playerInteractive.RemoveEquipment();
        //            }
        //        }
        //    }
        //}

        protected ItemData currentItemData;
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


        public virtual void Start()
        {
            playerInteractive = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInteractive>();
        }

        public virtual void SetInfo(ItemData getItemData)
        {
            //icon.sprite = getItemData.Icon;
            //stackNumber++;
            //stack.text = stackNumber.ToString() + " / " + getItemData.MaxStackNumber.ToString();
            //currentItemData = getItemData;
        }

        public virtual void AddItemInSlot()
        {
            //if (IsFilled) return;
            //stackNumber++;
            //stack.text = stackNumber.ToString() + " / " + currentItemData.MaxStackNumber.ToString();
        }

        public virtual void UseItemInSlot()
        {
            //if (IsEmpty) return;

            ////スロットないのアイテムが武器かどうかを判定する
            //if (CurrentItemData.IsWeapon)
            //{
            //   // playerInteractive.CurrentHaveWeapon.Attack();
            //    return;
            //}


            //var item = Instantiate(CurrentItemData.ItemPrefab);

            ////アイテムが使用できたならスタック数を減らす
            //if (item.CanUseItem())
            //{
            //    stackNumber--;
            //    stack.text = stackNumber.ToString() + " / " + currentItemData.MaxStackNumber.ToString();

            //    if (stackNumber == 0)
            //        ResetInfo();
            //}
        }

        public virtual void ResetInfo()
        {
            //icon.sprite = null;
            //stackNumber = 0;
            //stack.text = "";
            //currentItemData = null;
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            Outline.color = highLightColor;
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            Outline.color = Color.black;
        }
    }
}

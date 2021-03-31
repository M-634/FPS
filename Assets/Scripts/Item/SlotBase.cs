using System.Collections;
using System.Collections.Generic;
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
    public abstract class SlotBase : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler
    {
        [SerializeField] protected Image icon;
        [SerializeField] protected Image Outline;
        [SerializeField] protected TextMeshProUGUI keyCode;
        [SerializeField] protected Color highLightColor;
        [SerializeField] protected TextMeshProUGUI stack;
        protected PlayerInteractive playerInteractive;
        protected ItemData currentItemData;
        public Image Icon { get => icon; set => icon = value; }
        public string KeyCode { set => keyCode.text = value; }
        public ItemData CurrentItemData { get => currentItemData; }

        public bool IsEmpty { get => currentItemData == null; }
        public virtual bool IsFilled { get; set; } = false;


        public virtual void Start()
        {
            playerInteractive = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInteractive>();
        }

        public virtual void SetInfo(ItemData getItemData)
        {
            currentItemData = getItemData;
            icon.sprite = getItemData.Icon;
        }

        /// <summary>
        /// アイテムスロットのみに使用する関数。
        /// スロットないのスタック数を増やす。
        /// </summary>
        public virtual void AddItemInSlot(){ }

        /// <summary>
        /// 継承先でアイテムスロットなら使用、武器スロットなら装備する処理を実装する。
        /// その後、使用できたか、装備したらインベントリーを閉じる
        /// </summary>
        public virtual void UseItemInSlot() {}


        public virtual void ThrowAwayItem(Vector3 worldPoint)
        {
            if (IsEmpty) return;

            var item = Instantiate(CurrentItemData.ItemPrefab,worldPoint,Quaternion.identity);
            item.Drop();
            ResetInfo();
        }

        public virtual void ResetInfo()
        {
            icon.sprite = null;
            currentItemData = null;
        }

        bool isSelected = false;

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            Outline.color = highLightColor;
            isSelected = true;
            StartCoroutine(ReciveInteractiveAction());
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            Outline.color = Color.black;
            isSelected = false;
        }
        
        IEnumerator ReciveInteractiveAction()
        {
            while (isSelected)
            {
                if (PlayerInputManager.ClickLeftMouse())
                    UseItemInSlot();

                if (PlayerInputManager.ClickRightMouse())
                {
                    var mousePosition = Input.mousePosition;
                    mousePosition.z += 2f;

                    var worldPoint = Camera.main.ScreenToWorldPoint(mousePosition);
                    ThrowAwayItem(worldPoint);
                }
                yield return null;
            }
        }
    }
}

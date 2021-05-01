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
    public abstract class SlotBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] protected Image icon;
        [SerializeField] protected Image Outline;
        [SerializeField] protected TextMeshProUGUI keyCode;
        [SerializeField] protected Color highLightColor;
        [SerializeField] protected TextMeshProUGUI stackSize;

        public Queue<Item> itemsInSlot;//アイテムスロットのみ
        public Item currentItemInSlot;//次の使用時に使うアイテム

        protected int maxStacSizeInSlot;
        private int stackSizeInSlot;
        public int StacSizeInSlot
        {
            get { return stackSizeInSlot; }
            set
            {
                stackSizeInSlot = value;
                if (stackSizeInSlot > 0)
                    stackSize.text = stackSizeInSlot.ToString() + "/" + maxStacSizeInSlot.ToString();
                else
                    ResetInfo();
            }
        }

        bool isSelected = false;
        public string KeyCode { set => keyCode.text = value; }
        public virtual bool IsEmpty => stackSizeInSlot == 0;
        public virtual bool IsFilled => stackSizeInSlot != 0 && stackSizeInSlot == maxStacSizeInSlot;

        protected PlayerInputManager playerInput;
        protected Transform playerCamera;

        protected virtual void Start()
        {
            playerCamera = Camera.main.transform;
            itemsInSlot = new Queue<Item>();
        }

        /// <summary>
        /// 初期化時に PlayerItemInventoryクラスから呼ばれる。
        /// 必要なコンポーネントを設定する。
        /// </summary>
        /// <param name="_playerInput"></param>
        /// <param name="_itemInventory"></param>
        public void SetInput(PlayerInputManager _playerInput)
        {
            playerInput = _playerInput;
        }

        /// <summary>
        /// 取得したアイテムをスロットにセットする関数。
        /// </summary>
        /// <param name="getItem"></param>
        public virtual void SetInfo(Item getItem)
        {
            currentItemInSlot = getItem;
            icon.sprite = getItem.Icon;
            maxStacSizeInSlot = getItem.MaxStacSize;
            StacSizeInSlot = getItem.StacSize;
            itemsInSlot.Enqueue(getItem);
            getItem.gameObject.SetActive(false);
            GameManager.Instance.SoundManager.PlaySE(SoundName.PickUP);
        }

        /// <summary>
        /// スロットないのアイテムを捨てる関数
        /// </summary>
        /// <param name="worldPoint"></param>
        public virtual void DropObject() { }

        /// <summary>
        /// スロット内のデータを空にする
        /// </summary>
        public virtual void ResetInfo()
        {
            icon.sprite = null;
            stackSizeInSlot = 0;
            maxStacSizeInSlot = 0;
            stackSize.text = "";
            currentItemInSlot = null;
            itemsInSlot.Clear();
        }

        /// <summary>
        /// スロットの上にポインターが来たら、選択されたと判定する関数。
        /// スロットにハイライトをつけ、入力を受け付ける非同期処理を走らせる
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            Outline.color = highLightColor;
            isSelected = true;
            StartCoroutine(ReciveInteractiveAction());
        }

        /// <summary>
        ///スロット上からポインターがなくなったら、選択された判定すを解除し
        ///ハイライトを消す
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnPointerExit(PointerEventData eventData)
        {
            Outline.color = Color.black;
            isSelected = false;
        }

        /// <summary>
        /// スロットを選択を選択されている間、入力を受け付ける関数。
        /// </summary>
        /// <returns></returns>
        IEnumerator ReciveInteractiveAction()
        {
            while (isSelected && !IsEmpty)
            {
              
                if (playerInput.UseItem)
                {
                    currentItemInSlot.OnUseEvent?.Invoke(playerInput.transform.gameObject);

                    if (currentItemInSlot.CanUseItem)
                    {
                        itemsInSlot.Dequeue();
                        Destroy(currentItemInSlot.gameObject);
                        if (itemsInSlot.Count > 0)
                        {
                            currentItemInSlot = itemsInSlot.Peek();
                        }
                        StacSizeInSlot--;
                    }
                }


                if (playerInput.DropItem)
                {
                    // DropObject();
                    currentItemInSlot.OnDropEvent?.Invoke();
                    itemsInSlot.Dequeue();
                    if (itemsInSlot.Count > 0)
                    {
                        currentItemInSlot = itemsInSlot.Peek();
                    }


                    if (currentItemInSlot.ItemType == ItemType.Rifle)
                        ResetInfo();
                    else
                        StacSizeInSlot--;
                }

                yield return null;
            }
        }
    }
}

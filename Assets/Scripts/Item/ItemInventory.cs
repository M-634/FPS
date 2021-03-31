using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Musashi
{
    public class ItemInventory : SingletonMonoBehaviour<ItemInventory>
    {
        [SerializeField] ItemDataBase itemDataBase;
        [SerializeField] CanvasGroup inventoryCanvasGroup;
        [SerializeField] SlotBase[] itemSlots;


        bool isOpenInventory = true;

        public bool IsSlotSelected { get => SelectedSlot != null; }
        public SlotBase SelectedSlot { get; private set; }


        private void Start()
        {
            OpenAndCloseInventory();
        }

        private void Update()
        {
            if (PlayerInputManager.InventoryAction())
                OpenAndCloseInventory();
        }

        public void SetKeyCode()
        {
            for (int i = 0; i < itemSlots.Length; i++)
            {
                if (i == 0)
                    itemSlots[i].KeyCode = "Q";
                else
                    itemSlots[i].KeyCode = i.ToString();
            }
        }

        /// <summary>
        /// 取得したアイテムがデータベースに存在するか確認する。
        /// データベースにあったら、 アイテムを拾うことができるか判定する
        /// </summary>
        /// <param name="getItem"></param>
        /// <returns></returns>
        public bool CanGetItem(BaseItem getItem)
        {
            foreach (var itemData in itemDataBase.ItemDataList)
            {
                if (itemData.KindOfItem == getItem.kindOfItem)
                {
                    return SearchItemSlot(itemData);
                }
            }
            Debug.LogWarning("データベースに該当するアイテムがありません");
            return false;
        }


        /// <summary>
        /// 既にスロット内にあるアイテムかどうか調べる。
        /// ないなら左側から順番に埋めていく
        /// </summary>
        /// <returns></returns>
        private bool SearchItemSlot(ItemData getItemData)
        {
            for (int i = 0; i < itemSlots.Length; i++)
            {
                //スタック数が満タンの時は次のスロットを調べる
                if (itemSlots[i].IsFilled)
                {
                    continue;
                }

                //同一アイテムがスロット内にあり,かつスタック数が満タンではない時はスタック数を足していく
                if (!itemSlots[i].IsEmpty && itemSlots[i].CurrentItemData.KindOfItem == getItemData.KindOfItem && !itemSlots[i].IsFilled)
                {
                    itemSlots[i].AddItemInSlot();
                    return true;
                }

                //スロットが空なら、アイテムデータをセットする
                if (itemSlots[i].IsEmpty)
                {
                    itemSlots[i].SetInfo(getItemData);
                    return true;
                }
            }
            return false;//全てのスロットが埋まっている
        }

        public void OpenAndCloseInventory()
        {
            if (isOpenInventory)
            {
                inventoryCanvasGroup.HideUIWithCanvasGroup();
                GameManager.Instance.LockCusor();
                EventManeger.Instance.Excute(EventType.CloseInventory);
            }
            else
            {
                inventoryCanvasGroup.ShowUIWithCanvasGroup();
                GameManager.Instance.UnlockCusor();
                EventManeger.Instance.Excute(EventType.OpenInventory);
            }
            isOpenInventory = !isOpenInventory;
        }
    }
}

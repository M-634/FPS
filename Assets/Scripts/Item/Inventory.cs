using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Musashi
{
    public class Inventory : SingletonMonoBehaviour<Inventory>
    {
        [SerializeField] ItemDataBase itemDataBase;
        [SerializeField] CanvasGroup inventoryCanvasGroup;
        [SerializeField] Slot[] weaponSlots;
        [SerializeField] Slot[] itemSlots;
        public Slot[] Slots { get => itemSlots; }

        public bool IsSlotSelected { get => SelectedSlot != null; }
        public Slot SelectedSlot { get; private set; }

        bool isOpenInventory = true;
     
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
                if (itemData.KindOfItem == getItem.kindOfitem)
                {
                    return SearchSlot(itemData);
                }
            }
            Debug.LogWarning("データベースに該当するアイテムがありません");
            return false;
        }

        /// <summary>
        /// アイテムが武器なら武器スロットへ、アイテムならアイテムスロットへ入れる。
        /// 既にスロット内にあるアイテムかどうか調べる。
        /// ないなら左側から順番に埋めていく
        /// </summary>
        /// <returns></returns>
        private bool SearchSlot(ItemData getItemData)
        {
            //武器
            if (getItemData.IsWeapon)
            {
                for (int i = 0; i < weaponSlots.Length; i++)
                {
                    //スタック内に武器があるなら次へ
                    if (weaponSlots[i].IsFilled)
                    {
                        continue;
                    }
                    //スロットが空なら、武器データをセットする
                    if (weaponSlots[i].IsEmpty)
                    {
                        weaponSlots[i].SetInfo(getItemData);
                        return true;
                    }
                    return false;//全てのスロットが埋まっている
                }
            }

            //アイテム
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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

//memo :5/23 アイテムはとりあえず、回復キットと弾薬の２種類のみとする。
//To Do :AmmoControllerとこのクラスを結合させる
namespace Musashi
{
    public class ItemInventory : MonoBehaviour
    {
        public Action OpenInventory;
        public Action CloseInventory;

        [SerializeField] ItemDataBase itemDataBase;
        [SerializeField] CanvasGroup inventoryCanvasGroup;
        [SerializeField] ItemSlot[] itemSlots;


        [SerializeField] AmmoControlInInventory ammoControl;
        [SerializeField] TextMeshProUGUI ammoStackNumberText;
        [SerializeField] TextMeshProUGUI healItemStackNumberText;

        private int healItemNumber;
        Queue<HealItem> stockHealItems;

        bool isOpenInventory = false;

        public bool IsSlotSelected { get => SelectedSlot != null; }
        public SlotBase SelectedSlot { get; private set; }

     
        private void Start()
        {
            stockHealItems = new Queue<HealItem>();
           // inventoryCanvasGroup.HideUIWithCanvasGroup();
            //if (inputProvider)
            //{
            //    foreach (var slot in itemSlots)
            //    {
            //        slot.SetInput(inputProvider);
            //    }
            //}
        }

        //private void Update()
        //{
        //    if (inputProvider.Inventory)
        //    {
        //        OpenAndCloseInventory();
        //    }
        //}

        /// <summary>
        /// 取得したアイテムがデータベースに存在するか確認する。
        /// データベースにあったら、 アイテムを拾うことができるか判定する
        /// </summary>
        /// <returns>true: アイテム取得(オブジェットが消える). 
        ///false:アイテム取得失敗か、インベントリ内が最大である (オブジェットが消えない) </returns>
        public bool CanGetItem(Item getItem)
        {
            foreach (var itemData in itemDataBase.ItemDataList)
            {
                if (itemData.itemName == getItem.ItemName)
                {
                    // return SearchItemSlot(getItem);
                    return SearchInventory(getItem);
                }
            }
            Debug.LogWarning("データベースに該当するアイテムがありません");
            return false;
        }

        /// <summary>
        /// 取得アイテムごとにインベントリ内を調べる
        /// </summary>
        private bool SearchInventory(Item getItem)
        {
            if (getItem.ItemType == ItemType.HealthKit)
            {
                if(CanStackItem(getItem, ref healItemNumber, healItemStackNumberText))
                {
                    stockHealItems.Enqueue(getItem as HealItem);
                    return true;
                }
            }
            if (getItem.ItemType == ItemType.AmmoBox)
            {
                return CanStackItem(getItem, ref ammoControl.sumNumberOfAmmoInInventory, ammoStackNumberText);
            } 
            return false;
        }


        /// <summary>
        /// 取得アイテムが最大取得数を超えていないかチェックし、アイテムごとのスタック数を更新する
        /// </summary>
        private bool CanStackItem(Item getItem, ref int stackNumInInventory, TextMeshProUGUI displayText)
        {
            bool res = true;
            int temp = stackNumInInventory + getItem.StacSize;
            if (temp > getItem.MaxStacSize)
            {
                getItem.StacSize = temp - getItem.MaxStacSize;
                temp = getItem.MaxStacSize;
                res = false;
            }
            stackNumInInventory = temp;

            if (displayText)
            {
                displayText.text = stackNumInInventory.ToString();
            }
            return res;
        }

             
        public void UseHealthItem()
        {
        
            if (stockHealItems.Count == 0) return;

            var item = stockHealItems.Peek();

            item.CanHealPlayer();
            Debug.Log(item.name);

            //if (canUseItme)
            //{
            //    healItemNumber--;
            //    item.StacSize--;
            //    if (item.StacSize == 0)
            //    {
            //        item = stockHealItems.Dequeue();
            //        Destroy(item.gameObject);
            //    }
            //}
        }

        ///// <summary>
        ///// 既にスロット内にアイテムが存在するか調べる。
        ///// ないなら左側から順番に埋めていく。
        ///// </summary>
        ///// <returns></returns>
        //private bool SearchItemSlot(Item getItem)
        //{
        //    for (int i = 0; i < itemSlots.Length; i++)
        //    {
        //        //スタック数が満タンの時は次のスロットを調べる
        //        if (itemSlots[i].IsFilled)
        //        {
        //            continue;
        //        }

        //        //同一アイテムがスロット内にあり,かつスタック数が満タンではない時はスタック数を足していく
        //        if (!itemSlots[i].IsEmpty && itemSlots[i].currentItemInSlot.ItemName == getItem.ItemName && !itemSlots[i].IsFilled)
        //        {
        //            itemSlots[i].AddItemInSlot(getItem);
        //            if (getItem.ItemType == ItemType.AmmoBox)
        //            {
        //                ammoControl.AddSumOfAmmo(getItem.StacSize);
        //            }
        //            return true;
        //        }

        //        //スロットが空なら、アイテムデータをセットする
        //        if (itemSlots[i].IsEmpty)
        //        {
        //            itemSlots[i].SetInfo(getItem);
        //            if (getItem.ItemType == ItemType.AmmoBox)
        //            {
        //                ammoControl.AddSumOfAmmo(getItem.StacSize);
        //            }
        //            return true;
        //        }
        //    }
        //    return false;//全てのスロットが埋まっている
        //}

        //public void OpenAndCloseInventory()
        //{
        //    if (GameManager.Instance.HaveShowConfigure) return;

        //    if (isOpenInventory)
        //    {
        //        if (CloseInventory != null)
        //        {
        //            inventoryCanvasGroup.HideUIWithCanvasGroup();
        //            GameManager.Instance.LockCusor();
        //            CloseInventory.Invoke();
        //        }
        //    }
        //    else
        //    {
        //        if (OpenInventory != null)
        //        {
        //            inventoryCanvasGroup.ShowUIWithCanvasGroup();
        //            GameManager.Instance.UnlockCusor();
        //            OpenInventory.Invoke();
        //        }
        //    }
        //    isOpenInventory = !isOpenInventory;
        //}
    }
}

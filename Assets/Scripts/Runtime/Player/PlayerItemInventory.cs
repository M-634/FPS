using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Musashi.Weapon;

//memo :5/23 アイテムはとりあえず、回復キットと弾薬の２種類のみとする。
namespace Musashi.Item
{
    /// <summary>
    /// ゲームシーンUIに表示しているアイテム(Ammo, HealKit, Weapon)を管理するクラス
    /// </summary>
    public class PlayerItemInventory : MonoBehaviour
    {
        [Header("item settings")]
        [SerializeField] ItemDataBase itemDataBase;

        [Header("Heal item")]
        [SerializeField] int maxHealKitNumberInInventory = 10;
        [SerializeField] TextMeshProUGUI healItemStackNumberText;
        Queue<HealItem> stockHealItems;
        private int healItemNumber;

        [Header("Ammo in inventory")]
        [SerializeField] bool testScene;
        [SerializeField] int limmitAmmoNumberInInventory = 999;

        public int SumNumberOfAmmoInInventory { get; set; }

        PlayerInputProvider input;

        private void Start()
        {
            if (testScene)
            {
                SumNumberOfAmmoInInventory = limmitAmmoNumberInInventory;
            }
            else
            {
                SumNumberOfAmmoInInventory = 0;
            }
        }

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
                if (CanStackItem(getItem, ref healItemNumber, healItemStackNumberText))
                {
                    stockHealItems.Enqueue(getItem as HealItem);
                    return true;
                }
            }
            //if (getItem.ItemType == ItemType.AmmoBox)
            //{
            //    return CanStackItem(getItem, ref sumNumberOfAmmoInInventory, ammoStackNumberInInventoryText);
            //}
            return false;
        }

        /// <summary>
        /// 取得アイテムが最大取得数を超えていないかチェックし、アイテムごとのスタック数を更新する
        /// </summary>
        private bool CanStackItem(Item getItem, ref int stackNumInInventory, TextMeshProUGUI displayText)
        {
            if (limmitAmmoNumberInInventory == getItem.MaxStacSize) return false;

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
                displayText.text = stackNumInInventory.ToString() + " / " + getItem.MaxStacSize.ToString();
            }
            return res;
        }

        public void UseHealthItem()
        {
            if (stockHealItems.Count == 0) return;

            var item = stockHealItems.Peek();
            var canUseItme = item.OnUseEvent.Invoke();

            if (canUseItme)
            {
                healItemNumber--;
                if (healItemStackNumberText)
                {
                    healItemStackNumberText.text = healItemNumber.ToString() + " / " + item.MaxStacSize.ToString();
                }

                item.StacSize--;
                if (item.StacSize == 0)
                {
                    item = stockHealItems.Dequeue();
                    Destroy(item.gameObject);
                }
            }
        }

    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Musashi.Item;

//memo :5/23 アイテムはとりあえず、回復キットと弾薬の２種類のみとする。
namespace Musashi.Player
{
    /// <summary>
    /// ゲームシーンUIに表示しているアイテム(Ammo, HealKit, Weapon)を管理するクラス
    /// </summary>
    public class PlayerItemInventory : MonoBehaviour
    {
        private class ItemInventoryTable
        {
            private readonly BaseItem item;
            private readonly PlayerItemInventory inventory;
            private int stackingNumber;
            public int CurrentStackSize
            {
                get => stackingNumber;
                set
                {
                    stackingNumber = value;
                    if (stackingNumber > item.GetMaxStacSize)
                    {
                        stackingNumber = item.GetMaxStacSize;
                    }

                    if (stackingNumber <= 0)
                    {
                        DeleteTable();
                        return;
                    }
                    Debug.Log(item.GetItemName + " : " + stackingNumber);
                }
            }
            public bool LimitStacSize => stackingNumber == item.GetMaxStacSize;
            public string GetItemGUID => item.GetItemGUID;
            public ItemInventoryTable(BaseItem item, PlayerItemInventory inventory)
            {
                this.item = item;
                this.inventory = inventory;
                CurrentStackSize += item.GetAddStacSize;
                Debug.Log("instance table");
                Debug.Log("tables count :" + inventory.itemTables);
            }
            public void Use(Slot slot = null)
            {
                if (item.UseItem())
                {
                    CurrentStackSize--;
                    if (slot)
                    {
                        slot.UpdateStackSizeText(CurrentStackSize);
                    }
                }
            }
            public void DeleteTable()
            {
                Destroy(item.gameObject);
                inventory.itemTables.Remove(this);
                Debug.Log("Destroy :" + item.GetItemName);
                Debug.Log("tables count :" + inventory.itemTables);
            }
        }

        public event Action ChangedAmmoInInventoryEvent;
        public const int LIMITITEMSTACKSIZE = 999;

        [SerializeField] bool testScene;
        [SerializeField] Slot currentHealItemSlot;//今のところ回復アイテムは一種類だけなので、スロットを最初からアタッチちする

        private readonly List<ItemInventoryTable> itemTables = new List<ItemInventoryTable>();

        private int sumAmmoInInventory;
        public int SumAmmoInInventory
        {
            get => sumAmmoInInventory;
            set
            {
                sumAmmoInInventory = value;
                if (ChangedAmmoInInventoryEvent != null)
                {
                    ChangedAmmoInInventoryEvent.Invoke();
                }
            }
        }

        PlayerInputProvider inputProvider;

        private void Start()
        {
            inputProvider = GetComponent<PlayerInputProvider>();
            inputProvider.PlayerInputActions.UseHealItem.performed += UseHealItem_performed;

            if (testScene)
            {
                SumAmmoInInventory = LIMITITEMSTACKSIZE;
            }
            else
            {
                SumAmmoInInventory = 0;
            }
        }

        private void UseHealItem_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            Debug.Log("can input");
            if (currentHealItemSlot)
            {
                var item = GetItemFromTables(currentHealItemSlot.GUID);
                item.Use(currentHealItemSlot);
            }
        }

        /// <summary>
        /// 引数に与えられたアイテムが、既にインベントリ内に存在するかどうか判定する関数
        /// </summary>
        private bool HasItem(string guid)
        {
            foreach (var item in itemTables)
            {
                if (item.GetItemGUID == guid)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 引数に与えられたアイテムから、それを含んだアイテムテーブルを返す関数
        /// </summary>
        private ItemInventoryTable GetItemFromTables(string guid)
        {
            foreach (var itemTable in itemTables)
            {
                if (itemTable.GetItemGUID == guid) return itemTable;
            }
            return null;
        }

        /// <summary>
        /// プレイヤーの任意のタイミングで使用できるアイテムを、プレイヤーインベントリ内に
        /// 追加できるか判定する関数
        /// </summary>
        public bool AddItem(BaseItem pickedItem)
        {
            //cheack if same item is in table or not.
            if (HasItem(pickedItem.GetItemGUID))
            {
                var itemTable = GetItemFromTables(pickedItem.GetItemGUID);
                if (itemTable.LimitStacSize)
                {
                    Debug.Log(pickedItem.GetItemName + ": limit stack size");
                    return false;
                }
                itemTable.CurrentStackSize += pickedItem.GetAddStacSize;
                currentHealItemSlot.UpdateStackSizeText(itemTable.CurrentStackSize);
                Destroy(pickedItem.gameObject);
                return true;
            }

            //create new table
            var newTable = new ItemInventoryTable(pickedItem, this);
            itemTables.Add(newTable);
            if (pickedItem.GetItemType == ItemType.HealthKit)
            {
                currentHealItemSlot.SetItemInfo(pickedItem.GetItemGUID, pickedItem.GetAddStacSize);
            }

            pickedItem.gameObject.SetActive(false);
            return true;
        }
    }
}

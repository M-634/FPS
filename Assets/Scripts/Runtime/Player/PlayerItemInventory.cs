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

                    if(stackingNumber <= 0)
                    {
                        DeleteTable();
                        return;
                    }
                    Debug.Log(item.GetItemName + " : " + stackingNumber);
                    //ui更新
                }
            }
            public bool LimitStacSize => stackingNumber == item.GetMaxStacSize;
            public string GetItemGUID => item.GetItemGUID;
            public ItemInventoryTable(BaseItem item,PlayerItemInventory inventory)
            {
                this.item = item;
                this.inventory = inventory;
                CurrentStackSize += item.GetAddStacSize;
                Debug.Log("instance table");
                Debug.Log("tables count :" + inventory.itemTables);
            }
            public void Use()
            {
                if (item.UseItem())
                {
                    CurrentStackSize--;
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// 引数に与えられたアイテムが、既にインベントリ内に存在するかどうか判定する関数
        /// </summary>
        private bool HasItem(BaseItem getItem)
        {
            Debug.Log("picked :" + getItem.gameObject);
            foreach (var item in itemTables)
            {
                Debug.Log("table :" + item.GetItemGUID);
                if (item.GetItemGUID == getItem.GetItemGUID)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 引数に与えられたアイテムから、それを含んだアイテムテーブルを返す関数
        /// </summary>
        private ItemInventoryTable GetItemFromTables(BaseItem getItem)
        {
            foreach (var itemTable in itemTables)
            {
                if (itemTable.GetItemGUID == getItem.GetItemGUID) return itemTable;
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
            if (HasItem(pickedItem))
            {
                var itemTable = GetItemFromTables(pickedItem);
                if (itemTable.LimitStacSize)
                {
                    Debug.Log(pickedItem.GetItemName + ": limit stack size");
                    return false;
                }
                itemTable.CurrentStackSize += pickedItem.GetAddStacSize;
                Destroy(pickedItem.gameObject);
                return true;
            }

            //create new table
            var newTable = new ItemInventoryTable(pickedItem,this);
            itemTables.Add(newTable);
            pickedItem.gameObject.SetActive(false);
            return true;
        }
        public void UseItem(BaseItem item)
        {
            if (HasItem(item))
            {
                var i = GetItemFromTables(item);
                i.Use();
            }
        }
    }
}

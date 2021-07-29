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
        /// <summary>
        /// インベントリ内のアイテム情報を一括にまとめた内部クラス
        /// </summary>
        public class ItemInventoryTable
        {
            private readonly PlayerItemInventory inventory;
            private int stackingNumber;
            public BaseItem GetBaseItem { get; private set; }
            public int CurrentStackSize
            {
                get => stackingNumber;
                set
                {
                    stackingNumber = value;
                    if (stackingNumber > GetBaseItem.GetMaxStacSize)
                    {
                        stackingNumber = GetBaseItem.GetMaxStacSize;
                    }

                    if (stackingNumber <= 0)
                    {
                        DeleteTable();
                        return;
                    }
                    Debug.Log(GetBaseItem.GetItemName + " : " + stackingNumber);
                }
            }
            public bool LimitStacSize => stackingNumber == GetBaseItem.GetMaxStacSize;
            public string GetItemGUID => GetBaseItem.GetItemGUID;
            public string GetItemNameInTabe => GetBaseItem.GetItemName;
            public ItemInventoryTable(BaseItem item, PlayerItemInventory inventory)
            {
                this.GetBaseItem = item;
                this.inventory = inventory;
                CurrentStackSize += item.GetAddStacSize;
                Debug.Log("instance table");
                Debug.Log("tables count :" + inventory.ItemTable);
            }
            public void Use(Slot slot = null)
            {
                if (GetBaseItem.UseItem())
                {
                    CurrentStackSize--;
                    if (slot)
                    {
                        slot.UpdateStackSizeText(CurrentStackSize);
                    }
                    Debug.Log($"{GetBaseItem.GetItemName}を使用しました");
                }
            }
            public void DeleteTable()
            {
                Destroy(GetBaseItem.gameObject);
                inventory.ItemTable.Remove(this);
                Debug.Log("Destroy :" + GetBaseItem.GetItemName);
                Debug.Log("tables count :" + inventory.ItemTable);
            }
        }

        [SerializeField] bool setLimitStackSizeAmmo;
        [SerializeField] Slot currentHealItemSlot;//今のところ回復アイテムは一種類だけなので、スロットを最初からアタッチちする

        public const int LIMITITEMSTACKSIZE = 999;
        public event Action ChangedAmmoInInventoryEvent;
        PlayerInputProvider inputProvider;
        public List<ItemInventoryTable> ItemTable { get; private set; }= new List<ItemInventoryTable>();

        private int sumAmmoInInventory;
        public int SumAmmoInInventory
        {
            get => sumAmmoInInventory;
            set
            {
                sumAmmoInInventory = value;
                if(sumAmmoInInventory < 0)
                {
                    sumAmmoInInventory = 0;
                }
                if (ChangedAmmoInInventoryEvent != null)
                {
                    ChangedAmmoInInventoryEvent.Invoke();
                }
            }
        }

        private void Start()
        {
            inputProvider = GetComponent<PlayerInputProvider>();
            inputProvider.PlayerInputActions.UseHealItem.performed += UseHealItem_performed;

            if (setLimitStackSizeAmmo)
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
            if (currentHealItemSlot)
            {
                var item = GetItemFromTables(currentHealItemSlot.GUID);
                if (item != null)
                {
                    item.Use(currentHealItemSlot);
                }
                else
                {
                    Debug.Log($"Playerはヒールアイテムをインベントリ内に所持していません。");
                }
            }
        }

        /// <summary>
        /// 引数に与えられたアイテムが、既にインベントリ内に存在するかどうか判定する関数
        /// </summary>
        private bool HasItem(string guid)
        {
            foreach (var item in ItemTable)
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
            foreach (var itemTable in ItemTable)
            {
                if (itemTable.GetItemGUID == guid) return itemTable;
            }
            return null;
        }

        /// <summary>
        /// プレイヤーの任意のタイミングで使用できるアイテムを、プレイヤーインベントリ内に
        /// 追加できるか判定する関数
        /// </summary>
        public bool AddItem(BaseItem pickedItem,int offset = 0)
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
                itemTable.CurrentStackSize += pickedItem.GetAddStacSize + offset;
                currentHealItemSlot.UpdateStackSizeText(itemTable.CurrentStackSize);
                Destroy(pickedItem.gameObject);
                return true;
            }

            //create new table
            var newTable = new ItemInventoryTable(pickedItem, this);
            ItemTable.Add(newTable);
            if (pickedItem.GetItemType == ItemType.HealthKit)
            {
                currentHealItemSlot.SetItemInfo(pickedItem.GetItemGUID, pickedItem.GetAddStacSize);
            }

            pickedItem.gameObject.SetActive(false);
            return true;
        }
        //public void AddItemFromSpwan(List<ItemInventoryTable> inventoryTable)
        //{
        //    foreach (var item in inventoryTable)
        //    {
        //        AddItem(item.GetBaseItem, item.CurrentStackSize - item.GetBaseItem.GetAddStacSize);
        //    }
        //}
    }
}

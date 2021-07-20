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
        public event Action ChangedAmmoInInventoryEvent;

        [Header("item settings")]
        [SerializeField] ItemDataBase itemDataBase;

        [Header("Ammo in inventory")]
        [SerializeField] bool testScene;

        Dictionary<BaseItem, int> stackingItemTable= new Dictionary<BaseItem, int>();

        public const int LIMITITEMSTACKSIZE = 999; 
        private int sumAmmoInInventory;
        public int SumAmmoInInventory 
        {
            get => sumAmmoInInventory ;
            set 
            {
                sumAmmoInInventory = value;
                if (ChangedAmmoInInventoryEvent != null)
                {
                    ChangedAmmoInInventoryEvent.Invoke();
                }
            }
        }
  
        private void Start()
        {
            if (testScene)
            {
                SumAmmoInInventory = LIMITITEMSTACKSIZE;
            }
            else
            {
                SumAmmoInInventory = 0;
            }
        }

        public bool AddItem(BaseItem pickedItem)
        {
            //cheack if same item is in table or not.
            if (stackingItemTable.ContainsKey(pickedItem))
            {
                if (stackingItemTable[pickedItem] >= pickedItem.GetMaxStacSize) return false;


            }
            
            return true;      
        }

    }
}

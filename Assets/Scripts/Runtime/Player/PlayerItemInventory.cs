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
        [Header("item settings")]
        [SerializeField] ItemDataBase itemDataBase;

        [Header("Heal item")]
        [SerializeField] int maxHealKitNumberInInventory = 10;
        [SerializeField] TextMeshProUGUI healItemStackNumberText;
        readonly Queue<HealItem> stockHealItems;

        private int healItemNumber;

        [Header("Ammo in inventory")]
        [SerializeField] bool testScene;
        [SerializeField] int limmitAmmoNumberInInventory = 999;

        PlayerWeaponManager weaponManager;
        Dictionary<ItemSettingSOData, int> currentItemStacks = new Dictionary<ItemSettingSOData, int>();

        public int SumNumberOfAmmoInInventory { get; set; }
      
        private void Start()
        {
            weaponManager = GetComponent<PlayerWeaponManager>();
            if (testScene)
            {
                SumNumberOfAmmoInInventory = limmitAmmoNumberInInventory;
            }
            else
            {
                SumNumberOfAmmoInInventory = 0;
            }
 
        }

        public bool AddItem(BaseItem healItem)
        {
            return true;      
        }

    }
}

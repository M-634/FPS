using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//memo :5/23 アイテムはとりあえず、回復キットと弾薬の２種類のみとする。
namespace Musashi
{
    /// <summary>
    /// ゲームシーンUIに表示しているアイテム(Ammo, HealKit, Weapon)を管理するクラス
    /// </summary>
    public class ItemInventory : SingletonMonoBehaviour<ItemInventory>
    {
        [Header("item settings")]
        [SerializeField] ItemDataBase itemDataBase;

        [Header("Heal item")]
        [SerializeField] int maxHealKitNumberInInventory = 10;
        [SerializeField] TextMeshProUGUI healItemStackNumberText;
        Queue<HealItem> stockHealItems;
        private int healItemNumber;

        [Header("Weapons")]
        [Header("0:hundGun 1:shutGun 2: Rifle")]
        [SerializeField] Slot[] weaponSlots;
        [Header("0:hundGun 1:shutGun 2: Rifle")]
        [SerializeField] WeaponActiveControl[] weaponActive;

        [Header("Equipmet weapon info")]
        [SerializeField] GameObject equipmentWeaponInfo;
        [SerializeField] Image ammoCounterSllider;
        [SerializeField] Image equipmentWeaponIcon;
        [SerializeField] TextMeshProUGUI ammoCounterText;

        int currentWeaponEquipmentIndex = -1;

        [Header("Ammo in inventory")]
        [SerializeField] bool testScene;
        [SerializeField] int maxAmmoInInventory = 999;
        [SerializeField] TextMeshProUGUI ammoStackNumberInInventoryText;
        int sumNumberOfAmmoInInventory;
        public int SumNumberOfAmmoInInventory
        {
            get => sumNumberOfAmmoInInventory;
            private set
            {
                sumNumberOfAmmoInInventory = value;
                if (ammoStackNumberInInventoryText)
                {
                    ammoStackNumberInInventoryText.text = sumNumberOfAmmoInInventory.ToString() + "/" + maxAmmoInInventory.ToString();
                }
            }
        }

        InputProvider input;

        private void Start()
        {
            stockHealItems = new Queue<HealItem>();
            input = GetComponentInParent<InputProvider>();

            if (testScene)
            {
#if UNITY_EDITOR
                SumNumberOfAmmoInInventory = maxAmmoInInventory;
#endif
            }
            else
            {
                SumNumberOfAmmoInInventory = 0;
            }

            for (int i = 0; i < weaponSlots.Length; i++)
            {
                weaponSlots[i].StackSize.text = weaponActive[i].Control.CurrentAmmo.ToString() + " / " + weaponActive[i].Control.MaxAmmo.ToString();
            }

            if (equipmentWeaponInfo.activeSelf)
            {
                equipmentWeaponInfo.SetActive(false);
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
            if (getItem.ItemType == ItemType.AmmoBox)
            {
                return CanStackItem(getItem, ref sumNumberOfAmmoInInventory, ammoStackNumberInInventoryText);
            }
            return false;
        }

        /// <summary>
        /// 取得アイテムが最大取得数を超えていないかチェックし、アイテムごとのスタック数を更新する
        /// </summary>
        private bool CanStackItem(Item getItem, ref int stackNumInInventory, TextMeshProUGUI displayText)
        {
            if (maxAmmoInInventory == getItem.MaxStacSize) return false;

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

        private void Update()
        {
            if (input.UseHealItem)
            {
                UseHealthItem();
            }

            ChangeWeapon(input.SwichWeaponID);
        }

        private void ChangeWeapon(int index)
        {
            if (index == -1) return;

            //remove equipment 
            if (currentWeaponEquipmentIndex != -1)
            {
                weaponActive[currentWeaponEquipmentIndex].SetActive(false);
                weaponSlots[currentWeaponEquipmentIndex].MissingSelection();
            }
            currentWeaponEquipmentIndex = index;
            equipmentWeaponInfo.SetActive(true);

            weaponActive[currentWeaponEquipmentIndex].SetActive(true);
            weaponSlots[currentWeaponEquipmentIndex].OnSelected();
            equipmentWeaponIcon.sprite = weaponSlots[currentWeaponEquipmentIndex].Icon.sprite;
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

        #region Ammo Management
        /// <summary>
        /// リロードできるかどうか判定する関数
        /// </summary>
        /// <param name="maxAmmo">各銃の最大装填数</param>
        /// <param name="currentAmmo">現在の弾の装填数</param>
        public bool CanReloadAmmo(int maxAmmo, int currentAmmo)
        {
            int diff = maxAmmo - currentAmmo;
            if (diff <= 0) return false;

            if (SumNumberOfAmmoInInventory - diff >= 0)
            {
                return true;
            }

            if (SumNumberOfAmmoInInventory > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 実際にリロード出来る弾数
        /// </summary>
        /// <returns></returns>
        public int ReloadAmmoNumber(int maxAmmo, int currentAmmo)
        {
            int diff = maxAmmo - currentAmmo;

            if (SumNumberOfAmmoInInventory - diff >= 0)
            {
                SumNumberOfAmmoInInventory -= diff;
                return maxAmmo;
            }

            int temp = currentAmmo + SumNumberOfAmmoInInventory;
            SumNumberOfAmmoInInventory = 0;
            return temp;
        }

        /// <summary>
        /// ショットガンのリロード。一発ずつ行う
        /// </summary>
        /// <returns></returns>
        public int ReloadAmmNumber()
        {
            SumNumberOfAmmoInInventory--;
            if (SumNumberOfAmmoInInventory < 0)
            {
                SumNumberOfAmmoInInventory = 0;
            }
            return 1;
        }

        /// <summary>
        /// 装備中の残弾数を表示する
        /// </summary>
        /// <param name="currentAmmo"></param>
        /// <param name="maxAmmo"></param>
        public void DisplayEquipmentWeaponInfo(int currentAmmo, int maxAmmo)
        {
            equipmentWeaponInfo.SetActive(true);
            ammoCounterText.text = currentAmmo.ToString();
            ammoCounterSllider.fillAmount = (float)currentAmmo / maxAmmo;
            if (currentWeaponEquipmentIndex != -1)
            {
                weaponSlots[currentWeaponEquipmentIndex].StackSize.text = currentAmmo.ToString() + " / " + maxAmmo.ToString();
            }
        }
        #endregion

    }
}

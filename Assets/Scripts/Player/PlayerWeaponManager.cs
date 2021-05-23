using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Musashi
{
    /// <summary>
    /// プレイヤーが武器を装備したり、切り替えることを管理するクラス
    /// </summary>
    public class PlayerWeaponManager : MonoBehaviour
    {
        [SerializeField] WeaponDataBase weaponDataBase;
        [SerializeField] WeaponActiveControl[] activeControls;
        [SerializeField] WeaponSlot[] weaponSlots;
        [SerializeField] CurrentWeaponAmmoCounter currentWeaponAmmoCounter;

        bool canInputAction = true;

        InputProvider inputProvider; 
        WeaponActiveControl currentActiveWeapon;
        WeaponSlot currentActiveSlot;

        public bool HaveWeapon => currentActiveWeapon != null && currentActiveSlot != null;

        private void Start()
        {
            inputProvider = GetComponentInParent<InputProvider>();
    
            foreach (var item in activeControls)
            {
                item.SetActive(false);
            }

            //for (int i = 0; i < weaponSlots.Length; i++)
            //{
            //    weaponSlots[i].SetInput(inputProvider);
            //    weaponSlots[i].SetWeaponSlot(i, this);
            //}
        }


        private void Update()
        {
            if (!canInputAction) return;

            var i = inputProvider.SwichWeaponID;
            if (i == -1) return;
            if (HaveWeapon && i == currentActiveSlot.slotNumber) return;
            EquipWeapon(i);
        }

        /// <summary>
        /// 武器のデータベースに存在するか確認する関数。
        /// </summary>
        /// <param name="getWeapon"></param>
        /// <returns></returns>
        public bool CanGetItem(Item getWeapon)
        {
            foreach (var weaponData in weaponDataBase.WeaponDataList)
            {
                if (getWeapon.ItemName == weaponData.weaponName)
                {
                    return CanEquipWeapon(getWeapon);
                }
            }
            Debug.LogWarning("武器データに存在しないため拾えません");
            return false;
        }

        /// <summary>
        /// 拾える武器が装備することができるか確認する関数
        /// </summary>
        /// <param name="weaponData"></param>
        /// <returns></returns>
        public bool CanEquipWeapon(Item getWeapon)
        {
            //スロットに空きがあるか調べる
            for (int i = 0; i < weaponSlots.Length; i++)
            {
                if (weaponSlots[i].IsEmpty)
                {
                    weaponSlots[i].SetInfo(getWeapon);
                    EquipWeapon(i);
                    return true;
                }
            }


            //装備中の武器と拾った武器を交換する
            if (HaveWeapon)
            {
                int i = currentActiveSlot.slotNumber;
                currentActiveWeapon.SetActive(false);
                //currentActiveSlot.DropItem();

                weaponSlots[i].SetInfo(getWeapon);
                EquipWeapon(i);
                return true;
            }

            //スロットに空きがなく、交換も出来ないので装備出来ない⇒拾えない
            return false;
        }

        /// <summary>
        /// スロットのインデックスを指定して、スロット内の武器データとactiveWeaponsの武器データを照合し、武器を装備する
        /// </summary>
        /// <param name="index">スロットのインデックス</param>
        public  void EquipWeapon(int index)
        {
            PutAwayCurrentWeapon();
            for (int i = 0; i < activeControls.Length; i++)
            {
                if (weaponSlots[index].currentItemInSlot == null) continue;

                if(weaponSlots[index].currentItemInSlot.ItemName == activeControls[i].Control.weaponName)
                {
                    activeControls[i].SetActive(true);
                    currentActiveWeapon = activeControls[i];
                    currentActiveSlot = weaponSlots[index];

                    currentActiveWeapon.Control.CurrentAmmo = weaponSlots[index].currentItemInSlot.StacSize;
                    currentActiveSlot.StacSizeInSlot = weaponSlots[index].currentItemInSlot.StacSize;

                    currentWeaponAmmoCounter.UIContents.SetActive(true);
                    currentWeaponAmmoCounter.WeaponIcon.sprite = weaponSlots[index].currentItemInSlot.Icon;
                    return;
                }
            }
        }

        /// <summary>
        /// 装備中の武器と、スロットをはずす。
        /// </summary>
        public void PutAwayCurrentWeapon()
        {
            if (!HaveWeapon) return;

            currentActiveSlot.currentItemInSlot.StacSize = currentActiveWeapon.Control.CurrentAmmo;
       
            currentActiveWeapon.Control.CancelAnimation();
            currentActiveWeapon.SetActive(false);
            currentActiveWeapon = null;
            currentActiveSlot = null;

            currentWeaponAmmoCounter.Init();
            currentWeaponAmmoCounter.UIContents.SetActive(false);
        }

        /// <summary>
        /// インベントリの開閉イベントの通知を受ける関数
        /// </summary>
        /// <param name="value">false : Open Inventory</param>
        private void ReciveInventoryEvent(bool value)
        {
            canInputAction = value;

            if (!value && HaveWeapon)
            {
                currentActiveSlot.StacSizeInSlot = currentActiveWeapon.Control.CurrentAmmo;
            }

            if (HaveWeapon)
            {
                currentActiveWeapon.SetActive(value);
            }
        }


        //PlayerEventManager playerEvent;
        ItemInventory inventory;
        private void OnEnable()
        {
            //playerEvent = GetComponent<PlayerEventManager>();
            //if (playerEvent)
            //{
            //    playerEvent.Subscribe(PlayerEventType.OpenInventory, () => ReciveInventoryEvent(false));
            //    playerEvent.Subscribe(PlayerEventType.CloseInventory, () => ReciveInventoryEvent(true));
            //}

            inventory = GetComponentInChildren<ItemInventory>();

            if (inventory)
            {
                inventory.OpenInventory += () => ReciveInventoryEvent(false);
                inventory.CloseInventory += () => ReciveInventoryEvent(true);
            }
        }

        private void OnDisable()
        {
            //if (playerEvent)
            //{
            //    playerEvent.UnSubscribe(PlayerEventType.OpenInventory, () => ReciveInventoryEvent(false));
            //    playerEvent.UnSubscribe(PlayerEventType.CloseInventory, () => ReciveInventoryEvent(true));
            //}

            if (inventory)
            {
                inventory.OpenInventory -= () => ReciveInventoryEvent(false);
                inventory.CloseInventory -= () => ReciveInventoryEvent(true);
            }
        }
    }
}

﻿using UnityEngine;

namespace Musashi
{
    /// <summary>
    /// プレイヤーが武器を装備したり、切り替えることを管理するクラス
    /// </summary>
    public class PlayerWeaponManager : MonoBehaviour
    {
        [SerializeField] WeaponDataBase weaponDataBase;
        /// <summary>playerの子どもに存在する武器。使用時にアクティブを切り替える</summary>
        [SerializeField] WeaponActiveControl[] activeControls;
        [SerializeField] WeaponSlot[] weaponSlots;

        bool canInputAction = true;

        PlayerInputManager playerInput;

        //cheack 
        public WeaponActiveControl currentActiveWeapon;
        public WeaponSlot currentActiveSlot;
        public bool HaveWeapon => currentActiveWeapon != null && currentActiveSlot != null;

        private void Start()
        {
            playerInput = GetComponent<PlayerInputManager>();
    
            foreach (var item in activeControls)
            {
                item.SetActive(false);
            }

            for (int i = 0; i < weaponSlots.Length; i++)
            {
                weaponSlots[i].SetInput(playerInput);
                weaponSlots[i].SetWeaponSlot(i, this);
            }
        }


        private void Update()
        {
            if (!canInputAction) return;

            var i = playerInput.SwichWeaponID;
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
                currentActiveSlot.DropItem();
                currentActiveWeapon.SetActive(false);

                int i = currentActiveSlot.slotNumber;
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
                    activeControls[i].Control.CurrentAmmo = weaponSlots[index].currentItemInSlot.StacSize;
          
                    currentActiveWeapon = activeControls[i];
                    currentActiveSlot = weaponSlots[index];
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
        }

        private void ReciveInventoryEvent(bool value)
        {
            canInputAction = value;
            if (HaveWeapon)
            {
                currentActiveWeapon.SetActive(value);
            }
        }


        PlayerEventManager playerEvent;
        private void OnEnable()
        {
            playerEvent = GetComponent<PlayerEventManager>();
            if (playerEvent)
            {
                playerEvent.Subscribe(PlayerEventType.OpenInventory, () => ReciveInventoryEvent(false));
                playerEvent.Subscribe(PlayerEventType.CloseInventory, () => ReciveInventoryEvent(true));
            }
        }

        private void OnDisable()
        {
            if (playerEvent)
            {
                playerEvent.UnSubscribe(PlayerEventType.OpenInventory, () => ReciveInventoryEvent(false));
                playerEvent.UnSubscribe(PlayerEventType.CloseInventory, () => ReciveInventoryEvent(true));
            }
        }
    }
}

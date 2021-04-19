using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace Musashi
{
    public class WeaponSlot : SlotBase
    {
        int slotNumber;
        PlayerWeaponManager playerWeaponManager;
        private WeaponData currentWeaponData;
        public WeaponData CurrentWeaponData => currentWeaponData;
        public override bool IsFilled => IsEmpty == false;

        public void SetWeaponSlot(int index,PlayerWeaponManager playerWeaponManager)
        {
            slotNumber = index;
            this.playerWeaponManager = playerWeaponManager;
        }

        public override void SetInfo<T>(T getData)
        {
            currentWeaponData = getData as WeaponData;
            icon.sprite = currentWeaponData.Icon;
            IsEmpty = false;
        }

        /// <summary>
        /// スロットをクリックして武器を装備する
        /// </summary>
        public override void UseObject(GameObject player)
        {
            if (IsEmpty) return;
            playerWeaponManager.EquipWeapon(slotNumber);
            player.GetComponent<PlayerItemInventory>().OpenAndCloseInventory();
        }

        public override void DropObject()
        {
            if (IsEmpty) return;
            playerWeaponManager.PutAwayWeapon();
            var go = Instantiate(CurrentWeaponData.PickUpWeaonPrefab, playerCamera.position + playerCamera.forward * 2f, Quaternion.Euler(0,0,90));
            go.Drop(playerCamera);
            ResetInfo();
        }

        public override void ResetInfo()
        {
            currentWeaponData = null;
            icon.sprite = null;
            IsEmpty = true;
        }
    }
}

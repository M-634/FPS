using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Musashi
{
    /// <summary>
    /// プレイヤーが武器やアイテムを扱う時に制御するクラス
    /// </summary>
    public class PlayerInteractive : MonoBehaviour
    {
        /// <summary>playerの子どもに存在する武器。使用時にアクティブを切り替える</summary>
        [SerializeField] BaseWeapon[] activeWeapons;
        [SerializeField] WeaponSlot[] weaponSlots;
        [SerializeField] LayerMask pickUpLayer;
        [SerializeField] GameObject interactiveMessage;
        [SerializeField] float distance = 10f;

        public BaseWeapon CurrentHaveWeapon { get; private set; }
        int currentWeaponIndex = -1;
        RaycastHit hit;

        private void Update()
        {
            if (CheakPickUpObj() && PlayerInputManager.InteractiveAction())
            {
                if (hit.collider.TryGetComponent(out IPickUpObjectable pickUpObjectable))
                {
                    pickUpObjectable.OnPicked();
                }
            }
        }

        private bool CheakPickUpObj()
        {
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, distance, pickUpLayer))
            {
                InteractiveMessage.ShowInteractiveMessage(InteractiveMessage.InteractiveText);
                return true;
            }
            InteractiveMessage.CloseMessage();
            return false;
        }

        public void PickUpWeapon()
        {

        }

        public void EquipWeapon()
        {

        }
    }
}

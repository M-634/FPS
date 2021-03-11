using UnityEngine;

namespace Musashi
{
    /// <summary>
    /// プレイヤーが武器やアイテムを扱う時に制御するクラス
    /// </summary>
    public class PlayerInteractive : MonoBehaviour
    {
        [SerializeField] Transform equipPosition;
        [SerializeField] BaseWeaponController[] equipWeapons;
        [SerializeField] LayerMask pickUpLayer;
        [SerializeField] GameObject interactiveMessage;
        [SerializeField] float distance = 10f;

        public BaseWeaponController CurrentHaveWeapon { get; private set; }
        int currentWeaponIndex = -1;
        PlayerWeaponController wp;
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

            if (PlayerInputManager.Use())
            {
                //スロットが選択されているかどうか判定
                if (Inventory.Instance.IsSlotSelected)
                {
                    //アイテムなら消費し、武器なら攻撃する
                    Inventory.Instance.SelectedSlot.UseItemInSlot();
                }
            }
        }

        private bool CheakPickUpObj()
        {
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, distance, pickUpLayer))
            {
                //if (hit.collider.TryGetComponent(out PlayerWeaponController playerWeapon))
                //{
                //    wp = playerWeapon;
                //    return true;
                //}

                //interactiveMessage.SetActive(true);
                InteractiveMessage.ShowInteractiveMessage(InteractiveMessage.InteractiveText);
                return true;
            }
            //interactiveMessage.SetActive(false);
            InteractiveMessage.CloseMessage();
            return false;
        }

        public void UseWeapon()
        {
            CurrentHaveWeapon.TryShot();
        }

        public void EquipmentWeapon(KindOfItem kindOfItem)
        {
            for (int i = 0; i < equipWeapons.Length; i++)
            {
                if (equipWeapons[i].data.KindOfItem == kindOfItem)
                {
                    equipWeapons[i].gameObject.SetActive(true);
                    CurrentHaveWeapon = equipWeapons[i];
                    currentWeaponIndex = i;
                }
            }
        }

        public void RemoveEquipment()
        {
            //現在装備していれば装備を外す
            if (currentWeaponIndex != -1)
            {
                equipWeapons[currentWeaponIndex].gameObject.SetActive(false);
            }
        }
        public void PickUp()
        {
            CurrentHaveWeapon = wp;
            CurrentHaveWeapon.transform.position = equipPosition.position;
            CurrentHaveWeapon.transform.parent = equipPosition;
            CurrentHaveWeapon.transform.localEulerAngles = new Vector3(0f, 0, 0);
            CurrentHaveWeapon.GetComponent<Rigidbody>().isKinematic = true;
        }

        public void Drop()
        {
            CurrentHaveWeapon.transform.parent = null;
            CurrentHaveWeapon.GetComponent<Rigidbody>().isKinematic = false;
            CurrentHaveWeapon = null;
        }
    }
}

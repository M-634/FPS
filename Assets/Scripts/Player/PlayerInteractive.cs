using UnityEngine;

namespace Musashi
{
    /// <summary>
    /// プレイヤーが武器やアイテムを扱う時に制御するクラス
    /// </summary>
    public class PlayerInteractive : MonoBehaviour
    {
        [SerializeField] Transform equipPosition;
        [SerializeField] LayerMask pickUpLayer;
        [SerializeField] GameObject interactiveMessage;
        [SerializeField] float distance = 10f;

        public PlayerWeaponController CurrentHaveWeapon { get;private set; }
        PlayerWeaponController wp;
        RaycastHit hit;


        private void Update()
        {
            if (CheakPickUpObj() && PlayerInputManager.PickUp())
            {
                //if (CurrentHaveWeapon)
                //    Drop();
                //PickUp();
                if (hit.collider.TryGetComponent(out IPickUpObjectable pickUpObjectable))
                {
                    pickUpObjectable.OnPicked();
                }
            }

            //if (PlayerInputManager.Drop() && CurrentHaveWeapon)
            //    Drop();

            //if (CurrentHaveWeapon)
            //     UseWeapon();
        }

        private bool CheakPickUpObj()
        {
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, distance,pickUpLayer))
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

        private void UseWeapon()
        {
            if (PlayerInputManager.Shot())
            {
                CurrentHaveWeapon.TryShot();
            }

            if (PlayerInputManager.CoolDownWeapon())
            {
                CurrentHaveWeapon.IsCoolTime = true;
            }
            CurrentHaveWeapon.UpdateAmmo();
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

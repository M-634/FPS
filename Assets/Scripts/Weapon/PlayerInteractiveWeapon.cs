using UnityEngine;

namespace Musashi
{
    /// <summary>
    /// プレイヤーが武器を扱う時に制御するクラス
    /// </summary>
    public class PlayerInteractiveWeapon : MonoBehaviour
    {
        [SerializeField] Transform equipPosition;
        [SerializeField] float distance = 10f;

        public PlayerWeaponController CurrentHaveWeapon { get;private set; }
        PlayerWeaponController wp;


        private void Update()
        {
            if (CheakWeapons() && PlayerInputManager.PickUp())
            {
                if (CurrentHaveWeapon)
                    Drop();
                PickUp();
            }

            if (PlayerInputManager.Drop() && CurrentHaveWeapon)
                Drop();

            if (!CurrentHaveWeapon) return;
            UseWeapon();
        }

        private bool CheakWeapons()
        {
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, distance))
            {
                if (hit.collider.TryGetComponent(out PlayerWeaponController playerWeapon))
                {
                    wp = playerWeapon;
                    return true;
                }
            }
            return false;
        }

        private void UseWeapon()
        {
            if (PlayerInputManager.Shot() && CurrentHaveWeapon)
            {
                CurrentHaveWeapon.TryShot();
            }

            if (PlayerInputManager.CoolDownWeapon() && CurrentHaveWeapon)
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

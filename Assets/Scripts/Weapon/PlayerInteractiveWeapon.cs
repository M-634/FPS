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

        public PlayerWeaponController currentHaveWeapon;
        PlayerWeaponController wp;


        private void Update()
        {
            if (CheakWeapons() && PlayerInputManager.PickUp())
            {
                if (currentHaveWeapon)
                    Drop();
                PickUp();
            }

            if (PlayerInputManager.Drop())
                Drop();

            if (!currentHaveWeapon) return;
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
            if (PlayerInputManager.Shot() && currentHaveWeapon)
            {
                currentHaveWeapon.TryShot();
            }

            if (PlayerInputManager.CoolDownWeapon() && currentHaveWeapon)
            {
                currentHaveWeapon.IsCoolTime = true;
            }
            currentHaveWeapon.UpdateAmmo();
        }

        public void PickUp()
        {
            currentHaveWeapon = wp;
            currentHaveWeapon.transform.position = equipPosition.position;
            currentHaveWeapon.transform.parent = equipPosition;
            currentHaveWeapon.transform.localEulerAngles = new Vector3(0f, 0, 0);
            currentHaveWeapon.GetComponent<Rigidbody>().isKinematic = true;
        }

        public void Drop()
        {
            currentHaveWeapon.transform.parent = null;
            currentHaveWeapon.GetComponent<Rigidbody>().isKinematic = false;
            currentHaveWeapon = null;
        }
    }
}

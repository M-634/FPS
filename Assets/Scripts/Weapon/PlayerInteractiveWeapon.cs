using UnityEngine;

namespace Musashi
{
    /// <summary>
    /// プレイヤーが武器を扱う時に制御するクラス
    /// </summary>
    public class PlayerInteractiveWeapon : MonoBehaviour
    {
        public BaseWeaponController currentHaveWeapon;


        private void Update()
        {
            if (PlayerInputManager.Shot())
            {
                currentHaveWeapon.TryShot();
            }

            if (PlayerInputManager.CoolDownWeapon())
            {
                currentHaveWeapon.IsCoolTime = true;
            }
            currentHaveWeapon.UpdateAmmo();
        }

        void ChangeWeapon(BaseWeaponController nextWeapon)
        {
            currentHaveWeapon = nextWeapon;
        }
    }
}

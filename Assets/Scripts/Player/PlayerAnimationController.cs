using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Musashi
{
    /// <summary>
    /// playerのFPS視点の手のアニメーションを制御するクラス
    /// </summary>
    public class PlayerAnimationController : MonoBehaviour
    {
        [SerializeField] Animator animator;
        PlayerInteractiveWeapon interactiveWeapon;
        bool isAiming;

        private void Start()
        {
            interactiveWeapon = GetComponent<PlayerInteractiveWeapon>();
        }

        public void MoveAnimation(float velocity)
        {
 
            if (interactiveWeapon.CurrentHaveWeapon)
            {
                if (PlayerInputManager.Shot())
                {
                    animator.Play("Fire");
                    isAiming = true;
                }

                if (PlayerInputManager.Aiming())
                {
                    animator.Play("Aiming");
                    isAiming = true;
                }

                if (PlayerInputManager.AimCancel())
                {
                    isAiming = false;
                }

                if (isAiming) return;

                if (velocity > 5f)
                    animator.Play("RunWtihGun");
                else
                    animator.Play("IdleWithGun");
            }
            else
            {
                if (velocity > 5f)
                    animator.Play("Run");
                else
                    animator.Play("Idle");
            }
        }

    }
}

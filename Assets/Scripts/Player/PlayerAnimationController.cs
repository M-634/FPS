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
        PlayerInteractive interactiveWeapon;
        PlayerCamaraControl camaraControl;
        bool isAiming;

        private void Start()
        {
            camaraControl = GetComponent<PlayerCamaraControl>();
            interactiveWeapon = GetComponent<PlayerInteractive>();
        }

        public void ShotingAnimation()
        {
            if (!interactiveWeapon.CurrentHaveWeapon) return;

            if (PlayerInputManager.Use())
            {
                animator.Play("Fire");
                isAiming = true;
            }

            if (PlayerInputManager.Aiming())
            {
                animator.Play("Aiming");
                camaraControl.SetAimingFov();
                isAiming = true;
            }

            if (PlayerInputManager.AimCancel())
            {
                camaraControl.SetNormalFov(true);
                isAiming = false;
            }
        }

        public void MoveAnimation(float velocity, PlayerMoveControl.State state)
        {
            ShotingAnimation();
            if (isAiming) return;

            if (state != PlayerMoveControl.State.Normal)
            {
                animator.Play("Idle");
                return;
            }

            if (interactiveWeapon.CurrentHaveWeapon)
            {

                if (velocity > 5)
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

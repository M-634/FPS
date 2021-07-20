using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Musashi
{
    /// <summary>
    /// プレイヤーの入力を管理するクラス
    /// プレイヤーオブジェット全ての親オブジェットにアタッチすること
    /// リファクタリングメモ：アクションで登録できるようにする
    /// </summary>
    public class PlayerInputProvider : MonoBehaviour
    {
        MyInputActions inputActions;
        MyInputActions.PlayerActions PlayerInputActions;

        private bool isGamepad;
        public bool IsGamepad
        {
            get
            {
                if (Gamepad.current == null) return false;
                if (Gamepad.current.wasUpdatedThisFrame) isGamepad = true;
                if (Mouse.current.wasUpdatedThisFrame) isGamepad = false;
                return isGamepad;
            }
        }

        public Vector3 GetMoveInput
        {
            get
            {
                if (GameManager.Instance.CanProcessInput)
                {
                    var move = new Vector3(PlayerInputActions.Move.ReadValue<Vector2>().x, 0f, PlayerInputActions.Move.ReadValue<Vector2>().y);
                    move = Vector3.ClampMagnitude(move, 1);
                    return move;
                }
                return Vector3.zero;
            }
        }
        public float GetLookInputsHorizontal => GameManager.Instance.CanProcessInput ? PlayerInputActions.Look.ReadValue<Vector2>().x : 0f;
        public float GetLookInputVertical
        {
            get
            {
                if (GameManager.Instance.CanProcessInput)
                {
                    var Y = PlayerInputActions.Look.ReadValue<Vector2>().y;
                    if (GameManager.Instance.Configure.DoInvert_Y == false)
                    {
                        Y *= -1;
                    }
                    return Y;
                }
                return 0;
            }
        }
        public bool Jump => GameManager.Instance.CanProcessInput && PlayerInputActions.Jump.triggered;
        public bool Fire => GameManager.Instance.CanProcessInput && PlayerInputActions.Fire.triggered;

        private bool heldFire;
        public bool HeldFire => GameManager.Instance.CanProcessInput && heldFire;
        public bool Reload => GameManager.Instance.CanProcessInput && PlayerInputActions.Reload.triggered;
        public bool Interactive => GameManager.Instance.CanProcessInput && PlayerInputActions.Interactive.triggered;
        public bool UseHealItem => GameManager.Instance.CanProcessInput && PlayerInputActions.UseHealItem.triggered;

        private bool aim;
        public bool Aim => GameManager.Instance.CanProcessInput && aim;

        private bool sprint;
        public bool Sprint => GameManager.Instance.CanProcessInput && sprint;
        public bool CanCrouch { get; set; }

        private int swichWeaponIDByGamepad = int.MaxValue;
        /// <summary>
        ///押してない時は int.MaxValue。武器チェンジする時は、0か1か２を返す.
        /// </summary>
        public int SwichWeaponID
        {
            get
            {
                if (!GameManager.Instance.CanProcessInput) return int.MaxValue;

                if (PlayerInputActions.SwichWeapon0.triggered) return 0;
                if (PlayerInputActions.SwichWeapon1.triggered) return 1;
                if (PlayerInputActions.SwichWeapon2.triggered) return 2;

                if (IsGamepad)
                {
                    if (PlayerInputActions.SwichWeaponByGamePad_Right.triggered)
                    {
                        swichWeaponIDByGamepad = (swichWeaponIDByGamepad + 1) % 3;
                        return swichWeaponIDByGamepad;
                    }

                    if (PlayerInputActions.SwichWeaponByGamePad_Left.triggered)
                    {
                        if (swichWeaponIDByGamepad - 1 < 0)
                        {
                            swichWeaponIDByGamepad = 2;
                        }

                        else
                        {
                            swichWeaponIDByGamepad -= 1;
                        }
                        return swichWeaponIDByGamepad;
                    }
                }
                return int.MaxValue;
            }
        }


        private void Awake()
        {
            inputActions = new MyInputActions();
            PlayerInputActions = inputActions.Player;

            PlayerInputActions.Fire.performed += ctx => heldFire = true;
            PlayerInputActions.Fire.canceled += ctx => heldFire = false;

            PlayerInputActions.Sprint.performed += ctx => sprint = true;
            PlayerInputActions.Sprint.canceled += ctx => sprint = false;

            PlayerInputActions.Aim.started += ctx => aim = true;
            PlayerInputActions.Aim.canceled += ctx => aim = false;

            PlayerInputActions.Crouch.started += ctx => CanCrouch = GameManager.Instance.CanProcessInput ? !CanCrouch : CanCrouch;

            PlayerInputActions.OpenOption.performed += ctx => GameManager.Instance.SwichConfiguUI();
        }

        private void OnEnable()
        {
            inputActions.Enable();
        }

        private void OnDisable()
        {
            inputActions.Disable();
        }
    }
}

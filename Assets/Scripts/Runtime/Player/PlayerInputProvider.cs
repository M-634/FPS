using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Musashi
{
    /**************************************************************************************************
     * reference : Doom Eternal and Apex 
     * if you want to check defult key bindngs and control settings, Please see the link below. 
     * https://docs.google.com/spreadsheets/d/1iQjKegCrqvZT50606PKLM4dYmw4fLeF6l2jI4MOS1bM/edit#gid=0
     **************************************************************************************************/
     
    /// <summary>
    /// プレイヤーの入力を管理するクラス
    /// </summary>
    public class PlayerInputProvider : MonoBehaviour
    {
        MyInputActions inputActions;
        public MyInputActions.PlayerActions PlayerInputActions { get; private set; }

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
                    if (GameManager.Instance.Configure.DoInvert_Y)
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
        public bool Interactive => GameManager.Instance.CanProcessInput && PlayerInputActions.InteractPickup.triggered;

        private bool aim;
        public bool Aim => GameManager.Instance.CanProcessInput && aim;

        private bool sprint;
        public bool Sprint => GameManager.Instance.CanProcessInput && sprint;
        public bool CanCrouch { get; set; } = true;

        public event Action<int> EquipWeaponAction;
        public event Action HolsterWeaponAction;

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

            PlayerInputActions.EquipWeapon1.performed += ctx => EquipWeaponAction.Invoke(0);
            PlayerInputActions.EquipWeapon2.performed += ctx => EquipWeaponAction.Invoke(1);
            PlayerInputActions.EquipWeapon3.performed += ctx => EquipWeaponAction.Invoke(2);
            PlayerInputActions.EquipWeapon4.performed += ctx => EquipWeaponAction.Invoke(3);

            PlayerInputActions.HolsterWeapon.performed += ctx => HolsterWeaponAction();
            PlayerInputActions.NextWeapon.performed += ctx => Debug.Log("up");
            PlayerInputActions.PreviousWeapon.performed += ctx => Debug.Log("down");
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

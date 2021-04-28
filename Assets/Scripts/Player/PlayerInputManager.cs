using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;


namespace Musashi
{
    /// <summary>
    /// プレイヤーの入力を管理するクラス
    /// </summary>
    public class PlayerInputManager : MonoBehaviour//このクラスに依存しているクラスが多いので依存関係を解消させること
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

        //Input property
        public Vector2 Move => PlayerInputActions.Move.ReadValue<Vector2>();
        public Vector2 Look => PlayerInputActions.Look.ReadValue<Vector2>();
        public Vector2 MousePosition => PlayerInputActions.MousePosition.ReadValue<Vector2>();
        public bool Jump => PlayerInputActions.Jump.triggered;
        public bool Fire => PlayerInputActions.Fire.triggered;

        private bool heldFire;
        public bool HeldFire => heldFire;
        public bool Reload => PlayerInputActions.Reload.triggered;
        public bool Interactive => PlayerInputActions.Interactive.triggered;
        public bool Inventory => PlayerInputActions.Inventory.triggered;
        public bool UseItem => PlayerInputActions.UseItem.triggered;
        public bool DropItem => PlayerInputActions.DropItem.triggered;

        private bool aim;
        public bool Aim => aim;

        private bool sprint;
        public bool Sprint => sprint;

        private int swichWeaponIDByGamepad = 0;
        /// <summary>
        ///押してない時は -１。武器チェンジする時は、0か1を返す.
        /// </summary>
        public int SwichWeaponID
        {
            get
            {
                if (PlayerInputActions.SwichWeapon0.triggered) return 0;
                if (PlayerInputActions.SwichWeapon1.triggered) return 1;
                if (PlayerInputActions.SwichWeaponByGamePad.triggered)
                {
                    //押す度に0と１を切り替える
                    swichWeaponIDByGamepad =(swichWeaponIDByGamepad + 1) % 2;
                    return swichWeaponIDByGamepad;
                }
                return -1;
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

            PlayerInputActions.Esc.performed += ctx => GameManager.Instance.SwichConfiguUI();
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

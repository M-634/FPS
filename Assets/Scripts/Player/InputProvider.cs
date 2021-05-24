using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;


namespace Musashi
{
    /// <summary>
    /// プレイヤーの入力を管理するクラス
    /// プレイヤーオブジェット全ての親オブジェットにアタッチすること
    /// </summary>
    public class InputProvider : MonoBehaviour//このクラスに依存しているクラスが多いので依存関係を解消させること
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
        public bool UseHealItem => PlayerInputActions.UseHealItem.triggered;
        public bool DropItem => PlayerInputActions.DropItem.triggered;

        private bool aim;
        public bool Aim => aim;

        private bool sprint;
        public bool Sprint => sprint;

        private int swichWeaponIDByGamepad = -1;
        /// <summary>
        ///押してない時は -１。武器チェンジする時は、0か1か２を返す.
        /// </summary>
        public int SwichWeaponID
        {
            get
            {
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

        //test
        private void Update()
        {
            if (Fire)
            {
                Debug.Log("fire");
            }

            if (heldFire)
            {
                Debug.Log("heldFire");
            }
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

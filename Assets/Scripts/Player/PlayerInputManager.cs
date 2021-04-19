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
        public bool IsGamepad => Gamepad.current.wasUpdatedThisFrame;

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
        public bool Aim => PlayerInputActions.Aim.triggered;

        private bool sprint;
        public bool Sprint => sprint;
          
        private void Awake()
        {
            inputActions = new MyInputActions();
            PlayerInputActions = inputActions.Player;

            PlayerInputActions.Fire.performed += ctx => heldFire = true;
            PlayerInputActions.Fire.canceled += ctx => heldFire = false;

            PlayerInputActions.Sprint.performed += ctx => sprint = true;
            PlayerInputActions.Sprint.canceled += ctx => sprint = false;

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

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;


namespace Musashi
{
    /// <summary>
    /// プレイヤーの入力を管理するクラス
    /// 必ず、playerタグが付いたオブジェクトにアタッチすること
    /// </summary>
    public class PlayerInputManager : MonoBehaviour
    {
        MyInputActions inputActions;
        MyInputActions.PlayerActions PlayerInputActions;
   
        public Vector2 Move => PlayerInputActions.Move.ReadValue<Vector2>();
        public Vector2 Look => PlayerInputActions.Look.ReadValue<Vector2>();
        public bool Jump => PlayerInputActions.Jump.triggered;
        public bool Fire => PlayerInputActions.Fire.triggered;

        private bool heldFire;
        public bool HeldFire => heldFire;
        public bool Reload => PlayerInputActions.Reload.triggered;
        public bool Interactive => PlayerInputActions.Interactive.triggered;
        public bool IsGamepad => Gamepad.current.wasUpdatedThisFrame;

        private void Awake()
        {
            inputActions = new MyInputActions();
            PlayerInputActions = inputActions.Player;

            PlayerInputActions.Fire.performed += ctx => heldFire = true;
            PlayerInputActions.Fire.canceled += ctx => heldFire = false;
        }

        private void Reset()
        {
            gameObject.tag = "Player";
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

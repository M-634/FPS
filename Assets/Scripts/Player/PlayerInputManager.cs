using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;


namespace Musashi
{
    /// <summary>
    /// プレイヤーの入力を管理するクラス
    /// </summary>
    public class PlayerInputManager : MonoBehaviour
    {
        MyInputActions inputActions;
        MyInputActions.PlayerActions PlayerInputActions;
   
        public Vector2 Move => PlayerInputActions.Move.ReadValue<Vector2>();
        public Vector2 Look => PlayerInputActions.Look.ReadValue<Vector2>();
        public bool Jump => PlayerInputActions.Jump.triggered;
        public bool Fire => PlayerInputActions.Fire.triggered;

        public bool IsGamepad => Gamepad.current.wasUpdatedThisFrame;

        private void Awake()
        {
            inputActions = new MyInputActions();
            PlayerInputActions = inputActions.Player;
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

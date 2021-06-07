using System.Collections;
using UnityEditor;
using UnityEngine;

namespace Musashi.Player
{
    /// <summary>
    /// プレイヤーの動きをステートパターンで制御するクラス。
    /// リファレンス：タイタンフォール２。APEX
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMoveStateMchine : MonoBehaviour
    {
        [Header("General")]
        [Tooltip("Force applied downward when in the air")]
        [SerializeField] float gravityDownForce = 20f;
        [Tooltip("distance from the bottom of the character controller capsule to test for grounded")]
        [SerializeField] float groundCheckDistance = 1f;
        [Tooltip("Physic layers checked to consider the player grounded")]
        [SerializeField] LayerMask groundLayer;

        [Header("Movement")]
        [Tooltip("Acceleration speed when in the air")]
        [SerializeField] float accelerationSpeedInAir = 25f;

        [Header("Jump")]
        [Tooltip("Force applied upward when jumping")]
        [SerializeField] float jumpForce = 10f;

        public float GravityDownForce => gravityDownForce;
        public float JumpFoce => jumpForce;
        public float AccelerationSpeedInAir => accelerationSpeedInAir;
        public Vector3 WorldSpaceMoveInput => transform.TransformVector(InputAction.GetMoveInput);
        public Vector3 GroundNormal { get; set; }
        public bool IsGround { get; set; }
        public Vector3 CharacterVelocity { get; set; }
        public CharacterController Controller { get; private set; }
        public InputProvider InputAction { get; private set; }
        public StateMachine<PlayerMoveStateMchine> StateMachine { get; private set; }

        #region State Property
        public IState<PlayerMoveStateMchine> OnGroundState { get; private set; } = new PlayerOnGroundState();
        public IState<PlayerMoveStateMchine> JumpState { get; private set; } = new PlayerJumpState();
        public IState<PlayerMoveStateMchine> CrouchingState { get; private set; } = new PlayerIsCrouchingState();
        public IState<PlayerMoveStateMchine> WallRunningState { get; private set; } = new PlayerWallRunState();
        #endregion

        private void Start()
        {
            InputAction = GetComponentInParent<InputProvider>();
            Controller = GetComponent<CharacterController>();
            StateMachine = new StateMachine<PlayerMoveStateMchine>(this, OnGroundState);
        }

        private void Update()
        {
            GroundCheck();
            StateMachine.CurrentState.OnUpdate(this);
        }

        private void GroundCheck()
        {
            IsGround = false;
            GroundNormal = Vector3.up;

            //cheack ground
            if (Physics.CapsuleCast(GetCapsuleBottomHemisphere(), GetCapsuleTopHemisphere(Controller.height), Controller.radius, Vector3.down, out RaycastHit hit, Controller.skinWidth + groundCheckDistance, groundLayer, QueryTriggerInteraction.Ignore))
            {
                GroundNormal = hit.normal;//地面の法線を取得

                //地面の傾きがキャラクターコントローラーで設定したSlopeLimitを超えているかどうか
                if (Vector3.Dot(hit.normal, transform.up) > 0f && IsNormalUnderSlopeLimit(GroundNormal))
                {
                    IsGround = true;

                    //空中にちょっと浮いていたら、地面まで戻す
                    //if (hit.distance > characterController.skinWidth)
                    //{
                    //    characterController.Move(Vector3.down * hit.distance);
                    //}
                }
            }
        }


        /// <summary>
        /// 斜辺上の時、進行方向のベクトルを調整する関数
        /// </summary>
        /// <param name="direction">Input direction</param>
        /// <param name="slopeNormal">groundNormal</param>
        /// <returns></returns>
        private Vector3 GetDirectionOnSlope(Vector3 direction, Vector3 slopeNormal)
        {
            Vector3 directRight = Vector3.Cross(direction, transform.up);
            return Vector3.Cross(slopeNormal, directRight).normalized;
        }

        // Returns true if the slope angle represented by the given normal is under the slope angle limit of the character controller
        bool IsNormalUnderSlopeLimit(Vector3 slopeNormal)
        {
            return Vector3.Angle(transform.up, slopeNormal) <= Controller.slopeLimit;
        }

        // Gets the center point of the bottom hemisphere of the character controller capsule    
        Vector3 GetCapsuleBottomHemisphere()
        {
            return transform.position + (transform.up * Controller.radius);
        }

        // Gets the center point of the top hemisphere of the character controller capsule    
        Vector3 GetCapsuleTopHemisphere(float atHeight)
        {
            return transform.position + (transform.up * (atHeight - Controller.radius));
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Handles.color = Color.red;
            Handles.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - groundCheckDistance, transform.position.z));
        }
#endif
    }

    /// <summary>
    /// プレイヤーが地上にいる時の動きを制御するクラス
    /// </summary>
    public class PlayerOnGroundState : IState<PlayerMoveStateMchine>
    {
        public void OnEnter(PlayerMoveStateMchine owner, IState<PlayerMoveStateMchine> prevState = null)
        {
            Debug.Log(this.ToString());
        }

        public void OnExit(PlayerMoveStateMchine owner, IState<PlayerMoveStateMchine> nextState = null)
        {

        }

        /// <summary>
        /// move on ground
        /// </summary>
        public void OnUpdate(PlayerMoveStateMchine owner)
        {
            if (owner.IsGround && owner.InputAction.Jump)
            {
                owner.StateMachine.ChangeState(owner.JumpState);
            }
        }
    }

    /// <summary>
    /// プレイヤーのジャンプ時の動きを制御するクラス
    /// </summary>
    public class PlayerJumpState : IState<PlayerMoveStateMchine>
    {
        public void OnEnter(PlayerMoveStateMchine owner, IState<PlayerMoveStateMchine> prevState = null)
        {
            //if prevState is CrouchingState, cancel crouching; 

            Debug.Log(this.ToString());
            //start by canceling out the vertical component of our velocity
            owner.CharacterVelocity = new Vector3(owner.CharacterVelocity.x, 0f, owner.CharacterVelocity.z);

            //prevState = WallRunState or OnGroundState によって加える力を分ける
            //then,add the jumpSpeed value upwards
            owner.CharacterVelocity += Vector3.up * owner.JumpFoce;
            //play sound

            //force grounding to false
            owner.IsGround = false;
            owner.GroundNormal = Vector3.up;
        }

        public void OnExit(PlayerMoveStateMchine owner, IState<PlayerMoveStateMchine> nextState = null)
        {

        }

        /// <summary>
        /// move in air
        /// </summary>
        public void OnUpdate(PlayerMoveStateMchine owner)
        {
            //add air acceleration
            owner.CharacterVelocity += owner.WorldSpaceMoveInput * owner.AccelerationSpeedInAir * Time.deltaTime;

            //limit air speed to maximum, but only horizontally
            

            // apply the gravity to the velocity
            owner.CharacterVelocity += Vector3.down * owner.GravityDownForce * Time.deltaTime;
        }
    }

    public class PlayerIsCrouchingState : IState<PlayerMoveStateMchine>
    {
        public void OnEnter(PlayerMoveStateMchine owner, IState<PlayerMoveStateMchine> prevState = null)
        {
            throw new System.NotImplementedException();
        }

        public void OnExit(PlayerMoveStateMchine owner, IState<PlayerMoveStateMchine> nextState = null)
        {
            throw new System.NotImplementedException();
        }

        public void OnUpdate(PlayerMoveStateMchine owner)
        {
            throw new System.NotImplementedException();
        }
    }

    public class PlayerWallRunState : IState<PlayerMoveStateMchine>
    {
        public void OnEnter(PlayerMoveStateMchine owner, IState<PlayerMoveStateMchine> prevState = null)
        {

        }

        public void OnExit(PlayerMoveStateMchine owner, IState<PlayerMoveStateMchine> nextState = null)
        {

        }

        public void OnUpdate(PlayerMoveStateMchine owner)
        {

        }
    }
}
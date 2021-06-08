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
        #region SerializeFields 
        [Header("General")]
        [Tooltip("Force applied downward when in the air")]
        [SerializeField] float gravityDownForce = 20f;
        [Tooltip("distance from the bottom of the character controller capsule to test for grounded")]
        [SerializeField] float groundCheckDistance = 1f;
        [Tooltip("Physic layers checked to consider the player grounded")]
        [SerializeField] LayerMask groundLayer;

        [Header("Movement")]
        [Tooltip("Max movement speed when grounded (when not sprinting)")]
        [SerializeField] float maxSpeedOnGround = 10f;
        [Tooltip("Sharpness for the movement when grounded, a low value will make the player accelerate and decelerate slowly, a high value will do the opposite")]
        [SerializeField] float movementSharpnessOnGround = 15;
        [Tooltip("Max movement speed when crouching")]
        [SerializeField, Range(0, 1)] float maxSpeedCrouchedRatio = 0.5f;
        [Tooltip("Max movement speed when not grounded")]
        [SerializeField] float maxSpeedInAir = 10f;
        [Tooltip("Acceleration speed when in the air")]
        [SerializeField] float accelerationSpeedInAir = 25f;
        [Tooltip("Multiplicator for the sprint speed based on grounded speed")]
        [SerializeField] float sprintSpeedModifier = 2f;

        [Header("Jump")]
        [Tooltip("Force applied upward when jumping")]
        [SerializeField] float jumpForce = 10f;
        #endregion

        #region  Member variables
        bool isGround;
        Vector3 groundNormal;
        Vector3 characterVelocity;
        CharacterController controller;
        InputProvider inputProvider;
        StateMachine<PlayerMoveStateMchine> stateMachine;
        #endregion

        #region Utility properties
        bool IsSprinting => inputProvider.Sprint && inputProvider.GetMoveInput.z > 0f;
        float SpeedModifier => IsSprinting ? sprintSpeedModifier : 1f;
        Vector3 WorldSpaceMoveInput => transform.TransformVector(inputProvider.GetMoveInput);
        #endregion

        #region State properties
        IState<PlayerMoveStateMchine> OnGroundState => new PlayerOnGroundState();
        IState<PlayerMoveStateMchine> JumpState => new PlayerJumpState();
        IState<PlayerMoveStateMchine> CrouchingState => new PlayerIsCrouchingState();
        IState<PlayerMoveStateMchine> WallRunningState => new PlayerWallRunState();
        #endregion

        #region Methods
        private void Start()
        {
            inputProvider = GetComponentInParent<InputProvider>();
            controller = GetComponent<CharacterController>();
            stateMachine = new StateMachine<PlayerMoveStateMchine>(this, OnGroundState);
            GameManager.Instance.LockCusor();
        }

        private void Update()
        {
            GroundCheck();
            stateMachine.CurrentState.OnUpdate(this);
            controller.Move(characterVelocity * Time.deltaTime);
        }

        private void GroundCheck()
        {
            isGround = false;
            groundNormal = Vector3.up;

            //cheack ground
            if (Physics.CapsuleCast(GetCapsuleBottomHemisphere(), GetCapsuleTopHemisphere(controller.height), controller.radius, Vector3.down, out RaycastHit hit, controller.skinWidth + groundCheckDistance, groundLayer, QueryTriggerInteraction.Ignore))
            {
                groundNormal = hit.normal;//地面の法線を取得

                //地面の傾きがキャラクターコントローラーで設定したSlopeLimitを超えているかどうか
                if (Vector3.Dot(hit.normal, transform.up) > 0f && IsNormalUnderSlopeLimit(groundNormal))
                {
                    isGround = true;

                    //空中にちょっと浮いていたら、地面まで戻す
                    //if (hit.distance > characterController.skinWidth)
                    //{
                    //    characterController.Move(Vector3.down * hit.distance);
                    //}
                }
            }
        }

        /// <summary>
        /// 地上にいる時のプレイヤーの動きを制御する関数
        /// </summary>
        /// <param name="isCrouching"></param>
        private void HandleGroundedMovment(bool isCrouching)
        {
            Vector3 targetVelocity = WorldSpaceMoveInput * maxSpeedOnGround * SpeedModifier;
            if (isCrouching)
            {
                targetVelocity *= maxSpeedCrouchedRatio;
            }
            targetVelocity = GetDirectionOnSlope(targetVelocity.normalized, groundNormal) * targetVelocity.magnitude;

            //smmothly interpolat between our current velocity and the target velocity based on acceleration speed
            characterVelocity = Vector3.Lerp(characterVelocity, targetVelocity, movementSharpnessOnGround * Time.deltaTime);
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
            return Vector3.Angle(transform.up, slopeNormal) <= controller.slopeLimit;
        }

        // Gets the center point of the bottom hemisphere of the character controller capsule    
        Vector3 GetCapsuleBottomHemisphere()
        {
            return transform.position + (transform.up * controller.radius);
        }

        // Gets the center point of the top hemisphere of the character controller capsule    
        Vector3 GetCapsuleTopHemisphere(float atHeight)
        {
            return transform.position + (transform.up * (atHeight - controller.radius));
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Handles.color = Color.red;
            Handles.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - groundCheckDistance, transform.position.z));
        }
#endif
        #endregion
        
        /// <summary>
        /// プレイヤーが地上にいる時の動きを制御するクラス
        /// </summary>
        private class PlayerOnGroundState : IState<PlayerMoveStateMchine>
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
                if (owner.isGround && owner.inputProvider.Jump)
                {
                    owner.stateMachine.ChangeState(owner.JumpState);
                    return;
                }
                owner.HandleGroundedMovment(false);
            }
        }

        /// <summary>
        /// プレイヤーのジャンプ時の動きを制御するクラス
        /// </summary>
        private class PlayerJumpState : IState<PlayerMoveStateMchine>
        {
            public void OnEnter(PlayerMoveStateMchine owner, IState<PlayerMoveStateMchine> prevState = null)
            {
                Debug.Log(this.ToString());
                //if prevState is CrouchingState, cancel crouching; 

                //start by canceling out the vertical component of our velocity
                owner.characterVelocity = new Vector3(owner.characterVelocity.x, 0f, owner.characterVelocity.z);

                //prevState = WallRunState or OnGroundState によって加える力を分ける
                //then,add the jumpSpeed value upwards
                owner.characterVelocity += Vector3.up * owner.jumpForce;
                //play sound

                //force grounding to false
                owner.isGround = false;
                owner.groundNormal = Vector3.up;
            }

            public void OnExit(PlayerMoveStateMchine owner, IState<PlayerMoveStateMchine> nextState = null)
            {

            }

            /// <summary>
            /// move in air
            /// </summary>
            public void OnUpdate(PlayerMoveStateMchine owner)
            {
                if (owner.isGround)
                {
                    owner.stateMachine.ChangeState(owner.OnGroundState);
                    return;
                }

                //add air acceleration
                owner.characterVelocity += owner.WorldSpaceMoveInput * owner.accelerationSpeedInAir * Time.deltaTime;

                //limit air speed to maximum, but only horizontally
                float verticalVelocity = owner.characterVelocity.y;
                Vector3 horizontalVelocity = Vector3.ProjectOnPlane(owner.characterVelocity, Vector3.up);
                horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, owner.maxSpeedInAir * owner.SpeedModifier);
                owner.characterVelocity = horizontalVelocity + (Vector3.up * verticalVelocity);

                // apply the gravity to the velocity
                owner.characterVelocity += Vector3.down * owner.gravityDownForce * Time.deltaTime;
            }
        }

        private class PlayerIsCrouchingState : IState<PlayerMoveStateMchine>
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

        private class PlayerWallRunState : IState<PlayerMoveStateMchine>
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
}
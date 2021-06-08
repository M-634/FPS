using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace Musashi.Player
{
    /// <summary>
    /// プレイヤーの動きをステートパターンで制御するクラス。
    /// リファレンス：タイタンフォール２。APEX
    /// </summary>
    [RequireComponent(typeof(CharacterController),typeof(AudioSource))]
    public class PlayerMoveStateMchine : MonoBehaviour
    {
        #region SerializeFields 
        [Header("References")]
        [Tooltip("Reference to the main camera used for the player")]
        [SerializeField] Camera playerCamera;

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

        [Header("Stance")]
        [Tooltip("Ratio (0-1) of the character height where the camera will be at")]
        public float cameraHeightRatio = 0.9f;
        [Tooltip("Height of character when standing")]
        public float capsuleHeightStanding = 1.8f;
        [Tooltip("Height of character when crouching")]
        public float capsuleHeightCrouching = 0.9f;
        [Tooltip("Speed of crouching transitions")]
        public float crouchingSharpness = 10f;

        [Header("Audio")]
        [Tooltip("Amount of footstep sounds played when moving one meter")]
        [SerializeField] float footstepSFXFrequency = 1f;
        [Tooltip("Amount of footstep sounds played when moving one meter while sprinting")]
        [SerializeField] float footstepSFXFrequencyWhileSprinting = 1f;
        [Tooltip("Sound played for footsteps")]
        [SerializeField] AudioClip footstepSFX;
        [Tooltip("Sound played when jumping")]
        [SerializeField] AudioClip jumpSFX;
        [Tooltip("Sound played when landing")]
        [SerializeField] AudioClip landSFX;
        [Tooltip("Sound played when taking damage froma fall")]
        [SerializeField] AudioClip fallDamageSFX;
        #endregion

        #region  Member variables
        bool isGround;
        float footstepDistanceCounter;
        float targetCharacterHeight;
        float lastTimeJumped;
        Vector3 groundNormal;
        Vector3 characterVelocity;
        CharacterController controller;
        InputProvider inputProvider;
        AudioSource audioSource;
        StateMachine<PlayerMoveStateMchine> stateMachine;
        #endregion

        #region Const memeber variables
        float k_GroundCheckDistanceInAir = 0.2f;
        float k_JumpGroundingPreventionTime = 0.07f;
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
           if(playerCamera == null)
            {
                playerCamera = Camera.main;
            }
            inputProvider = GetComponentInParent<InputProvider>();
            controller = GetComponent<CharacterController>();
            stateMachine = new StateMachine<PlayerMoveStateMchine>(this, OnGroundState);
            audioSource = GetComponent<AudioSource>();
            GameManager.Instance.LockCusor();

            UpdateCharacterHeight(true);
        }

        private void Update()
        {
            GroundCheck();
            stateMachine.CurrentState.OnUpdate(this);
            controller.Move(characterVelocity * Time.deltaTime);
        }

        private void GroundCheck()
        {
            float chosenGroundCheckDistance = isGround ? (controller.skinWidth + groundCheckDistance) : k_GroundCheckDistanceInAir;

            //reset values befor the ground check
            isGround = false;
            groundNormal = Vector3.up;

            //ジャンプしてすぐにチェックはしない
            if (Time.time >= lastTimeJumped + k_JumpGroundingPreventionTime)
            {
                //cheack ground
                if (Physics.CapsuleCast(GetCapsuleBottomHemisphere(), GetCapsuleTopHemisphere(controller.height), controller.radius, Vector3.down, out RaycastHit hit, chosenGroundCheckDistance, groundLayer, QueryTriggerInteraction.Ignore))
                {
                    groundNormal = hit.normal;//地面の法線を取得

                    //地面の傾きがキャラクターコントローラーで設定したSlopeLimitを超えているかどうか
                    if (Vector3.Dot(hit.normal, transform.up) > 0f && IsNormalUnderSlopeLimit(groundNormal))
                    {
                        isGround = true;

                        ////空中にちょっと浮いていたら、地面まで戻す
                        if (hit.distance > controller.skinWidth)
                        {
                            controller.Move(Vector3.down * hit.distance);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 地上にいる時のプレイヤーの動きを制御する関数
        /// </summary>
        /// <param name="isCrouching"></param>
        private void HandleGroundedMovment(IState<PlayerMoveStateMchine> state)
        {
            UpdateCharacterHeight(false);

            Vector3 targetVelocity = WorldSpaceMoveInput * maxSpeedOnGround * SpeedModifier;
            if (state is PlayerIsCrouchingState)
            {
                targetVelocity *= maxSpeedCrouchedRatio;
            }
            targetVelocity = GetDirectionOnSlope(targetVelocity.normalized, groundNormal) * targetVelocity.magnitude;

            //smmothly interpolat between our current velocity and the target velocity based on acceleration speed
            characterVelocity = Vector3.Lerp(characterVelocity, targetVelocity, movementSharpnessOnGround * Time.deltaTime);

            //play footsteps Sound
            FootstepsSound();
        }

        /// <summary>
        /// キャラクターの高さとカメラ位置を調整する関数
        /// </summary>
        /// <param name="force"></param>
        private void UpdateCharacterHeight(bool force)
        {
            if (force)
            {
                controller.height = targetCharacterHeight;
                controller.center = Vector3.up * controller.height * 0.5f;
                playerCamera.transform.localPosition = Vector3.up * targetCharacterHeight * cameraHeightRatio;
            }
            // Update smooth height
            else if (controller.height != targetCharacterHeight)
            {
                // resize the capsule and adjust camera position
                controller.height = Mathf.Lerp(controller.height, targetCharacterHeight, crouchingSharpness * Time.deltaTime);
                controller.center = Vector3.up * controller.height * 0.5f;
                playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, Vector3.up * targetCharacterHeight * cameraHeightRatio, crouchingSharpness * Time.deltaTime);
            }
        }

        /// <summary>
        /// 足音を制限するクラス
        /// </summary>
        private void FootstepsSound()
        {
            float chosenFootstepSFXFrequency = IsSprinting ? footstepSFXFrequencyWhileSprinting : footstepSFXFrequency;
            if(footstepDistanceCounter >= 1f / chosenFootstepSFXFrequency)
            {
                footstepDistanceCounter = 0f;
                audioSource.Play(footstepSFX);
            }
            footstepDistanceCounter += characterVelocity.magnitude * Time.deltaTime;
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
        #endregion
        
        /// <summary>
        /// プレイヤーが地上にいる時の動きを制御するクラス
        /// </summary>
        private class PlayerOnGroundState : IState<PlayerMoveStateMchine>
        {
            public void OnEnter(PlayerMoveStateMchine owner, IState<PlayerMoveStateMchine> prevState = null)
            {
                Debug.Log(this.ToString());
                owner.targetCharacterHeight = owner.capsuleHeightStanding;
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

                if (!owner.IsSprinting && owner.inputProvider.CanCrouch)
                {
                    owner.stateMachine.ChangeState(owner.CrouchingState);
                    return;
                }
                owner.HandleGroundedMovment(this);
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

                //play jump sound

                //force grounding to false
                owner.isGround = false;
                owner.groundNormal = Vector3.up;

                owner.lastTimeJumped = Time.time;
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

        /// <summary>
        /// プレイヤーのしゃがみ状態を制限するクラス
        /// </summary>
        private class PlayerIsCrouchingState : IState<PlayerMoveStateMchine>
        {
            public void OnEnter(PlayerMoveStateMchine owner, IState<PlayerMoveStateMchine> prevState = null)
            {
                Debug.Log(this.ToString());

                owner.targetCharacterHeight = owner.capsuleHeightCrouching;
            }

            public void OnExit(PlayerMoveStateMchine owner, IState<PlayerMoveStateMchine> nextState = null)
            {
                owner.inputProvider.CanCrouch = false;
            }

            public void OnUpdate(PlayerMoveStateMchine owner)
            {
                if (owner.IsSprinting || !owner.inputProvider.CanCrouch)
                {
                    owner.stateMachine.ChangeState(owner.OnGroundState);
                    return;
                }
                owner.HandleGroundedMovment(this);
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
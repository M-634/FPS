using System.Collections;
using UnityEditor;
using UnityEngine;

namespace Musashi.Player
{
    /// <summary>
    /// プレイヤーの動きをステートパターンで制御するクラス。
    /// リファレンス：タイタンフォール２。APEX
    /// </summary>
    [RequireComponent(typeof(CharacterController), typeof(AudioSource))]
    public partial class PlayerCharacterStateMchine : MonoBehaviour
    {
        #region SerializeFields 
        [Header("Camera settings")]
        [Tooltip("Reference to the main camera used for the player")]
        [SerializeField] Camera playerCamera;
        [SerializeField] float defultFieldOfView = 60f;
        //[SerializeField] float mouseSensitivity = 2f;
        //[SerializeField] float controllerSensitivity = 100f;
        //[SerializeField] float aimingRotaionMultipiler = 0.4f;
        //[SerializeField] float cameraTransitionDuration = 1;

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
        [Tooltip("Multipilcator for the second jumping based on first jumpForce")]
        [SerializeField] float secondJumpModifier = 1.2f;
        [Tooltip("Time from the first jump to being able to make the second jump.")]
        [SerializeField] float firstJumpDuration = 0.1f;
     
        [Header("Stance")]
        [Tooltip("Ratio (0-1) of the character height where the camera will be at")]
        [SerializeField] float cameraHeightRatio = 0.9f;
        [Tooltip("Height of character when standing")]
        [SerializeField] float capsuleHeightStanding = 1.8f;
        [Tooltip("Height of character when crouching")]
        [SerializeField] float capsuleHeightCrouching = 0.9f;
        [Tooltip("Speed of crouching transitions")]
        [SerializeField] float crouchingSharpness = 10f;

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

        [Space]
        [SerializeField] PlayerWallRunState WallRunState;
        #endregion

        #region  Member variables
        bool isGround;
        bool isAiming;
        bool isDushJump;//ダッシュ速度でジャンプしていたかどうか判断するフラグ。
        float fovSpeed;
        float targetFov;
        float cameraVerticalAngle;
        float footstepDistanceCounter;
        float targetCharacterHeight;
        float lastTimeFirstJumped;
        Vector3 groundNormal;
        Vector3 characterVelocity;
        CharacterController controller;
        InputProvider inputProvider;
        AudioSource audioSource;
        StateMachine<PlayerCharacterStateMchine> stateMachine;
        #endregion

        #region Const memeber variables
        const float k_GroundCheckDistanceInAir = 0.2f;
        const float k_JumpGroundingPreventionTime = 0.07f;
        #endregion

        //ここプロパティである必要がない
        #region Utility properties
        public bool IsSprinting => isGround && inputProvider.Sprint && inputProvider.GetMoveInput.z > 0f;
        public float SpeedModifier => IsSprinting ? sprintSpeedModifier : 1f;
        public Vector3 WorldSpaceMoveInput => transform.TransformVector(inputProvider.GetMoveInput);
        public float CameraRotaionMuliplier => isAiming ? GameManager.Instance.Configure.AimingRotaionMultipiler : 1f;
        public float CameraSensitivity => InputProvider.IsGamepad ? GameManager.Instance.Configure.ControllerSensitivity : GameManager.Instance.Configure.MouseSensitivity;
        #endregion

        #region State properties
        IState<PlayerCharacterStateMchine> OnGroundState => new PlayerOnGroundState();
        IState<PlayerCharacterStateMchine> JumpState => new PlayerJumpState();
        IState<PlayerCharacterStateMchine> CrouchingState => new PlayerIsCrouchingState();
        // IState<PlayerMoveStateMchine> WallRunningState => new PlayerWallRunState();
        #endregion

        #region Methods
        private void Start()
        {
            if (playerCamera == null)
            {
                playerCamera = Camera.main;
            }
            inputProvider = GetComponentInParent<InputProvider>();
            controller = GetComponent<CharacterController>();
            audioSource = GetComponent<AudioSource>();

            stateMachine = new StateMachine<PlayerCharacterStateMchine>(this, OnGroundState);
            GameManager.Instance.LockCusor();

            UpdateCharacterHeight(true);
            playerCamera.fieldOfView = defultFieldOfView;
        }


        private void Update()
        {
            ControlCameraAndPlayerRotation();
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
            if (Time.time < lastTimeFirstJumped + k_JumpGroundingPreventionTime) return;

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

        /// <summary>
        /// 地上にいる時のプレイヤーの動きを制御する関数
        /// </summary>
        /// <param name="isCrouching"></param>
        private void HandleGroundedMovment(IState<PlayerCharacterStateMchine> state)
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
            float chosenFootstepSFXFrequency = IsSprinting ? footstepSFXFrequencyWhileSprinting : footstepSFXFrequency;
            if (footstepDistanceCounter >= 1f / chosenFootstepSFXFrequency)
            {
                footstepDistanceCounter = 0f;
                audioSource.Play(footstepSFX);
            }
            footstepDistanceCounter += characterVelocity.magnitude * Time.deltaTime;
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
        /// プレイヤーの回転とカメラの制御をする関数
        /// </summary>
        private void ControlCameraAndPlayerRotation()
        {
            //horizoltal rotation
            transform.Rotate(new Vector3(0f, inputProvider.GetLookInputsHorizontal * CameraSensitivity * CameraRotaionMuliplier, 0f), Space.Self);

            //vertical rotation
            cameraVerticalAngle += inputProvider.GetLookInputVertical * CameraSensitivity * CameraRotaionMuliplier;
            cameraVerticalAngle = Mathf.Clamp(cameraVerticalAngle, -89f, 89f);

            playerCamera.transform.localEulerAngles = new Vector3(cameraVerticalAngle, 0f, WallRunState.GetCameraAngle(this));

            //set field of view
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFov, Time.deltaTime * fovSpeed);
        }

        /// <summary>
        /// 外部からカメラのFOVを変更する関数。
        /// </summary>
        public void SetFovOfCamera(bool isAiming, float targetFov, float fovSpeed)
        {
            this.isAiming = isAiming;
            if (isAiming)
            {
                this.targetFov = targetFov;
            }
            else
            {
                this.targetFov = defultFieldOfView; 
            }
            this.fovSpeed = fovSpeed;
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
    }
}
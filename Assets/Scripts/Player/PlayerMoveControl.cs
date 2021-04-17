using UnityEngine;
using UnityEditor;
using TMPro;

namespace Musashi
{
    [RequireComponent(typeof(PlayerInputManager))]
    public class PlayerMoveControl : MonoBehaviour
    {
        [Header("Move")]
        [SerializeField] float maxSpeedOnGround = 1f;
        [SerializeField] float sprintSpeedModifier = 2f;
        [SerializeField] float movementSharpnessOnGround = 15f;

        [SerializeField] float gravity = 9.81f;
        [SerializeReference, Tooltip("maxVelocity = jumpPower / 10 が目安")] float maxVelocity = 10f;//ジャンプ時の最大加速度
        [SerializeField] float jumpPower = 100f;
        [SerializeField] float groundCheckDistance = 1f;
        [SerializeField] LayerMask groundLayer;
        public bool isGround;
        public enum State { Normal, JumpUpper, JumpDown, Grappling, }
        public State state;

        [Header("Camera")]
        [SerializeField] PlayerCamaraControl cameraControl;
    
        [Header("Audio")]
        [SerializeField] float footstepSFXFrequency = 0.3f;
        float footstepDistanceCounter;

        [SerializeField] AudioClip footstepSFX;
        [SerializeField] AudioClip jumpSFX;
        [SerializeField] AudioClip landSFX;
        AudioSource audioSource;

        PlayerAnimationController animationController;
        PlayerInputManager playerInputManager;
        CharacterController characterController;
        Vector3 characterVelocity;
        Vector3 groundNormal;

        private void Start()
        {
            animationController = GetComponent<PlayerAnimationController>();
            playerInputManager = GetComponent<PlayerInputManager>();
            characterController = GetComponent<CharacterController>();
            audioSource = GetComponent<AudioSource>();

            GameManager.Instance.LockCusor();
        }

   
        private void Update()
        {
            //isGround = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
            GroundCheck();

            // if (inputManager.HasPutJumpButton && isGround) { state = State.JumpUpper; }

            if (state == State.JumpUpper)
            {
                characterVelocity += Vector3.up * jumpPower * Time.deltaTime;
                characterController.Move(characterVelocity * Time.deltaTime);
                if (Mathf.Abs(characterVelocity.y) >= maxVelocity)
                {
                    state = State.JumpDown;
                }
            }

            if (state == State.JumpDown)
            {
                CalculationGravity();
                characterController.Move(characterVelocity * Time.deltaTime);

                if (isGround)
                {
                    characterVelocity.y = 0;
                    audioSource.Play(landSFX);
                    state = State.Normal;
                }
            }

            //grounded movement
            if (state == State.Normal)
            {
                //inputManager.HasPutJumpButtonを変更する
                if (isGround && playerInputManager.Jump)
                {
                    state = State.JumpUpper;
                    audioSource.Play(jumpSFX);
                    return;
                }

                if (!isGround)
                {
                    state = State.JumpDown;
                    return;
                }


                var dir = transform.right * playerInputManager.Move.x + transform.forward * playerInputManager.Move.y;

                var isSprinting = false;//PlayerInputManager.Dash();
                var speedModifier = isSprinting ? sprintSpeedModifier : 1f;

                var targetVelocity = dir.normalized * maxSpeedOnGround * speedModifier;


                targetVelocity = GetDirectionOnSlope(targetVelocity, groundNormal) * targetVelocity.magnitude;//grouundNormal = 0 => 0;
                characterVelocity = Vector3.Lerp(characterVelocity, targetVelocity, movementSharpnessOnGround * Time.deltaTime);

                //footsteps sound
                if (footstepDistanceCounter > 1f / footstepSFXFrequency)
                {   
                    footstepDistanceCounter = 0;
                    audioSource.PlayRandomPitch(footstepSFX, 1f, 0.5f);
                }
                footstepDistanceCounter += characterVelocity.magnitude * Time.deltaTime;

                //Move
                characterController.Move(characterVelocity * Time.deltaTime);
            }

          
            //Set animation
            if (animationController)
                animationController.MoveAnimation(characterVelocity.magnitude,state);
        }

        void GroundCheck()
        {

            isGround = false;
            groundNormal = Vector3.up;

            //if (grapplingGun.IsGrappling || state == State.JumpUpper) return;
            if (state == State.Grappling || state == State.JumpUpper) return;


            //cheack ground
            if (Physics.CapsuleCast(GetCapsuleBottomHemisphere(), GetCapsuleTopHemisphere(characterController.height), characterController.radius, Vector3.down, out RaycastHit hit, characterController.skinWidth + groundCheckDistance, groundLayer, QueryTriggerInteraction.Ignore))
            {
                groundNormal = hit.normal;//地面の法線を取得

                //地面の傾きがキャラクターコントローラーで設定したSlopeLimitを超えているかどうか
                if (Vector3.Dot(hit.normal, transform.up) > 0f && IsNormalUnderSlopeLimit(groundNormal))
                {
                    isGround = true;

                    //空中にちょっと浮いていたら、地面まで戻す
                    if (hit.distance > characterController.skinWidth)
                    {
                        characterController.Move(Vector3.down * hit.distance);
                    }
                }
            }
        }

        private void CalculationGravity()
        {
            characterVelocity += Vector3.down * gravity * Time.deltaTime;
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
            return Vector3.Angle(transform.up, slopeNormal) <= characterController.slopeLimit;
        }

        // Gets the center point of the bottom hemisphere of the character controller capsule    
        Vector3 GetCapsuleBottomHemisphere()
        {
            return transform.position + (transform.up * characterController.radius);
        }

        // Gets the center point of the top hemisphere of the character controller capsule    
        Vector3 GetCapsuleTopHemisphere(float atHeight)
        {
            return transform.position + (transform.up * (atHeight - characterController.radius));
        }


#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Handles.color = Color.red;
            Handles.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - groundCheckDistance, transform.position.z));
        }
#endif
    }

}

using UnityEngine;
using UnityEditor;
using TMPro;

namespace Musashi
{
    public class PlayerMoveControl : MonoBehaviour
    {
        private enum State { IsGround, Jumping, Falling, }
        private State state;
     
        [Header("Move")]
        [SerializeField] float maxSpeedOnGround = 1f;
        [SerializeField] float sprintSpeedModifier = 2f;
        [SerializeField] float movementSharpnessOnGround = 15f;
        bool isSprint;
        Vector3 characterVelocity;

        [Header("Gravity")]
        [SerializeField] float gravity = 9.81f;

        [Header("Jump")]
        [SerializeReference, Tooltip("maxVelocity = jumpPower / 10 が目安")] float maxVelocity = 10f;//ジャンプ時の最大加速度
        [SerializeField] float jumpPower = 100f;
        [SerializeField] float inputSensitivityInAirModifier = 0.2f;//空中時に入力から受けるベクトルの大きさを調整する

        [Header("Cheack Ground")]
        [SerializeField] float groundCheckDistance = 1f;
        [SerializeField] LayerMask groundLayer;
        Vector3 groundNormal;
        bool isGround;

        [Header("Camera")]
        //[SerializeField] PlayerCamaraControl cameraControl;

        [Header("Audio")]
        [SerializeField] float footstepSFXFrequency = 0.3f;
        float footstepDistanceCounter;
        [SerializeField] AudioClip footstepSFX;
        [SerializeField] AudioClip jumpSFX;
        [SerializeField] AudioClip landSFX;
        AudioSource audioSource;

        InputProvider playerInput;
        CharacterController characterController;

       

        private void Start()
        {
            playerInput = GetComponentInParent<InputProvider>();
            characterController = GetComponent<CharacterController>();
            audioSource = GetComponent<AudioSource>();

            GameManager.Instance.LockCusor();
        }

        private void Update()
        {
            GroundCheck();
            var inputDirect = transform.right * playerInput.Move.x + transform.forward * playerInput.Move.y;
            if (state == State.Jumping)
            {
                characterVelocity +=  Vector3.up * jumpPower * Time.deltaTime;
                characterVelocity += inputDirect.normalized * inputSensitivityInAirModifier;
                if (Mathf.Abs(characterVelocity.y) >= maxVelocity)
                {
                    state = State.Falling;
                }
            }
            if (state == State.Falling)
            {
                CalculationGravity();
                characterVelocity += inputDirect.normalized * inputSensitivityInAirModifier;
                if (isGround)
                {
                    characterVelocity.y = 0;
                    audioSource.Play(landSFX);
                    state = State.IsGround;
                }
            }
            //grounded movement
            if (state == State.IsGround)
            {
                if (isGround && playerInput.Jump)
                {
                    state = State.Jumping;
                    audioSource.Play(jumpSFX);
                    return;
                }

                if (!isGround)
                {
                    state = State.Falling;
                    return;
                }

                // isSprint?
                if(playerInput.Move.y < 0.1f) isSprint = false;
                if (playerInput.Sprint) isSprint = true;
                //modify sprint move
                var speedModifier = isSprint && playerInput.Move.y > 0f ? sprintSpeedModifier : 1f;

                //target velocity 
                var targetVelocity = inputDirect.normalized * maxSpeedOnGround * speedModifier;
                targetVelocity = GetDirectionOnSlope(targetVelocity, groundNormal) * targetVelocity.magnitude;//grouundNormal = 0 => 0;
                characterVelocity = Vector3.Lerp(characterVelocity, targetVelocity, movementSharpnessOnGround * Time.deltaTime);

                //footsteps sound
                if (footstepDistanceCounter > 1f / footstepSFXFrequency)
                {
                    footstepDistanceCounter = 0;
                    audioSource.PlayRandomPitch(footstepSFX, 1f, 0.5f);
                }
                footstepDistanceCounter += characterVelocity.magnitude * Time.deltaTime;

            }
            //final Move
            characterController.Move(characterVelocity * Time.deltaTime);
        }

        void GroundCheck()
        {
            isGround = false;
            groundNormal = Vector3.up;

            if (state == State.Jumping) return;
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

using UnityEngine;
using UnityEditor;
using TMPro;

namespace Musashi
{
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


        [Header("Grappling Weapon")]
        [SerializeField] GrapplingGun grapplingGun;
        [SerializeField] ParticleSystem grapplingEffect;

        [Header("Camera")]
        [SerializeField] PlayerCamaraControl cameraControl;
        //[SerializeField] float NOMAL_FOV = 60f;
        //[SerializeField] float GRAPPLING_FOV = 90f;

        [Header("Audio")]
        [SerializeField] float footstepSFXFrequency = 0.3f;
        float footstepDistanceCounter;

        [SerializeField] AudioClip footstepSFX;
        [SerializeField] AudioClip jumpSFX;
        [SerializeField] AudioClip landSFX;
        [SerializeField] AudioClip grapplingWindSFX;
        AudioSource audioSource;


        [Header("Debug")]
        [SerializeField] TextMeshProUGUI[] texts;

        PlayerInputManager inputManager;
        PlayerAnimationController animationController;
        CharacterController characterController;
        Vector3 characterVelocity;
        Vector3 groundNormal;

        private void Start()
        {
            inputManager = GetComponent<PlayerInputManager>();
            characterController = GetComponent<CharacterController>();
            animationController = GetComponent<PlayerAnimationController>();
            audioSource = GetComponent<AudioSource>();

            grapplingEffect.Stop();
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
                if (isGround && inputManager.HasPutJumpButton)
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

                var x = inputManager.Input_X;
                var z = inputManager.Input_Z;

                var dir = transform.forward * z + transform.right * x;

                var isSprinting = PlayerInputManager.Dash();
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

            //ここの動きの修正は
            if (state == State.Grappling)
            {
                //set camera
                if (cameraControl)
                    cameraControl.SetGrapplingFov();

                //play effect
                if (grapplingEffect.isStopped)
                    grapplingEffect.Play();

                //Play Audio
                //memo:条件分岐付けないと音は鳴らないことに注意
                if (!audioSource.isPlaying)
                    audioSource.Play(grapplingWindSFX);

                var dir = grapplingGun.GetGrapplePoint - transform.position;

                //Adjust speed 
                var grapplingSpeedMin = 10f;
                var grapplingSpeedMax = 40f;
                var grapplingSpeed = Mathf.Clamp(Vector3.Distance(transform.position, grapplingGun.GetGrapplePoint), grapplingSpeedMin, grapplingSpeedMax);
                var hookshotSpeedMultipiker = 2f;


                //Grappling move   
                characterController.Move(dir.normalized * grapplingSpeed * hookshotSpeedMultipiker * Time.deltaTime);

                //Stop Grapping move
                if (Vector3.Distance(transform.position, grapplingGun.GetGrapplePoint) < 1.0f)
                {
                    state = State.JumpDown;
                    grapplingGun.IsGrappling = false;

                    //Stop audio
                    audioSource.StopWithFadeOut(0.1f);

                    //Stop effect
                    grapplingEffect.Stop();

                    if (cameraControl)
                        cameraControl.SetNormalFov();
                }

                //Cancel Grappling move
                //memo;キャンセル後の動きもゲームのおもしさに関わると思うので要調整が必要。
                if (inputManager.HasPutJumpButton)
                {
                    characterVelocity = dir;
                    grapplingGun.IsGrappling = false;
                    state = State.JumpUpper;

                    //Stop audio
                    audioSource.StopWithFadeOut(0.1f);

                    //Stop audio
                    grapplingEffect.Stop();

                    if (cameraControl)
                        cameraControl.SetNormalFov();
                }
            }

            if (grapplingGun.IsGrappling) state = State.Grappling;

            //Set animation
            texts[0].text = characterVelocity.magnitude.ToString("F2");
            if (animationController)
                animationController.MoveAnimation(characterVelocity.magnitude);
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

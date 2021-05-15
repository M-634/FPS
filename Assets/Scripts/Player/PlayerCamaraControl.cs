using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Musashi
{
    [RequireComponent(typeof(PlayerInputManager))]
    public class PlayerCamaraControl : MonoBehaviour
    {
        [Header("Camera base settings")]
        [SerializeField] Camera playerCamera;
        [SerializeField] float mouseSensitivity = 1f;
        [SerializeField] float controllerSensitivity = 50f;

        [Header("Field of view")]
        [SerializeField] const float NOMAL_FOV = 60f;
     
        [Header("PostProcess")]
        [SerializeField] PostProcessVolume volume;
        [SerializeField] PostProcessProfile standard;
        [SerializeField] PostProcessProfile nightVision;
        [SerializeField] GameObject nightVisionOverlay;
        [SerializeField] GameObject flashLight;

        float targetFov;
        float fov;
        float fovSpeed;
        private float xRotation;

        PlayerInputManager playerInputManager;
        public bool LockCamera { get; set; } = false;

        private void Start()
        {
            playerInputManager = GetComponent<PlayerInputManager>();

            playerCamera.fieldOfView = NOMAL_FOV;
            targetFov = playerCamera.fieldOfView;
            fov = targetFov;

            RenderSettings.fog = true;
            InitPlayerPostProcessSettings();
        }

        private void Update()
        {
            if (LockCamera) return;

            Look();
          
            fov = Mathf.Lerp(fov, targetFov, Time.deltaTime * fovSpeed);
            playerCamera.fieldOfView = fov;
        }

   
        /// <summary>
        /// デフォルト時のカメラのFOVにもどす関数
        /// </summary>
        public void SetNormalFovOfCamera(float fovSpeed)
        {
            this.targetFov = NOMAL_FOV;
            this.fovSpeed = fovSpeed;
        }

        /// <summary>
        ///カメラのFOVを変える関数 
        /// </summary>
        /// <param name="targetFov"></param>
        /// <param name="fovSpeed"></param>
        public void SetFovOfCamera(float targetFov, float fovSpeed)
        {
            this.targetFov = targetFov;
            this.fovSpeed = fovSpeed;
        }

        /// <summary>
        /// カメラの感度と回転を制御する関数
        /// </summary>
        private void Look()
        {
            float mouseX;
            float mouseY;

            //マウス と コントロール でsensitivityを変えること
            float sensitivity = playerInputManager.IsGamepad ? controllerSensitivity : mouseSensitivity;

            mouseX = Mathf.Round(playerInputManager.Look.x) * sensitivity * Time.fixedDeltaTime;
            mouseY = Mathf.Round(playerInputManager.Look.y) * sensitivity * Time.fixedDeltaTime;


            xRotation += mouseY;

            if (xRotation > 90)
            {
                xRotation = 90;
                mouseY = 0;
                ClampXAxisRotationToValue(270);
            }
            else if (xRotation < -90)
            {
                xRotation = -90;
                mouseY = 0;
                ClampXAxisRotationToValue(90);
            }

            playerCamera.transform.Rotate(Vector3.left * mouseY);
            transform.Rotate(Vector3.up * mouseX);
        }

        private void ClampXAxisRotationToValue(float value)
        {
            var eulerRotation = playerCamera.transform.eulerAngles;
            eulerRotation.x = value;
            playerCamera.transform.eulerAngles = eulerRotation;
        }

        public void ChangePostProcess()
        {
            if (volume.profile == standard)
            {
                SwitchNightVision();
            }
            else
            {
                InitPlayerPostProcessSettings();
            }
        }

        public void InitPlayerPostProcessSettings()
        {
            volume.enabled = true;
            volume.profile = standard;
            nightVisionOverlay.SetActive(false);
            flashLight.SetActive(false);
        }

        public void SwitchNightVision()
        {
            volume.profile = nightVision;
            nightVisionOverlay.SetActive(true);
            if (nightVisionOverlay.activeSelf)//バッテリーの充電がないならライトも消える
            {
                flashLight.SetActive(true);
            }
        }



        PlayerEventManager playerEvent;
        public void OnEnable()
        {
            playerEvent = GetComponent<PlayerEventManager>();
            if (playerEvent)
            {
                playerEvent.Subscribe(PlayerEventType.OpenInventory, () => LockCamera = true);
                playerEvent.Subscribe(PlayerEventType.CloseInventory, () => LockCamera = false);
            }
        }

        public void OnDisable()
        {
            if (playerEvent)
            {
                playerEvent.UnSubscribe(PlayerEventType.OpenInventory, () => LockCamera = true);
                playerEvent.UnSubscribe(PlayerEventType.CloseInventory, () => LockCamera = false);
            }
        }
    }
}




using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Musashi
{
    [RequireComponent(typeof(PlayerInputManager))]
    public class PlayerCamaraControl : MonoBehaviour
    {
        [Header("Camera Setting")]
        [SerializeField] Camera playerCamera;
        [SerializeField] float mouseSensitivity = 1f;
        [SerializeField] float controllerSensitivity = 50f;
        [SerializeField] float fovAimingSpeed = 1f;

        [Header("Field Of View")]
        [SerializeField] float NOMAL_FOV = 60f;
        [SerializeField] float AIMING_FOV = 30f;

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
            //if (Input.GetKeyDown(KeyCode.N))
            //    ChangePostProcess();

            fov = Mathf.Lerp(fov, targetFov, Time.deltaTime * fovSpeed);
            playerCamera.fieldOfView = fov;
        }

        public void SetNormalFov(bool isAimCancel = false)
        {
            this.targetFov = NOMAL_FOV;
            if (isAimCancel)
                fovSpeed = fovAimingSpeed;
                //playerCamera.fieldOfView = targetFov;
        }

        public void SetAimingFov()
        {
            this.targetFov = AIMING_FOV;
            fovSpeed = fovAimingSpeed;
            //playerCamera.fieldOfView = AIMING_FOV;
        }

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
                SwitchNightVision();
            else
                InitPlayerPostProcessSettings();
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
                flashLight.SetActive(true);
        }



        PlayerEventManager playerEvent;
        public void OnEnable()
        {
            playerEvent = GetComponent<PlayerEventManager>();
            playerEvent.Subscribe(PlayerEventType.OpenInventory, () => LockCamera = true);
            playerEvent.Subscribe(PlayerEventType.CloseInventory, () => LockCamera = false);
            //GameEventManeger.Instance.Subscribe(EventType.ChangePostProcess, ChangePostProcess);
            //GameEventManeger.Instance.Subscribe(GameEventType.OpenInventory, () => LockCamera = true) ;
            //GameEventManeger.Instance.Subscribe(GameEventType.CloseInventory, () => LockCamera = false);
            
        }

        public void OnDisable()
        {
            playerEvent.UnSubscribe(PlayerEventType.OpenInventory, ()=> LockCamera = true);
            playerEvent.UnSubscribe(PlayerEventType.CloseInventory, () => LockCamera = false);
            //GameEventManeger.Instance.UnSubscribe(GameEventType.ChangePostProcess, ChangePostProcess);
            //GameEventManeger.Instance.UnSubscribe(GameEventType.OpenInventory, () => LockCamera = true);
            //GameEventManeger.Instance.UnSubscribe(GameEventType.CloseInventory, () => LockCamera = false);
        }
    }
}




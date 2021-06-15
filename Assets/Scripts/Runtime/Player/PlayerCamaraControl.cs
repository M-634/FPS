//using System;
//using UnityEngine;
//using UnityEngine.Rendering.PostProcessing;

//namespace Musashi
//{
//    public class PlayerCamaraControl : MonoBehaviour
//    {
//        [Header("Camera base settings")]
//        [SerializeField] Camera playerCamera;
//        [SerializeField] float mouseSensitivity = 1f;
//        [SerializeField] float controllerSensitivity = 50f;

//        [Header("Field of view")]
//        [SerializeField] const float NOMAL_FOV = 60f;

//        float targetFov;
//        float fov;
//        float fovSpeed;
//        private float xRotation;

//        InputProvider inputProvider;
//        public bool LockCamera { get; set; } = false;

//        private void Start()
//        {
//            inputProvider = GetComponentInParent<InputProvider>();

//            playerCamera.fieldOfView = NOMAL_FOV;
//            targetFov = playerCamera.fieldOfView;
//            fov = targetFov;

//           // RenderSettings.fog = true;
//        }

//        private void Update()
//        {
//            if (LockCamera) return;

//            Look();
          
//            fov = Mathf.Lerp(fov, targetFov, Time.deltaTime * fovSpeed);
//            playerCamera.fieldOfView = fov;
//        }

   
//        /// <summary>
//        /// デフォルト時のカメラのFOVにもどす関数
//        /// </summary>
//        public void SetNormalFovOfCamera(float fovSpeed)
//        {
//            this.targetFov = NOMAL_FOV;
//            this.fovSpeed = fovSpeed;
//        }

//        /// <summary>
//        ///カメラのFOVを変える関数 
//        /// </summary>
//        /// <param name="targetFov"></param>
//        /// <param name="fovSpeed"></param>
//        public void SetFovOfCamera(float targetFov, float fovSpeed)
//        {
//            this.targetFov = targetFov;
//            this.fovSpeed = fovSpeed;
//        }

//        /// <summary>
//        /// カメラの感度と回転を制御する関数
//        /// </summary>
//        private void Look()
//        {
//            float mouseX;
//            float mouseY;

//            //マウス と コントロール でsensitivityを変えること
//            float sensitivity = inputProvider.IsGamepad ? controllerSensitivity : mouseSensitivity;

//            mouseX = Mathf.Round(inputProvider.Look.x) * sensitivity * Time.fixedDeltaTime;
//            mouseY = Mathf.Round(inputProvider.Look.y) * sensitivity * Time.fixedDeltaTime;


//            xRotation += mouseY;

//            if (xRotation > 90)
//            {
//                xRotation = 90;
//                mouseY = 0;
//                ClampXAxisRotationToValue(270);
//            }
//            else if (xRotation < -90)
//            {
//                xRotation = -90;
//                mouseY = 0;
//                ClampXAxisRotationToValue(90);
//            }

//            playerCamera.transform.Rotate(Vector3.left * mouseY);
//            transform.Rotate(Vector3.up * mouseX);
//        }

//        private void ClampXAxisRotationToValue(float value)
//        {
//            var eulerRotation = playerCamera.transform.eulerAngles;
//            eulerRotation.x = value;
//            playerCamera.transform.eulerAngles = eulerRotation;
//        }     
//    }
//}




using UnityEngine;
using DG.Tweening;

namespace Musashi
{
    public class PlayerCamaraControl : MonoBehaviour
    {
        [SerializeField] Camera playerCamera;
        [SerializeField] float sensitivity = 50f;
        [SerializeField] float fovGrapplingSpeed = 4f;
        [SerializeField] float fovAimingSpeed = 1f;

        [Header("Field Of View")]
        [SerializeField] float NOMAL_FOV = 60f;
        [SerializeField] float GRAPPLING_FOV = 90f;
        [SerializeField] float AIMING_FOV = 30f;

        float targetFov;
        float fov;
        float fovSpeed;
        private float xRotation;

        private void Start()
        {
            playerCamera.fieldOfView = NOMAL_FOV;
            targetFov = playerCamera.fieldOfView;
            fov = targetFov;
        }

        private void Update()
        {
            Look();

            fov = Mathf.Lerp(fov, targetFov, Time.deltaTime * fovSpeed);
            playerCamera.fieldOfView = fov;
        }

        public void SetNormalFov(bool isAimCancel = false)
        {
            this.targetFov = NOMAL_FOV;
            if (isAimCancel)
                fovSpeed = fovAimingSpeed;
            else
                fovSpeed = fovGrapplingSpeed;
        }

        public void SetGrapplingFov()
        {
            this.targetFov = GRAPPLING_FOV;
            fovSpeed = fovGrapplingSpeed;
        }

        public void SetAimingFov()
        {
            this.targetFov = AIMING_FOV;
            fovSpeed = fovAimingSpeed;
        }

        private void Look()
        {
            float mouseX;
            float mouseY;

            mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.fixedDeltaTime;
            mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.fixedDeltaTime;

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
    }
}

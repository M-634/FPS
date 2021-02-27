using UnityEngine;

namespace Musashi
{
    public class PlayerCamaraControl : MonoBehaviour
    {
        [SerializeField] Camera playerCamera;
        [SerializeField] float sensitivity = 50f;
        [SerializeField] float fovSpeed = 4f;

        float targetFov;
        float fov;
        private float xRotation;

        private void Update()
        {
            Look();

            fov = Mathf.Lerp(fov, targetFov, Time.deltaTime * fovSpeed);
            playerCamera.fieldOfView = fov;
        }

        public void SetCameraFov(float targetFov)
        {
            this.targetFov = targetFov;
        }

        public void InitFov(float nomalFov)
        {
            playerCamera.fieldOfView = nomalFov;
            targetFov = playerCamera.fieldOfView;
            fov = targetFov;
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

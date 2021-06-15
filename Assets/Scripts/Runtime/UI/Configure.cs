using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Musashi
{
    /// <summary>
    ///設定画面を制御するクラス 
    /// </summary>
    public class Configure : MonoBehaviour
    {
        [Header("Opitons scriptable object")]
        [SerializeField] OptionsSOData optionsData;

        [Header("Camera sensitivity settings")]
        [SerializeField] Slider mouseSensitivitySlider;
        [SerializeField] Slider controllerSensitivitySlider;
        [SerializeField] Slider aimingRotationMultipilerSlider;
   
        // player camera settings property
        public float MouseSensitivity { get => optionsData.mouseSensitivity; set => optionsData.mouseSensitivity = value; }
        public float ControllerSensitivity { get => optionsData.controllerSensitivity; set => optionsData.controllerSensitivity = value; }
        public float AimingRotaionMultipiler { get => optionsData.aimingRotaionMultipiler; set => optionsData.aimingRotaionMultipiler = value; }


        private void Start()
        {
            InitCameraSettings();
        }

        /// <summary>
        /// カメラの初期設定
        /// </summary>
        private void InitCameraSettings()
        {
            //設定画面の各種スライダーのイベントを登録する
            mouseSensitivitySlider.SetValueChangedEvent(value => MouseSensitivity = value * OptionsSOData.MAX_MOUSESENCITIVITY);
            controllerSensitivitySlider.SetValueChangedEvent(value => ControllerSensitivity = value * OptionsSOData.MAX_CONTROLLERSENCITICITY);
            aimingRotationMultipilerSlider.SetValueChangedEvent(value => AimingRotaionMultipiler = value * OptionsSOData.MAX_AIMINGROTATIONMULTIPILER);

            //各種設定の初期値を設定する
            mouseSensitivitySlider.SetInitializeSliderValue(MouseSensitivity, OptionsSOData.MIN_MOUSESENCITIVITY, OptionsSOData.MAX_MOUSESENCITIVITY);
            controllerSensitivitySlider.SetInitializeSliderValue(ControllerSensitivity, OptionsSOData.MIN_CONTROLLERSENCITICITY, OptionsSOData.MAX_CONTROLLERSENCITICITY);
            aimingRotationMultipilerSlider.SetInitializeSliderValue(AimingRotaionMultipiler, OptionsSOData.MiN_AIMINGROTATIONMULTIPILER, OptionsSOData.MAX_AIMINGROTATIONMULTIPILER);

        }

        /// <summary>
        /// カメラの各種設定をデフォルトの値にする関数
        /// </summary>
        public void DefultCameraSettings()
        {
            mouseSensitivitySlider.value = OptionsSOData.DEFULT_MOUSESENCITIVITY / OptionsSOData.MAX_MOUSESENCITIVITY;
            controllerSensitivitySlider.value = OptionsSOData.DEFULT_CONTROLLERSENCITICITY / OptionsSOData.MAX_CONTROLLERSENCITICITY;
            aimingRotationMultipilerSlider.value = OptionsSOData.DEFULT_AIMINGROTATIONMULTIPILER / OptionsSOData.MAX_AIMINGROTATIONMULTIPILER;
        }

        public void FullScreen()
        {
            Screen.SetResolution(Screen.width, Screen.height, true);
        }
    }


}

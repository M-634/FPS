using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;

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

        [Header("Other Gameplay settings")]
        [SerializeField] Toggle invertLookYToggle;
        [SerializeField] Toggle displayFramerateCounterToggle;


        // player camera settings property
        public float MouseSensitivity { get => optionsData.mouseSensitivity; set => optionsData.mouseSensitivity = value; }
        public float ControllerSensitivity { get => optionsData.controllerSensitivity; set => optionsData.controllerSensitivity = value; }
        public float AimingRotaionMultipiler { get => optionsData.aimingRotaionMultipiler; set => optionsData.aimingRotaionMultipiler = value; }

        public bool DoInvert_Y { get => optionsData.invert_Y; set => optionsData.invert_Y = value; }
        public bool DoDisplayFrameCounter { get => optionsData.displayFrameCount; set => optionsData.displayFrameCount = value; }

        private void Start()
        {  
            InitGameplaySettings();
        }

        /// <summary>
        /// Gameplay設定を初期化する
        /// </summary>
        private void InitGameplaySettings()
        {
            //ゲームプレイ設定画面の各種スライダーのイベントを登録する
            mouseSensitivitySlider.SetSliderValueChangedEvent(value => MouseSensitivity = value * OptionsSOData.MAX_MOUSESENCITIVITY);
            controllerSensitivitySlider.SetSliderValueChangedEvent(value => ControllerSensitivity = value * OptionsSOData.MAX_CONTROLLERSENCITICITY);
            aimingRotationMultipilerSlider.SetSliderValueChangedEvent(value => AimingRotaionMultipiler = value * OptionsSOData.MAX_AIMINGROTATIONMULTIPILER);

            invertLookYToggle.SetToggleValueChangedEvent(value => DoInvert_Y = value);
            displayFramerateCounterToggle.SetToggleValueChangedEvent(value => DoDisplayFrameCounter = value);

            //各種の初期値を設定する
            mouseSensitivitySlider.SetInitializeSliderValue(MouseSensitivity, OptionsSOData.MIN_MOUSESENCITIVITY, OptionsSOData.MAX_MOUSESENCITIVITY);
            controllerSensitivitySlider.SetInitializeSliderValue(ControllerSensitivity, OptionsSOData.MIN_CONTROLLERSENCITICITY, OptionsSOData.MAX_CONTROLLERSENCITICITY);
            aimingRotationMultipilerSlider.SetInitializeSliderValue(AimingRotaionMultipiler, OptionsSOData.MiN_AIMINGROTATIONMULTIPILER, OptionsSOData.MAX_AIMINGROTATIONMULTIPILER);

            invertLookYToggle.onValueChanged.Invoke(DoInvert_Y);
            invertLookYToggle.isOn = DoInvert_Y;
            displayFramerateCounterToggle.onValueChanged.Invoke(DoDisplayFrameCounter);
            displayFramerateCounterToggle.isOn = DoDisplayFrameCounter;

        }

        /// <summary>
        /// Gameplay各種設定をデフォルトの値にする関数
        /// </summary>
        public void DefultGameplaySettings()
        {
            mouseSensitivitySlider.value = OptionsSOData.DEFULT_MOUSESENCITIVITY / OptionsSOData.MAX_MOUSESENCITIVITY;
            controllerSensitivitySlider.value = OptionsSOData.DEFULT_CONTROLLERSENCITICITY / OptionsSOData.MAX_CONTROLLERSENCITICITY;
            aimingRotationMultipilerSlider.value = OptionsSOData.DEFULT_AIMINGROTATIONMULTIPILER / OptionsSOData.MAX_AIMINGROTATIONMULTIPILER;

            invertLookYToggle.isOn = OptionsSOData.DEFULT_INVERT_Y;
            displayFramerateCounterToggle.isOn = OptionsSOData.DEFULT_DISPLAYFRAMECOUNTER;
        }

        public void FullScreen()
        {
            Screen.SetResolution(Screen.width, Screen.height, true);
        }
    }


}

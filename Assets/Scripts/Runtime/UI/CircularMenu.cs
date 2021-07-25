using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.InputSystem;

namespace Musashi.UI
{
    public class CircularMenu : MonoBehaviour
    {
        [SerializeField] GameObject rootObject = default;
        [SerializeField] RectTransform circularSelectBar = default;
        [SerializeField] float limitMouseMove = 10f;
        [SerializeField] List<MenuButton> buttons;

        private int currentMenuItem;
        private int oldMenuItem;

        public bool IsActive { get; private set; }
        PlayerInputProvider inputProvider;

        private void Start()
        {
            inputProvider = FindObjectOfType<PlayerInputProvider>();//test     
            inputProvider.PlayerInputActions.SwitchCycleWeapon.performed +=
                ctx =>
                {
                    if (GameManager.Instance.ShowConfig) return;
                    GetActive(true);
                };
            inputProvider.PlayerInputActions.SwitchCycleWeapon.canceled += ctx => GetActive(false);

            foreach (var button in buttons)
            {
                button.sceneImage.color = button.normalColor;
            }
            GetActive(false);
        }

        private void Update()
        {
            if (!IsActive) return;
            if (GameManager.Instance.ShowConfig) return;

            GetCurrentMenuItem();
            if (inputProvider.OnSelect)//test
            {
                ButtonAcion();
            }
        }

        /// <summary>
        /// マウスポインタが移動したベクトル量から角度を計算し、選択するメニューボタンを決める関数
        /// </summary>
        public void GetCurrentMenuItem()
        {
            if (buttons.Count < 3) return;

            //set cursor mode
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }

            //calculate...
            var readVector = inputProvider.InputReadVector;
            if (readVector.magnitude < limitMouseMove) return;

            var angle = Mathf.Atan2(readVector.y, readVector.x) * Mathf.Rad2Deg;
            if (angle < 0) angle += 360;

            currentMenuItem = (int)angle / (360 / buttons.Count);
            var temp = circularSelectBar.localEulerAngles;
            temp.z = angle;
            circularSelectBar.localEulerAngles = temp;

            if (currentMenuItem != oldMenuItem)
            {
                buttons[oldMenuItem].UnSelected();
                buttons[currentMenuItem].OnSelected();
                oldMenuItem = currentMenuItem;
            }
        }

        public void ButtonAcion()
        {
            buttons[currentMenuItem].sceneImage.color = buttons[currentMenuItem].pressedColor;
            Debug.Log("you have pressed button");
            GetActive(false);
        }

        public void GetActive(bool value)
        {
            if (!rootObject)
            {
                rootObject = this.gameObject;
            }
            rootObject.SetActive(value);
            IsActive = value;
            GameManager.Instance.CanProcessPlayerMoveInput = !value;
        }
    }

    [Serializable]
    public class MenuButton
    {
        public RectTransform rectTransform;
        public float highLightedScaleMultipier;
        public Image sceneImage;
        public Color normalColor = Color.white;
        public Color highLightedColor = Color.gray;
        public Color pressedColor = Color.gray;

        public void OnSelected()
        {
            sceneImage.color = highLightedColor;
            rectTransform.localScale *= highLightedScaleMultipier;
        }

        public void UnSelected()
        {
            sceneImage.color = normalColor;
            rectTransform.localScale = Vector3.one;
        }
    }
}

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
        [SerializeField] GameObject rootObject;
        [SerializeField] float offestAngle;
        [SerializeField] List<MenuButton> buttons = new List<MenuButton>();

        private float angle;
        private int currentMenuItem;
        private int oldMenuItem;

        private Vector2 beforeMousePosition;
        private readonly Vector2 center = new Vector2(Screen.width / 2, Screen.height / 2);

        PlayerInputProvider inputProvider;

        private void Start()
        {
            inputProvider = FindObjectOfType<PlayerInputProvider>();//test     

            foreach (var button in buttons)
            {
                button.sceneImage.color = button.normalColor;
            }
            Debug.Log(center);
        }

        private void Update()
        {
            if (GameManager.Instance.ShowConfig) return;

            GetCurrentMenuItem();
            //if (inputProvider.Fire)//test
            //{
            //    ButtonAcion();
            //}
        }

        public void GetCurrentMenuItem()
        {
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Confined;
                GameManager.Instance.CanProcessInput = true;
            }

            var currentMousePosition = inputProvider.GetMousePosition;

            //ŽžŒv‰ñ‚è : angle < 0 
            angle = (int)(Mathf.Atan2(currentMousePosition.y - center.y, currentMousePosition.x - center.x) - Mathf.Atan2(beforeMousePosition.y - center.y, beforeMousePosition.x - center.x)) * Mathf.Rad2Deg;

            if (Mathf.Abs(angle) < offestAngle) return;

            if (angle < 0)
            {
                currentMenuItem = (currentMenuItem + 1) % buttons.Count;
            }
            else
            {
                currentMenuItem--;
                if (currentMenuItem < 0)
                {
                    currentMenuItem = buttons.Count - 1;
                }
            }

            if (currentMenuItem != oldMenuItem)
            {
                buttons[oldMenuItem].UnSelected();
                buttons[currentMenuItem].OnSelected();
                oldMenuItem = currentMenuItem;
            }
            beforeMousePosition = currentMousePosition;
        }

        public void ButtonAcion()
        {
            buttons[currentMenuItem].sceneImage.color = buttons[currentMenuItem].pressedColor;
            Debug.Log("you have pressed button");

            //complete ..
            GetActive(false);
        }

        public void GetActive(bool value)
        {
            if (!rootObject)
            {
                rootObject = this.gameObject;
            }
            rootObject.SetActive(value);

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

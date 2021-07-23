using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace Musashi.UI
{
    public class CircularMenu : MonoBehaviour
    {
        public List<MenuButton> buttons = new List<MenuButton>();
        private Vector2 mousePosition;
        private Vector2 fromVector2M = new Vector2(0.5f, 1.0f);
        private Vector2 centorcircle = new Vector2(0.5f, 0.5f);
        private Vector2 toVector2M;

        public int menuItem;
        public int currentMenuItem;
        private int oldMenuItem;

        PlayerInputProvider inputProvider;

        private void Start()
        {
            menuItem = buttons.Count;
            inputProvider = FindObjectOfType<PlayerInputProvider>();//test
        }

        private void Update()
        {
            GetCurrentMenuItem();
            if (inputProvider.Fire)//test
            {
                ButtonAcion();
            }
        }

        public void GetCurrentMenuItem()
        {
            mousePosition = new Vector2(inputProvider.GetLookInputsHorizontal, inputProvider.GetLookInputVertical);

            toVector2M = new Vector2(mousePosition.x / Screen.width, mousePosition.y / Screen.height);

            float angle = (Mathf.Atan2(fromVector2M.y - centorcircle.y, fromVector2M.x - centorcircle.x) - Mathf.Atan2(toVector2M.y - centorcircle.y, toVector2M.x - centorcircle.x)) * Mathf.Rad2Deg;

            if (angle < 0) angle += 360;

            currentMenuItem = (int)angle / (360 / menuItem);

            if(currentMenuItem != oldMenuItem)
            {
                buttons[oldMenuItem].sceneImage.color = buttons[oldMenuItem].normalColor;
                buttons[currentMenuItem].sceneImage.color = buttons[currentMenuItem].highLightedColor;
                oldMenuItem = currentMenuItem;
            }
        }

        public void ButtonAcion()
        {
            buttons[currentMenuItem].sceneImage.color = buttons[currentMenuItem].pressedColor;
            if(currentMenuItem == 0)
            {
                Debug.Log("you have pressed first button");
            }
        }
    }

    [Serializable]
    public class MenuButton 
    {
        public string name;
        public Image sceneImage;
        public Color normalColor = Color.white;
        public Color highLightedColor = Color.gray;
        public Color pressedColor = Color.gray;
    }
}

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Musashi.UI
{
    public class CircularMenu : MonoBehaviour
    {
        [SerializeField] GameObject rootObject = default;
        [SerializeField] RectTransform circularSelectBar = default;
        [SerializeField] float limitMouseMove = 10f;
        [SerializeField] List<MenuButton> buttons;

        private int nextIndex;
        private int currentMenuItem;
        private int oldMenuItem;

        public bool IsActive { get; private set; }
        public List<MenuButton> ButtonList => buttons;
        PlayerInputProvider inputProvider;
        public event Action<int> OnSelectAction;


        public void AddInfoInButton(string itemName, int stackSize, Sprite icon = null)
        {
            if (nextIndex == buttons.Count) return;

            buttons[nextIndex].SetInfo(itemName, nextIndex, stackSize, icon);
            nextIndex++;

            if (nextIndex >= buttons.Count)
            {
                nextIndex = buttons.Count;
            }
        }

        public void UptateInfo(int index, int stackSize)
        {
            buttons[index].UpdateStackSize(stackSize);
        }

        private void Start()
        {
            inputProvider = FindObjectOfType<PlayerInputProvider>();//test     
            inputProvider.PlayerInputActions.SwitchCycleWeapon.performed +=
                ctx =>
                {
                    if (GameManager.Instance.ShowConfig) return;
                    GetActive(true);
                };
            inputProvider.PlayerInputActions.SwitchCycleWeapon.canceled +=
                ctx =>
                {
                    GetActive(false);
                    OnSelectAction.Invoke(currentMenuItem);
                };

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
        [Header("Button settings")]
        public RectTransform rectTransform;
        public float highLightedScaleMultipier;
        public Image sceneImage;
        public Color normalColor = Color.white;
        public Color highLightedColor = Color.gray;
        public Color pressedColor = Color.gray;

        [Header("Set item info")]
        public string itemName;
        public int index;
        public Image image;
        public TextMeshProUGUI stackSizeText;


        public int OnSelected()
        {
            sceneImage.color = highLightedColor;
            rectTransform.localScale *= highLightedScaleMultipier;
            return index;
        }

        public void UnSelected()
        {
            sceneImage.color = normalColor;
            rectTransform.localScale = Vector3.one;
        }

        public void SetInfo(string itemName, int index, int stackSize, Sprite icon = null)
        {
            this.itemName = itemName;
            this.index = index;
            image.sprite = icon;
            UpdateStackSize(stackSize);
        }

        public void UpdateStackSize(int stackSize)
        {
            stackSizeText.text = stackSize.ToString();
        }
    }
}

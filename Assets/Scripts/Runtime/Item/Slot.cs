using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace Musashi.Item
{
    public class Slot : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI stackSizeText;
        [SerializeField] Image imageIcon;
        public string GUID { get; private set; }

        public void SetItemInfo(string guid, int stackSize, Sprite icon = null)
        {
            GUID = guid;
            if (imageIcon)
            {
                imageIcon.sprite = icon;
            }
            UpdateStackSizeText(stackSize);
        }

        public void UpdateStackSizeText(int value)
        {

            stackSizeText.text = value.ToString();
        }

        public void ClearSlot()
        {
            GUID = "";
            imageIcon.sprite = null;
            stackSizeText.text = "";
        }
    }
}

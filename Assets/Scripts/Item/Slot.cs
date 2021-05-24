using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace Musashi
{

    public class Slot : MonoBehaviour
    {
        [SerializeField] Image icon;
        [SerializeField] Image Outline;
        [SerializeField] TextMeshProUGUI keyCode;
        [SerializeField] TextMeshProUGUI stackSize;
        [SerializeField] Color highLightColor;
        [SerializeField] Color defultColor;

        public Image Icon => icon;
        public TextMeshProUGUI StackSize => stackSize;

        private void Awake()
        {
            Outline.enabled = false;
            Outline.color = highLightColor;
        }

        public void OnSelected()
        {
            Outline.enabled = true;
            Outline.color = highLightColor;
        }

        public void MissingSelection()
        {
            Outline.enabled = false;
            Outline.color = defultColor;
        }
    }
}

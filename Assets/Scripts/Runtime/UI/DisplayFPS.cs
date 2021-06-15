using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Musashi
{
    public class DisplayFPS : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI fpsText;
        int frameCount;
        float elapsedTime;
        bool canDisplay;

        void Update()
        {
            if (!canDisplay) return;

            frameCount++;
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= 1.0f)
            {
                float fps = 1.0f * frameCount / elapsedTime;
                if (fpsText)
                {

                }
                fpsText.text = $"FPS : {fps:F2}";

                frameCount = 0;
                elapsedTime = 0f;
            }
        }

        private void OnEnable()
        {
            GameManager.Instance.Configure.OnDisplayFramerateCounterToggleEvent += value =>
            {
                canDisplay = value;
                if (!value)
                {
                    fpsText.text = "";
                }
            };
        }

        private void OnDisable()
        {
            GameManager.Instance.Configure.OnDisplayFramerateCounterToggleEvent -= value =>
            {
                canDisplay = value;
                if (!value)
                {
                    fpsText.text = "";
                }
            };
        }
    }
}

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

        void Update()
        {
            if (!GameManager.Instance.Configure.DoDisplayFrameCounter)
            {
                frameCount = 0;
                elapsedTime = 0f;
                fpsText.text = "";
                return;
            }

            frameCount++;
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= 1.0f)
            {
                float fps = 1.0f * frameCount / elapsedTime;
                fpsText.text = $"FPS : {fps:F2}";

                frameCount = 0;
                elapsedTime = 0f;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayFPS : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI fpsText;
    int frameCount;
    float elapsedTime;

    // Update is called once per frame
    void Update()
    {
        frameCount++;
        elapsedTime += Time.deltaTime;

        if(elapsedTime >= 1.0f)
        {
            float fps = 1.0f * frameCount / elapsedTime;
            fpsText.text = $"FPS : {fps:F2}";

            frameCount = 0;
            elapsedTime = 0f;
        }
    }
}

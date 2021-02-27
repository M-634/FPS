using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public partial class DisplayFPS : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI fpsText;
    int frameCount;
    float elapsedTime;

    // Start is called before the first frame update
    void Start()
    {
        //Application.targetFrameRate = 90;
    }

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

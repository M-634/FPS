using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
///設定画面を制御するクラス 
/// </summary>
public class Configure : MonoBehaviour
{

    public void FullScreen()
    {
        Screen.SetResolution(Screen.width, Screen.height, true);
    }
}

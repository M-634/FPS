using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// オプション画面で設定できる値をまとめたクラス。
/// スクリプタブルオブジェクトとして保存しておく。
/// </summary>
[Serializable]
[CreateAssetMenu(fileName = "OptionsData", menuName = "OptionsSettingData")]
public class OptionsSOData :ScriptableObject
{
    [Header("Player Camer Settings")]
    [Range(0.01f,0.1f)] 
    public float mouseSensitivity = 0.05f;
    [Range(1f, 10f)]
    public float controllerSensitivity = 5f;
    [Range(0.1f,1f)] 
    public float aimingRotaionMultipiler = 0.4f;

    [Header("Other Gameplay Settings")]
    public bool invert_Y = false;
    public bool displayFrameCount = true;

    #region const variables
    public const float MAX_MOUSESENCITIVITY = 0.1f;
    public const float MIN_MOUSESENCITIVITY = 0.01f;
    public const float DEFULT_MOUSESENCITIVITY = 0.05f;

    public const float MAX_CONTROLLERSENCITICITY = 10f;
    public const float MIN_CONTROLLERSENCITICITY = 1f;
    public const float DEFULT_CONTROLLERSENCITICITY = 5f;

    public const float MAX_AIMINGROTATIONMULTIPILER = 1f;
    public const float MiN_AIMINGROTATIONMULTIPILER = 0.1f;
    public const float DEFULT_AIMINGROTATIONMULTIPILER = 0.4f;

    public const bool DEFULT_INVERT_Y = false;
    public const bool DEFULT_DISPLAYFRAMECOUNTER = true;
    #endregion


    //audio

    //graphical
}

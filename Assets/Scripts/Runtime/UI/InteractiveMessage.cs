using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

/// <summary>
/// ゲームシーンで、プレイヤーへのメッセージ表示を管理するクラス
/// (staticを修正して、イベントでできるようにすること)
/// </summary>
[RequireComponent(typeof(TextMeshProUGUI))]
public class InteractiveMessage : MonoBehaviour
{
    static TextMeshProUGUI textMeshProUGUI;

    //Interactive message
    public const string InteractiveText = "Press E to Pickup";

    //warning message
    public const string FullStackItem = "This Item is Full";
    public const string FullSlotItem = "Full Slot Inventory";
    public const string NoBatteryText = "No Battery !!";
    public const string HPISFull = "HP is Full !";


    static bool isDisplayInteractiveMessage = false;
    private void Start()
    {
        textMeshProUGUI = GetComponent<TextMeshProUGUI>();
    }

    public static void ShowInteractiveMessage(string message)//updata内で呼ばれる
    { 
        textMeshProUGUI.text = message;
        isDisplayInteractiveMessage = true;
    }

    public static void WarningMessage(string message)//一時的に呼ばれる
    {
        isDisplayInteractiveMessage = false;
        textMeshProUGUI.text = message;
        textMeshProUGUI.DOColor(Color.clear, 1f)
            .SetEase(Ease.Linear)
            .OnComplete(() => 
            {
                textMeshProUGUI.text = "";
                textMeshProUGUI.color = Color.white;
            });
    }

    public static void CloseMessage()
    {
        if (!isDisplayInteractiveMessage) return;
        textMeshProUGUI.text = "";
        isDisplayInteractiveMessage = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class InteractiveMessage : MonoBehaviour
{
    static TextMeshProUGUI textMeshProUGUI;

    public const string InteractiveText = "Press E to Pickup";
    public const string FullStackItem = "This Item is Full";
    public const string FullSlotItem = "Full Slot Inventory";

    private void Start()
    {
        textMeshProUGUI = GetComponent<TextMeshProUGUI>();
    }

    public static void ShowMessage(string message)
    {
        textMeshProUGUI.text = message;
    }

    public static void CloseMessage()
    {
        textMeshProUGUI.text = "";
    }
}

using UnityEditor;
using UnityEngine;

namespace Musashi.Item
{
    [CustomEditor(typeof(ItemSettingSOData))]
    public class ItemDataCustomInspector : Editor
    {
        ItemSettingSOData itemSetting;

        private void OnEnable()
        {
            itemSetting = target as ItemSettingSOData;
        }

        public override void OnInspectorGUI()
        {
            if (!itemSetting) return;
            DrawGeneralItem();
            EditorUtility.SetDirty(itemSetting);
        }

        private void DrawGeneralItem()
        {
            GUILayout.Label("General item settings", EditorStyles.boldLabel);
            GUILayout.BeginVertical("HelpBox");
            itemSetting.itemName = EditorGUILayout.TextField("Name", itemSetting.itemName);
            itemSetting.description = EditorGUILayout.TextField("Description", itemSetting.description);
            itemSetting.itemType = (ItemType)EditorGUILayout.EnumPopup("Item type", itemSetting.itemType);
            itemSetting.icon = (Sprite)EditorGUILayout.ObjectField("Item icon", itemSetting.icon, typeof(Sprite), false);

            itemSetting.stackable = EditorGUILayout.Toggle("Stackable", itemSetting.stackable);

            EditorGUILayout.HelpBox("Weapons using stacksize as ammo capacity. Max is the total capacity, stacksize is the current", MessageType.Info);
            itemSetting.stackSize = EditorGUILayout.IntSlider("Item stack size", itemSetting.stackSize, 1, 100);
            itemSetting.maxStackSize = EditorGUILayout.IntSlider("Max stack size", itemSetting.maxStackSize, 1, 999);

            GUILayout.EndVertical();
        }
    }
}
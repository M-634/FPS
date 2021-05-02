using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Musashi
{
    [CustomEditor(typeof(WeaponActiveControl))]
    public class WeaponActiveControlCustomInspector : Editor
    {
        WeaponActiveControl activeControl;

        private void OnEnable()
        {
            activeControl = target as WeaponActiveControl;
        }

        public override void OnInspectorGUI()
        {
            if (!activeControl) return;

            base.OnInspectorGUI();
            DrawSetting();

            EditorUtility.SetDirty(activeControl);
        }

        private void DrawSetting()
        {
            if (GUILayout.Button("SetActive"))
            {
                activeControl.SetActive(!activeControl.ActiveSelf);
            }
        }
    }
}

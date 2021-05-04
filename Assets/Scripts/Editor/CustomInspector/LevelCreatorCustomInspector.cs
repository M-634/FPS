using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Musashi.Level;

[CustomEditor(typeof(LevelCreator))]
public class LevelCreatorCustomInspector : Editor
{
    LevelCreator levelCreator;

    private void OnEnable()
    {
        levelCreator = target as LevelCreator;
    }

    public override void OnInspectorGUI()
    {
        if (!levelCreator) return;
        base.OnInspectorGUI();

        if(GUILayout.Button("Create Level"))
            levelCreator.CreateDungeon();
        if (GUILayout.Button("Delete Level"))
            levelCreator.DeletDungeon();

        EditorUtility.SetDirty(levelCreator);
    }
}

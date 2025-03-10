using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DiceDisplayManager))]
public class DiceDisplayManagerEditor : Editor
{
    private SerializedProperty diceListProperty;

    private void OnEnable()
    {
        diceListProperty = serializedObject.FindProperty("diceList");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawDefaultInspector();

        DiceDisplayManager manager = (DiceDisplayManager)target;

        if (GUILayout.Button("Display Dice"))
        {
            manager.DisplayDice(manager.diceList);
        }

        if (GUILayout.Button("Clear Dice"))
        {
            manager.ClearDice();
        }

        serializedObject.ApplyModifiedProperties();
    }
}

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(OpponentDiceDisplayManager))]
public class OpponentDiceDisplayManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        OpponentDiceDisplayManager manager = (OpponentDiceDisplayManager)target;

        EditorGUILayout.Space();

        if (GUILayout.Button("Generate Opponent Dice"))
        {
            manager.GenerateOpponentDice();
        }

        if (GUILayout.Button("Clear Opponent Dice"))
        {
            manager.ClearDice();
            EditorUtility.SetDirty(target);
        }
    }
}
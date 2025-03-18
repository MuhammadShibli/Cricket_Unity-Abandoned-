using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DiceSO))]
public class DiceSOEditor : Editor
{
    private Sprite[] sprites;
    private FaceSO[] faceAssets;
    private GUIContent[] faceDropdownContent;
    private const float FACE_PREVIEW_SIZE = 64f;
    private const float PADDING = 5f;

    private void OnEnable()
    {
        string spritePath = "Assets/Resources/cricket_SS.png";
        sprites = AssetDatabase.LoadAllAssetsAtPath(spritePath).OfType<Sprite>().ToArray();
       // Debug.Log($"Loaded {sprites.Length} sprites from {spritePath}");

        // Load all FaceSO assetos
        var faceGuids = AssetDatabase.FindAssets("t:FaceSO");
        faceAssets = new FaceSO[faceGuids.Length];
        faceDropdownContent = new GUIContent[faceGuids.Length];

        for (int i = 0; i < faceGuids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(faceGuids[i]);
            FaceSO face = AssetDatabase.LoadAssetAtPath<FaceSO>(path);
            faceAssets[i] = face;
            faceDropdownContent[i] = new GUIContent($"{face.faceLabel} - {face.description}");
        }
    }

    public override void OnInspectorGUI()
    {
        DiceSO diceSO = (DiceSO)target;

        // Draw non-array properties
        SerializedObject serializedObject = new SerializedObject(target);

        // Draw diceId
        SerializedProperty idProp = serializedObject.FindProperty("diceId");
        EditorGUILayout.PropertyField(idProp);

        // Draw diceType
        SerializedProperty typeProp = serializedObject.FindProperty("diceType");
        EditorGUILayout.PropertyField(typeProp);

        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Face Assignments", EditorStyles.boldLabel);

        // Draw face assignments with dropdowns
        EditorGUI.indentLevel++;
        for (int faceIndex = 0; faceIndex < 6; faceIndex++)
        {
            EditorGUILayout.BeginHorizontal();

            FaceSO currentFace = diceSO.faces[faceIndex];
            int selectedIndex = -1;

            if (currentFace != null)
            {
                selectedIndex = System.Array.IndexOf(faceAssets, currentFace);
            }

            if (selectedIndex >= 0)
            {
                DrawFacePreview(faceAssets[selectedIndex].spriteName);
            }

            EditorGUILayout.LabelField($"Face {faceIndex + 1}:", GUILayout.Width(60));
            int newSelectedIndex = EditorGUILayout.Popup(selectedIndex,
                faceDropdownContent.Select(c => c.text).ToArray());

            if (newSelectedIndex != selectedIndex && newSelectedIndex >= 0)
            {
                Undo.RecordObject(diceSO, "Change Face");
                diceSO.faces[faceIndex] = faceAssets[newSelectedIndex];
                EditorUtility.SetDirty(diceSO);
            }

            EditorGUILayout.EndHorizontal();
        }
        EditorGUI.indentLevel--;
    }

    private void DrawFacePreview(string spriteName)
    {
        if (sprites == null)
        {
            Debug.LogWarning($"Sprites array is null");
            GUILayout.Box(spriteName, GUILayout.Width(FACE_PREVIEW_SIZE ), GUILayout.Height(FACE_PREVIEW_SIZE));
            return;
        }

        Sprite sprite = sprites.FirstOrDefault(s => s.name == spriteName);
        if (sprite == null)
        {
            Debug.LogWarning($"Sprite with name {spriteName} not found");
            GUILayout.Box(spriteName, GUILayout.Width(FACE_PREVIEW_SIZE), GUILayout.Height(FACE_PREVIEW_SIZE));
            return;
        }

        // Calculate preview dimensions
        float aspectRatio = sprite.rect.width / sprite.rect.height;
        float width = FACE_PREVIEW_SIZE;
        float height = width / aspectRatio;

        Rect previewRect = EditorGUILayout.GetControlRect(false, height, GUILayout.Width(width));

        // Draw Image
        Rect uv = new Rect(
            sprite.rect.x / sprite.texture.width,
            sprite.rect.y / sprite.texture.height,
            sprite.rect.width / sprite.texture.width,
            sprite.rect.height / sprite.texture.height
        );
        GUI.DrawTextureWithTexCoords(previewRect, sprite.texture, uv, true);

        // Tooltip
        EditorGUI.LabelField(previewRect, new GUIContent("", spriteName));
    }
    
}

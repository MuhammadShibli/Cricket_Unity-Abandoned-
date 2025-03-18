using UnityEditor;
using UnityEngine;
using System.Linq;

[CustomEditor(typeof(CricketerSO))]
public class CricketerSOEditor : Editor
{
    private Sprite[] sprites;
    private const float FACE_PREVIEW_SIZE = 32f;
    private const float CRICKETER_PREVIEW_SIZE = 100f;
    private const float PADDING = 5f;
    private GUIStyle headerStyle;

    private void OnEnable()
    {
        string spritePath = "Assets/Resources/cricket_SS.png";
        sprites = AssetDatabase.LoadAllAssetsAtPath(spritePath).OfType<Sprite>().ToArray();

    }

    public override void OnInspectorGUI()
    {


        headerStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 12,
            margin = new RectOffset(0, 0, 10, 5)
        };

        CricketerSO cricketerSO = (CricketerSO)target;

        // Draw Cricketer Image Preview
        if (cricketerSO.Image != null)
        {
            EditorGUILayout.Space(10);

            // Calculate aspect ratio for proper display
            float aspect = cricketerSO.Image.rect.width / cricketerSO.Image.rect.height;
            float previewWidth = CRICKETER_PREVIEW_SIZE;
            float previewHeight = CRICKETER_PREVIEW_SIZE / aspect;

            // Center the preview horizontally
            float centerX = EditorGUILayout.GetControlRect().xMin + (EditorGUIUtility.currentViewWidth - previewWidth) * 0.5f;

            // Create preview rect
            Rect previewRect = new Rect(centerX, GUILayoutUtility.GetRect(previewWidth, previewHeight).y,
                                      previewWidth, previewHeight);

            // Draw the Image
            EditorGUI.DrawPreviewTexture(previewRect, cricketerSO.Image.texture,
                                       null, ScaleMode.ScaleToFit);

            EditorGUILayout.Space(10);
        }
        else
        {
            EditorGUILayout.HelpBox("No Cricketer Image assigned", MessageType.Info);
            EditorGUILayout.Space(5);
        }

        // Display default inspector
        DrawDefaultInspector();


        EditorGUILayout.Space(15);

        // Draw previews for each dice category
        DrawDiceCategoryPreviews("Special Dice", cricketerSO.specialDice);
        EditorGUILayout.Space(10);
        DrawDiceCategoryPreviews("Normal Dice", cricketerSO.normalDice);
        EditorGUILayout.Space(10);
        DrawDiceCategoryPreviews("Talent Dice", cricketerSO.talentDice);

        if (GUI.changed)
        {
            EditorUtility.SetDirty(cricketerSO);
        }
    }

    private void DrawDiceCategoryPreviews(string categoryName, DiceSO[] diceArray)
    {
        EditorGUILayout.LabelField(categoryName, headerStyle);
        EditorGUI.indentLevel++;

        for (int diceIndex = 0; diceIndex < diceArray.Length; diceIndex++)
        {
            DiceSO dice = diceArray[diceIndex];
            if (dice == null)
            {
                EditorGUILayout.HelpBox($"Dice {diceIndex + 1}: Not Assigned", MessageType.Info);
                continue;
            }

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField($"Dice {diceIndex + 1}: {dice.diceId} ({dice.diceType})", EditorStyles.boldLabel);

            // Draw faces preview grid
            EditorGUILayout.BeginHorizontal();
            for (int faceIndex = 0; faceIndex < dice.faces.Length; faceIndex++)
            {
                FaceSO face = dice.faces[faceIndex];
                if (face == null)
                {
                    GUILayout.Box("Empty", GUILayout.Width(FACE_PREVIEW_SIZE), GUILayout.Height(FACE_PREVIEW_SIZE));
                    continue;
                }

                DrawFacePreview(face);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);
        }

        EditorGUI.indentLevel--;
    }

    private void DrawFacePreview(FaceSO face)
    {
        if (string.IsNullOrEmpty(face.spriteName) || sprites == null) 
        {
            GUILayout.Box(face.faceId, GUILayout.Width(FACE_PREVIEW_SIZE), GUILayout.Height(FACE_PREVIEW_SIZE));
            return;
        }

        Sprite sprite = sprites.FirstOrDefault(s => s.name == face.spriteName);
        if (sprite == null)
        {
            GUILayout.Box(face.faceId, GUILayout.Width(FACE_PREVIEW_SIZE), GUILayout.Height(FACE_PREVIEW_SIZE));
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
        EditorGUI.LabelField(previewRect, new GUIContent("", $"{face.faceId}\n{face.description}"));
    }
}

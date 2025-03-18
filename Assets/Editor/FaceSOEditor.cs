using Codice.Client.Common.FsNodeReaders;
using NUnit.Framework;
using System;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FaceSO))]
public class FaceSOEditor : Editor
{
    private Sprite[] sprites;
    private string[] spriteNames;
    private const string SPRITE_PATH = "Assets/Resources/cricket_SS.png";

    private void OnEnable()
    {
        LoadSprites();
    }

    private void LoadSprites()
    {
        // Verify the asset exists
        if (!AssetDatabase.LoadAssetAtPath<Texture2D>(SPRITE_PATH))
        {
            Debug.LogError($"Sprite sheet not found at path: {SPRITE_PATH}");
            return;
        }

        // Load all assets and filter for sprites
        var assets = AssetDatabase.LoadAllAssetsAtPath(SPRITE_PATH);
        sprites = assets.OfType<Sprite>().ToArray();

        if (sprites == null || sprites.Length == 0)
        {
            Debug.LogError("No sprites found in Image sheet. Ensure the texture is set up as a Image sheet with multiple sprites.");
            return;
        }

        spriteNames = sprites.Select(s => s.name).ToArray();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        FaceSO faceSO = (FaceSO)target;

        // Display the default inspector
        DrawDefaultInspector();

        // Check if sprites are loaded
        if (sprites == null || sprites.Length == 0 || spriteNames == null || spriteNames.Length == 0)
        {
            EditorGUILayout.HelpBox("No sprites loaded. Check console for errors.", MessageType.Error);
            if (GUILayout.Button("Reload Sprites"))
            {
                LoadSprites();
            }
            return;
        }

        // Dropdown for selecting Image name
        EditorGUI.BeginChangeCheck();
        int selectedIndex = Array.IndexOf(spriteNames, faceSO.spriteName);
        if (selectedIndex == -1) selectedIndex = 0;

        selectedIndex = EditorGUILayout.Popup("Sprite Name", selectedIndex, spriteNames);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(faceSO, "Change Face Sprite");
            faceSO.spriteName = spriteNames[selectedIndex];
            EditorUtility.SetDirty(faceSO);
        }

        // Display Image preview
        if (!string.IsNullOrEmpty(faceSO.spriteName))
        {
            Sprite sprite = sprites.FirstOrDefault(s => s.name == faceSO.spriteName);
            if (sprite != null)
            {
                GUILayout.Space(5);

                // Calculate preview size while maintaining aspect ratio
                float aspectRatio = sprite.rect.width / sprite.rect.height;
                float previewSize = 64f;
                float width = previewSize;
                float height = previewSize;

                if (aspectRatio > 1)
                    height = width / aspectRatio;
                else
                    width = height * aspectRatio;

                Rect previewRect = GUILayoutUtility.GetRect(width, height);

                // Center the preview
                float xOffset = (previewRect.width - width) * 0.5f;
                previewRect.x += xOffset;
                previewRect.width = width;
                previewRect.height = height;

                // Draw only the selected Image
                GUI.DrawTextureWithTexCoords(
                    previewRect,
                    sprite.texture,
                    new Rect(
                        sprite.textureRect.x / sprite.texture.width,
                        sprite.textureRect.y / sprite.texture.height,
                        sprite.textureRect.width / sprite.texture.width,
                        sprite.textureRect.height / sprite.texture.height
                    )
                );
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}

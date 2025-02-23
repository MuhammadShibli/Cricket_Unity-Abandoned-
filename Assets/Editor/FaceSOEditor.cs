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

    private void OnEnable()
    {

        string spritePath = "Assets/Resources/cricket_SS.png";
        // Load all sprites from the spritesheet
        sprites = AssetDatabase.LoadAllAssetsAtPath(spritePath).OfType<Sprite>().ToArray();
        spriteNames = sprites.Select(s => s.name).ToArray();
    }

    public override void OnInspectorGUI()
    {
        FaceSO faceSO = (FaceSO)target;

        // Display the default inspector
        DrawDefaultInspector();

        // Display the rich text with the sprite
        EditorGUILayout.LabelField("Rich Text with Sprite", faceSO.GetRichTextWithSprite());

        // Dropdown for selecting sprite name
        int selectedIndex = Array.IndexOf(spriteNames, faceSO.spriteName);
        if (selectedIndex == -1) selectedIndex = 0;
        selectedIndex = EditorGUILayout.Popup("Sprite Name", selectedIndex, spriteNames);
        faceSO.spriteName = spriteNames[selectedIndex];

        // Display sprite preview
        if (!string.IsNullOrEmpty(faceSO.spriteName) && sprites != null)
        {
            Sprite sprite = sprites.FirstOrDefault(s => s.name == faceSO.spriteName);
            if (sprite != null)
            {
                // Calculate the aspect ratio and dimensions
                float aspectRatio = sprite.rect.width / sprite.rect.height;
                float maxHeight = 64f;
                float maxWidth = EditorGUIUtility.currentViewWidth - 40f; // Account for margins

                float width = Mathf.Min(maxHeight * aspectRatio, maxWidth);
                float height = width / aspectRatio;

                // Center the sprite preview
                Rect spriteRect = EditorGUILayout.GetControlRect(false, height);
                spriteRect.width = width;
                spriteRect.x = (EditorGUIUtility.currentViewWidth - width) * 0.5f;

                Rect uv = new Rect(
                    sprite.rect.x / sprite.texture.width,
                    sprite.rect.y / sprite.texture.height,
                    sprite.rect.width / sprite.texture.width,
                    sprite.rect.height / sprite.texture.height
                );
                GUI.DrawTextureWithTexCoords(spriteRect, sprite.texture, uv, true); // Added alphaBlend parameter
            }
            else
            {
                EditorGUILayout.HelpBox($"Sprite '{faceSO.spriteName}' not found in spritesheet.", MessageType.Warning);
            }
        }

        // Apply changes
        if (GUI.changed)
        {
            EditorUtility.SetDirty(faceSO);
        }
    }
}

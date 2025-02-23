using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "Face", menuName = "Cricket/Face")]
public class FaceSO : ScriptableObject
{
    public string faceId;
    public string symbol;
    public string description;
    [HideInInspector] public string spriteName; // Name of the sprite for rich text
    // Method to apply the face's effect
    public virtual void ApplyEffect()
    {
        // Implementation based on face type
        Debug.Log($"Applying effect: {description}");
    }

    // Method to get the rich text with the sprite
    public string GetRichTextWithSprite()
    {
        return $"<sprite name=\"{spriteName}\">";
    }
}

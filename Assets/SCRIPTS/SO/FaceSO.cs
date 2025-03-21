using JetBrains.Annotations;
using UnityEditor;
using UnityEngine; 
[CreateAssetMenu(fileName = "Face", menuName = "Cricket/Face")]
public class FaceSO : ScriptableObject
{
    public string faceId;
    public string faceLabel;
    public int spriteId;
    public string description;
    public GameObject diceObject;
    [HideInInspector] public string spriteName;

    
    public Sprite GetSprite(string name)
    {
        var toReturn = SpriteManager.GetSpriteWithName(spriteName);
        return toReturn;
    }

    
}

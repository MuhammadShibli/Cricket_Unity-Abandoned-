using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

[CreateAssetMenu(fileName = "Dice", menuName = "Cricket/Dice")]
public class DiceSO : ScriptableObject
{
    public string diceId;
    public string diceType;
    [HideInInspector] public FaceSO[] faces = new FaceSO[6];
    public Material faceMaterial;

    // Configuration constants
    public const float FACE_SCALE = 0.12f;
    public const float FACE_OFFSET = 0.09f;
    public const float DICE_SCALE = 0.5f;

    public bool ValidateFaces()
    {
        bool isValid = true;
        for (int i = 0; i < faces.Length; i++)
        {
            if (faces[i] == null)
            {
                Debug.LogError($"Missing face at index {i} for dice {diceId}");
                isValid = false;
            }
        }
        return isValid;
    }

    // Add the missing Roll method
  
}

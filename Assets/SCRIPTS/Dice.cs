// Dice.cs - Modified to focus on visualization
using TMPro;
using UnityEngine;

public class Dice : MonoBehaviour
{
    [SerializeField] private TextMeshPro[] faceDisplays;
    private DiceSO diceData;

    public void Initialize(DiceSO diceData)
    {
        this.diceData = diceData;
        UpdateVisualFaces();
    }

    private void UpdateVisualFaces()
    {
        for (int i = 0; i < diceData.faces.Length && i < faceDisplays.Length; i++)
        {
            faceDisplays[i].text = diceData.faces[i].symbol;
        }
    }

    // Shows the visual result of a roll
    public void ShowRolledFace(int faceIndex)
    {
        if (faceIndex < 0 || faceIndex >= faceDisplays.Length)
        {
            Debug.LogError("Face index out of range.");
            return;
        }

        for (int i = 0; i < faceDisplays.Length; i++)
        {
            faceDisplays[i].gameObject.SetActive(i == faceIndex);
        }
    }

    public void ResetVisuals()
    {
        foreach (var face in faceDisplays)
        {
            face.gameObject.SetActive(true);
        }
    }
}

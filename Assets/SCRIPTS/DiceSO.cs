using UnityEngine;

[CreateAssetMenu(fileName = "Dice", menuName = "Cricket/Dice")]
public class DiceSO : ScriptableObject
{
    public string diceId;
    public string diceType;
    public FaceSO[] faces = new FaceSO[6]; // Fixed size of 6 faces

    // Reference to the visual representation
    [SerializeField] private GameObject dicePrefab;

    public FaceSO Roll()
    {
        int randomIndex = Random.Range(0, faces.Length);
        return faces[randomIndex];
    }
}

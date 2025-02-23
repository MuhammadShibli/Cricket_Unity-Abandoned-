using UnityEngine;

[CreateAssetMenu(fileName = "Cricketer", menuName = "Cricket/Cricketer")]
public class CricketerSO : ScriptableObject
{
    public string cricketerId;
    public string cricketerName;
    
    // Dice assignments
    public DiceSO[] specialDice = new DiceSO[2];
    public DiceSO[] normalDice = new DiceSO[2];
    public DiceSO[] talentDice = new DiceSO[2];
}

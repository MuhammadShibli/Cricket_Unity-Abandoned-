using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[CreateAssetMenu(fileName = "DicePool", menuName = "Cricket/DicePool")]
public class DicePoolSO : ScriptableObject
{
    public List<DiceSO> dicePool = new List<DiceSO>();
    private HashSet<DiceSO> chosenPool = new HashSet<DiceSO>();

    public void AddDice(DiceSO dice)
    {
        dicePool.Add(dice);
    }

    public DiceSO GetARandomDice()
    {
        if (dicePool.Count == 0)
        {
            dicePool.AddRange(chosenPool);
            chosenPool.Clear();
        }
        int randomIndex = Random.Range(0, dicePool.Count);
        var randomDice = dicePool[randomIndex];
        dicePool.RemoveAt(randomIndex);
        chosenPool.Add(randomDice);
        return randomDice;
    }
    public List<DiceSO> DrawRandomDice(int amount)
    {
        List<DiceSO> randomDice = new List<DiceSO>();
        for (int i = 0; i < amount; i++)
        {
            var die = GetARandomDice();
            randomDice.Add(die);

        }
        return randomDice;
    }
    public void ResetDicePool()
    {
        chosenPool.Clear();
    }

}

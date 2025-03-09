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

    public DiceSO DrawRandomDice()
    {
        if (dicePool.Count == 0)
        {
            Debug.LogError("Dice pool is empty!");
            return null;
        }

        if (chosenPool.Count == dicePool.Count)
        {
            Debug.LogWarning("All dice have been drawn. Resetting the pool.");
            ResetDicePool();
        }

        int randomIndex = -1;
        DiceSO drawnDice = null;
        while (true)
        {
            randomIndex = Random.Range(0, dicePool.Count);
            drawnDice = dicePool[randomIndex];
            if (!chosenPool.Contains(drawnDice))
            {
                chosenPool.Add(drawnDice);
                break;
            }
        }
        return drawnDice;
    }

    private void ResetDicePool()
    {
        chosenPool.Clear();
    }

    public FaceSO RollDice(DiceSO dice)
    {
        return dice.Roll();
    }
}

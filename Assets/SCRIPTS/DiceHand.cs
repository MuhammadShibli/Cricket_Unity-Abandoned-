using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DiceHand : MonoBehaviour
{
   

    private List<DiceSO> currentDice = new List<DiceSO>();
    private HashSet<int> frozenIndices = new HashSet<int>();
    private const int MAX_REROLLS = 3;
    private int rerollsRemaining;
    private DicePoolSO dicePool;
    public GameObject diceParent;
   
    
    [SerializeField] private UnityEvent<List<FaceSO>> onDiceResults;
    public void InitializeHand(DicePoolSO pool, IEnumerable<DiceSO> initialDice, DiceDisplayConfig? displayconfig = null)
    {
       
        dicePool = pool;
        currentDice.Clear();
        frozenIndices.Clear();
        currentDice.AddRange(initialDice);
        rerollsRemaining = MAX_REROLLS;
       
       
    }

   

   public void RollDice()
    {
        List<FaceSO> results = new List<FaceSO>();
        foreach (DiceSO die in currentDice)
        {
            //results.Add(die.Roll());
        }
        onDiceResults.Invoke(results);
    }

    
    public void ToggleFreezeDice(int index)
    {
        if (index >= 0 && index < currentDice.Count)
        {
            if (frozenIndices.Contains(index))
            {
                frozenIndices.Remove(index);
                Debug.Log($"Unfroze dice at index {index}");
            }
            else
            {
                frozenIndices.Add(index);
                Debug.Log($"Froze dice at index {index}");
            }
        }
    }

    public bool IsDiceFrozen(int index)
    {
        return frozenIndices.Contains(index);
    }

    public bool CanReroll()
    {
        return rerollsRemaining > 0;
    }

    public int GetRerollsRemaining()
    {
        return rerollsRemaining;
    }

    public List<DiceSO> GetCurrentDice()
    {
        return new List<DiceSO>(currentDice);
    }

    public void RedrawUnfrozenDice()
    {
        if (!CanReroll() || dicePool == null)
        {
            Debug.LogWarning("Cannot redraw: No rerolls remaining or no dice pool assigned!");
            return;
        }

        // Return unfrozen dice to pool and draw new ones
        for (int i = 0; i < currentDice.Count; i++)
        {
            if (!frozenIndices.Contains(i))
            {
                // Return current die to pool
                dicePool.AddDice(currentDice[i]);

                // Draw new die from pool
                DiceSO newDie = dicePool.GetARandomDice();
                if (newDie != null)
                {
                    currentDice[i] = newDie;
                }
                else
                {
                    Debug.LogError($"Failed to draw new die from pool for position {i}");
                }
            }
        }

        rerollsRemaining--;
        Debug.Log($"Redrew unfrozen dice. {rerollsRemaining} rerolls remaining.");

        // Update the visual representation
       
    }

  

    // Add configurable display settings
    [System.Serializable]
    public struct DiceDisplayConfig
    {
        public Vector3 startPosition;
        public Vector3 spacing;
        public Vector3 rotation;
        public float scale;

        public static DiceDisplayConfig Default => new DiceDisplayConfig
        {
            startPosition = new Vector3(1, 0, 0),
            spacing = new Vector3(1, 0, 0),
            rotation = Vector3.zero,
            scale = 1f
        };


        public static DiceDisplayConfig Vertical => new DiceDisplayConfig
        {
            startPosition = new Vector3(0, 1, 0),
            spacing = new Vector3(0, 1, 0),
            rotation = new Vector3(90, 0, 0),
            scale = 1f
        };

        public static DiceDisplayConfig Stacked => new DiceDisplayConfig
        {
            startPosition = new Vector3(0, 0, 1),
            spacing = new Vector3(0, 0, 1),
            rotation = Vector3.zero,
            scale = 1f
        };

        public static DiceDisplayConfig offScreenTeamOne=> new DiceDisplayConfig
        {
            // Position dice above and behind the camera
            startPosition = new Vector3(0, 30, -30),
            // Space them out horizontally
            spacing = new Vector3(2, 0, 0),
            // Tilt them slightly
            rotation = new Vector3(15, 0, 0),
            scale = 1f
        };

        public static DiceDisplayConfig offScreenTeamTwo => new DiceDisplayConfig
        {
            // Position dice above and behind the camera, on the left side
            startPosition = new Vector3(0, -30, -30),
            spacing = new Vector3(2, 0, 0),
            rotation = new Vector3(15, 0, 0),
            scale = 1f
        };
        public static DiceDisplayConfig GetOffScreenConfig(bool isTeamTwo) =>
            isTeamTwo ? offScreenTeamTwo : offScreenTeamOne;
        // Add a method to get throw position for a specific dice index
        public Vector3 GetThrowPosition(int index)
        {
            return startPosition + (spacing * index);
        }
    }
}

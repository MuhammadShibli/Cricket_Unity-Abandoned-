using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DiceController : MonoBehaviour
{
    public DicePoolSO dicePool;
    CricketerSO cricketer;
    DiceHand diceHand;
    DiceDisplayManager diceDisplayManager;
    List<DiceSO> diceList = new List<DiceSO>();
    private void Awake()
    {
        diceHand = FindFirstObjectByType<DiceHand>();  
        var uiManager = FindFirstObjectByType<UI_manager>();
        diceDisplayManager = FindFirstObjectByType<DiceDisplayManager>();
        uiManager.onCricketerSelectedEvent.AddListener(OnCricketerSelected);
    }

    private void OnCricketerSelected(CricketerSO cricketer)
    {
        this.cricketer = cricketer;
        Debug.Log("In Controller : Cricketer selected: " + cricketer.name);
        //now we need to add the dice to the pool first
        CreateDicePool();
        // now we need  o pick the dice from the pool 6
       diceList = dicePool.DrawRandomDice(6);
        diceDisplayManager.SetDiceList(diceList);
        //then we need to initialize the dice hand
        diceHand.InitializeHand(dicePool, diceList);
    }

    void CreateDicePool()
    {
        dicePool.ResetDicePool();

        foreach (DiceSO d in cricketer.normalDice)
        {
            for (int i = 0; i < 3; i++)
            {
                dicePool.AddDice(d);
            }
        }
        foreach (DiceSO d in cricketer.specialDice)
        {
            for (int i = 0; i < 2; i++)
            {
                dicePool.AddDice(d);
            }
        }
        foreach (DiceSO d in cricketer.talentDice)
        {
            for (int i = 0; i < 1; i++)
            {
                dicePool.AddDice(d);
            }
        }
    }

   

}

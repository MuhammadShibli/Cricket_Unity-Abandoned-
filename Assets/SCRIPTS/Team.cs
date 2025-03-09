using System;
using UnityEngine;
using UnityEngine.Events;

public class Team : MonoBehaviour
{
    public bool isTeamTwo = false;
    public TeamSO teamData;
    private CricketerSO currentCricketer;
    public DiceHand diceHand;

    [SerializeField] private UnityEvent<CricketerSO> onCricketerSelected;
    BattleManager battleManager;
    private void Awake()
    {
        InitializeDiceHand();
        battleManager = FindFirstObjectByType<BattleManager>();
        battleManager.onRequestCricketerSelection.AddListener(OnCricketerRequested);
    }

    private void OnCricketerRequested(Team requestedTeam)
    {
       
        // Only respond if this team is the one being requested
        if (requestedTeam == this)
        {
            // Here you would implement your logic to show UI or select cricketer
            // For now, let's select the first cricketer as an example
            if (teamData != null && teamData.cricketers.Count > 0)
            {
                SelectCricketer(teamData.cricketers[0]);

                // Notify BattleManager that this team is ready
                battleManager?.OnTeamReady(this);
            }
        }
    }

    private void InitializeDiceHand()
    {

        if (!diceHand) diceHand = gameObject.GetComponent<DiceHand>() ?? gameObject.AddComponent<DiceHand>();
    }

    public void SelectCricketer(CricketerSO cricketer)
    {
        if (!teamData.cricketers.Contains(cricketer)) return;

        currentCricketer = cricketer;
        var dicePool = ScriptableObject.CreateInstance<DicePoolSO>();

        // Add all dice types to pool
        foreach (var dice in cricketer.specialDice) dicePool.AddDice(dice);
        foreach (var dice in cricketer.normalDice) dicePool.AddDice(dice);
        foreach (var dice in cricketer.talentDice) dicePool.AddDice(dice);

        diceHand.InitializeHand(dicePool, dicePool.dicePool);
        onCricketerSelected?.Invoke(cricketer);
    }

    // Delegate dice operations directly to DiceHand
    public void ToggleFreezeDice(int index) => diceHand.ToggleFreezeDice(index);
    public bool IsDiceFrozen(int index) => diceHand.IsDiceFrozen(index);
    public void RedrawUnfrozenDice() => diceHand.RedrawUnfrozenDice();
    public void FinalizeHand() => diceHand.RollDice();

    public CricketerSO GetCurrentCricketer() => currentCricketer;
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;

public class BattleManager : MonoBehaviour
{
    public Team teamOne;
    public Team teamTwo;
    private Team activeTeam;

    // Event to tell teams to select their cricketers
    [SerializeField] public UnityEvent<Team> onRequestCricketerSelection;
    // Event when both teams are ready
    [SerializeField] public UnityEvent onTeamsReady;

    private bool isBattleInProgress = false;
    private bool isTurnInProgress = false;
    private void Awake()
    {
        if(teamOne==null || teamTwo==null)
        {
            Debug.LogError("Teams not properly set up!");
            return;
        }

        
    }

    void Start()
    {
        // Signal team one to show selection UI and select
        onRequestCricketerSelection?.Invoke(teamOne);
        // Team two will auto-select when it receives the event
        onRequestCricketerSelection?.Invoke(teamTwo);
    }

      public void OnTeamReady(Team team)
    {
        if (teamOne.GetCurrentCricketer() != null && teamTwo.GetCurrentCricketer() != null)
        {
            onTeamsReady?.Invoke();
            StartBattle();
        }
    }
    public void StartBattle()
    {
    
        isBattleInProgress = true;
        Debug.Log("Battle started!");
        SetActiveTeam(teamOne);
        ExecuteTurn();
    }

    public void SetActiveTeam(Team team)
    {
        activeTeam = team;
        if (team?.teamData != null)
        {
            Debug.Log($"Active team: {team.teamData.teamName}");
        }
    }

    public void ExecuteTurn()
    {
        if (isTurnInProgress) return;

        isTurnInProgress = true;
        activeTeam.diceHand.RollDice();

    }

    

    private void EndTurn()
    {
        isTurnInProgress = false;

        if (isBattleInProgress)
        {
            // Switch teams
            SetActiveTeam(activeTeam == teamOne ? teamTwo : teamOne);
            // Queue next turn
            ExecuteTurn();
        }
    }

    private void ProcessResults(List<FaceSO> results)
    {
        if (activeTeam == null || results == null) return;

        foreach (var result in results)
        {
            if (result != null)
            {
                Debug.Log($"Dice result for {activeTeam.teamData.teamName}: {result.name}");
            }
        }
    }

   
   
    private void OnDisable()
    {
        StopAllCoroutines();
    }
}

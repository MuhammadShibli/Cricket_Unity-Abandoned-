using JetBrains.Annotations;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System;
public class UI_manager : MonoBehaviour
{
    public GameObject cricketerUI;
    public GameObject cricketercell;
    private BattleManager battleManager;
    private GameObject cricketerSelectUI;
    
    GameObject contentHolder;
    private void Awake()
    {
        // Find the BattleManager instance first
        battleManager = FindFirstObjectByType<BattleManager>();
        
        if (battleManager != null)
        {
            battleManager.onRequestCricketerSelection.AddListener(ShowCricketerUI);
        }
        else
        {
            Debug.LogError("BattleManager not found in scene!");
        }
    }

    public void ShowCricketerUI(Team t)
    {
        if (t.isTeamTwo) return;
      
        if (cricketerUI != null)
        {
            cricketerSelectUI = Instantiate(cricketerUI, transform);
            cricketerSelectUI.SetActive(true);
            contentHolder = cricketerSelectUI.GetComponent<UIPanelContentSelector>().contentView; 
            AddCellsToUI(t.teamData);
        }
        else
        {
            Debug.LogError("CricketerUI prefab not assigned!");
        }
    }

    private void OnDestroy()
    {
        // Clean up the listener when the object is destroyed
        if (battleManager != null)
        {
            battleManager.onRequestCricketerSelection.RemoveListener(ShowCricketerUI);
        }
    }

    // method for team to add cellss to the UI
    public void AddCellsToUI(TeamSO cricketers)
    {
        // Add cells to the UI
        foreach (var cricketer in cricketers.cricketers)
        {
    
            var cell =  Instantiate(cricketercell, contentHolder.transform);
            var cellObj = cell.GetComponent<CricketerCell>();
            
            cellObj.onCricketerSelected.AddListener(OnCricketerSelected);
            cellObj.Initialise(cricketer);
        }
    }

    private void OnCricketerSelected(CricketerSO arg0)
    { 
        cricketerSelectUI.SetActive(false);
        GetComponent<UIDicePhase>().OnCricketerReady(arg0);
    }
}
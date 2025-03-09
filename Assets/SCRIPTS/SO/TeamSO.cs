using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Team", menuName = "Cricket/Team")]
public class TeamSO : ScriptableObject
{
    public string teamId;
    public string teamName;
    public List<CricketerSO> cricketers = new List<CricketerSO>();
    public const int MAX_CRICKETERS = 4;

    public void ValidateTeam()
    {
        if (cricketers.Count > MAX_CRICKETERS)
        {
            Debug.LogWarning($"Team {teamName} has more than {MAX_CRICKETERS} cricketers. Extra cricketers will be ignored.");
            cricketers = cricketers.GetRange(0, MAX_CRICKETERS);
        }
    }
}

using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewMissionData", menuName = "Mission System/Mission Data")]
public class MissionData : ScriptableObject
{
    public string missionID;
    public string missionName;
    [TextArea(3, 5)] public string missionDescription;
    public int currencyReward;
    public int xpReward;

    // Using a list so we can clone it at runtime
    public List<ObjectiveData> objectives = new List<ObjectiveData>();
}

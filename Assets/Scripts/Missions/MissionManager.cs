using UnityEngine;
using System;
using System.Collections.Generic;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance { get; private set; }

    public event Action<MissionData> OnMissionStarted;
    public event Action<ObjectiveData> OnObjectiveUpdated;
    public event Action<MissionData> OnMissionCompleted;
    public event Action<MissionData> OnMissionFailed;
    public event Action OnExtractionUnlocked;

    private MissionData originalTemplate;
    private MissionData currentMission;
    private List<ObjectiveData> currentObjectives = new List<ObjectiveData>();
    private bool extractionUnlocked;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void StartMission(MissionData missionTemplate)
    {
        if (missionTemplate == null) return;

        // Clone the scriptable object so we don't modify the asset
        originalTemplate = missionTemplate;
        currentMission = UnityEngine.Object.Instantiate(missionTemplate);
        currentObjectives.Clear();

        foreach (var obj in currentMission.objectives)
        {
            // Clone objective data
            currentObjectives.Add(new ObjectiveData
            {
                objectiveID = obj.objectiveID,
                description = obj.description,
                type = obj.type,
                targetID = obj.targetID,
                isCompleted = false,
                isOptional = obj.isOptional
            });
        }

        extractionUnlocked = false;
        OnMissionStarted?.Invoke(currentMission);
    }

    public void UpdateObjective(string targetID)
    {
        if (currentMission == null) return;

        bool allMandatoryCompleted = true;
        bool updatedAny = false;

        foreach (var obj in currentObjectives)
        {
            if (!obj.isCompleted && obj.targetID == targetID)
            {
                obj.isCompleted = true;
                updatedAny = true;
                OnObjectiveUpdated?.Invoke(obj);
            }

            if (!obj.isCompleted && !obj.isOptional)
            {
                allMandatoryCompleted = false;
            }
        }

        if (updatedAny && allMandatoryCompleted && !extractionUnlocked)
        {
            UnlockExtraction();
        }
    }

    private void UnlockExtraction()
    {
        extractionUnlocked = true;
        Debug.Log("All mandatory objectives complete. Extraction Unlocked!");
        OnExtractionUnlocked?.Invoke();
    }

    public void CompleteMission()
    {
        if (currentMission != null && extractionUnlocked)
        {
            Debug.Log($"Mission {currentMission.missionName} Completed!");
            OnMissionCompleted?.Invoke(currentMission);
            Destroy(currentMission);
            currentMission = null;
            currentObjectives.Clear();
        }
    }

    public void FailMission()
    {
        if (currentMission != null)
        {
            Debug.Log($"Mission {currentMission.missionName} Failed!");
            OnMissionFailed?.Invoke(currentMission);
            Destroy(currentMission);
            currentMission = null;
            currentObjectives.Clear();
        }
    }

    public bool IsExtractionUnlocked => extractionUnlocked;
    public MissionData CurrentMission => currentMission;
    public MissionData OriginalTemplate => originalTemplate;
    public List<ObjectiveData> CurrentObjectives => currentObjectives;
}

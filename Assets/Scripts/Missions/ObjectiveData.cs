using System;

[Serializable]
public class ObjectiveData
{
    public string objectiveID;
    public string description;
    public ObjectiveType type;
    public string targetID; // ID of the specific target object (e.g. "Terminal_01")
    public bool isCompleted;
    public bool isOptional;
}

using System;

[Serializable]
public class SaveData
{
    public int hackerPoints = 0;
    public string lastMissionID = "";
    // Add arrays or lists for bought upgrades, etc.
    public bool[] boughtUpgrades = new bool[10];
}

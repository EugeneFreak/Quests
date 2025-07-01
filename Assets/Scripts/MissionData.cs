using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/MissionData")]
public class MissionData : ScriptableObject
{
    public string missionName;
    public float startDelay;

    public MonoBehaviour missionPrefab;

}

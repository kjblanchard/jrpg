using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ItemForPickup", order = 2)]
public class ItemForPickup : ScriptableObject
{
    [SerializeField] public int ItemNumber;
    [SerializeField] public int QuestNumberForPickup;
}

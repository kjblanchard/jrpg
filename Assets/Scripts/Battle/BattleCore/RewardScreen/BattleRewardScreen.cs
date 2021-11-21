using UnityEngine;

public class BattleRewardScreen : MonoBehaviour
{
    public void DisplayCanvas(bool isEnabled) => _canvas.enabled = isEnabled;
    [SerializeField] private Canvas _canvas;

    public GameObject ResultGameObjectPrefab;
    public GameObject PrefabSpawnLocation;

}

using UnityEngine;
using UnityEngine.UI;

public class PlayerHud : MonoBehaviour
{
    public void DisplayCanvas(bool isEnabled) => _canvas.enabled = isEnabled;
    [SerializeField] private Canvas _canvas;
    [SerializeField] private PlayerHudItem[] _playerHudItems;


    /// <summary>
    /// Adds the battler to the hud item so that it can be referenced.
    /// </summary>
    /// <param name="playerBattlers"></param>
    public void LoadBattlersIntoHud(Battler[] playerBattlers)
    {
        for (var i = 0; i < playerBattlers.Length; i++)
        {
            var currentBattler = playerBattlers[i];
            if (currentBattler is null)
                continue;
            _playerHudItems[i].BattlerAssigned = currentBattler;

            //SubscribeToBattlersDamageCausedEvent to be able to update the gui when this happens
            currentBattler.BattlerDamageComponent.DamageCausedEvent += OnDamageCaused;
            currentBattler.BattlerDamageComponent.MpDamageCausedEvent += OnDamageCaused;

            if (currentBattler.BattleStats.IsPlayer)
            {
                _playerHudItems[i].GambitDropdown.onValueChanged.AddListener((num) =>
                {
                    currentBattler.BattlerGambitComponent.IsGambitsEnabled = num > 0;
                    if (num > 0)
                        currentBattler.BattlerGambitComponent.GambitGroupChosen = num - 1;
                });

            }
        }

        foreach (var _playerHudItem in _playerHudItems)
        {
            if (_playerHudItem.BattlerAssigned is null)
                _playerHudItem.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Updates the elements in the hud.  Used when Information changes.
    /// </summary>
    public void UpdatePlayerHud()
    {
        foreach (var _playerHudItem in _playerHudItems)
        {
            if (_playerHudItem.BattlerAssigned == null) continue;
            _playerHudItem.HpText.text = _playerHudItem.BattlerAssigned.BattleStats.BattlerCurrentHp.ToString();
            _playerHudItem.NameText.text = _playerHudItem.BattlerAssigned.BattleStats.BattlerDisplayName;
            _playerHudItem.MpText.text = _playerHudItem.BattlerAssigned.BattleStats.BattlerCurrentMp.ToString();
        }
    }

    private void OnDamageCaused(object obj, int e)
    {
        UpdatePlayerHud();
    }

}

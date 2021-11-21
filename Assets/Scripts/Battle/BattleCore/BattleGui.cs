using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleGui : MonoBehaviour
{
    /// <summary>
    /// Tells if the gui is loading or not, useful for polling for animations and stuff.  This is powered by the events below and should subscribe to them.
    /// </summary>
    public static bool IsAnimationPlaying => _loadingDictionary.Count > 0;
    public BattlePlayerWindow GetPlayerWindow(Battler battler) => _playerWindows[battler.BattleStats.BattlerNumber];
    /// <summary>
    /// Gets the magic window for the battler, based on their battler number.
    /// </summary>
    /// <param name="battler">The battler to get his window</param>
    /// <returns>The magic window for that battler.</returns>
    public BattleMagicWindow GetMagicWindow(Battler battler) => _playerMagicWindows[battler.BattleStats.BattlerNumber];

    public BattleNotificationsGui BattleNotifications => _battleNotificationsGui;
    public PlayerHud PlayerHud => _playerHudComponent;
    public PlayerHud EnemyHud => _enemyHudComponent;
    public TurnOrderGui TurnOrder => _mainTurnOrderGui;
    public BattleGuiTransitionComponent BattleTransitionComponent => _battleGuiTransitionComponent;
    public BattleRewardScreen BattleRewardScreen => _battleRewardScreen;

    private static readonly Dictionary<Guid, object> _loadingDictionary = new Dictionary<Guid, object>();
    [SerializeField] private GameObject _playerHudGameObjectPrefab;
    private PlayerHud _playerHudComponent;
    [SerializeField] private GameObject _enemyHudGameObjectPrefab;
    private PlayerHud _enemyHudComponent;
    [SerializeField] private GameObject _mainTurnOrderGameObjectPrefab;
    private TurnOrderGui _mainTurnOrderGui;
    [SerializeField] private GameObject _playerWindowPrefab;
    private BattlePlayerWindow[] _playerWindows;
    [SerializeField] private GameObject _playerMagicWindowPrefab;
    private BattleMagicWindow[] _playerMagicWindows;
    [SerializeField] private GameObject _battleNotificationPrefab;
    private BattleNotificationsGui _battleNotificationsGui;
    [SerializeField] private GameObject _battleGuiNotificationPrefab;
    private BattleGuiTransitionComponent _battleGuiTransitionComponent;
    [SerializeField] private GameObject _battleRewardScreenPrefab;
    private BattleRewardScreen _battleRewardScreen;

    public void LoadAllGuiObjects()
    {
        var playerHud = Instantiate(_playerHudGameObjectPrefab);
        _playerHudComponent = playerHud.GetComponent<PlayerHud>();
        var enemyHud = Instantiate(_enemyHudGameObjectPrefab);
        _enemyHudComponent = enemyHud.GetComponent<PlayerHud>();
        var turnOrder = Instantiate(_mainTurnOrderGameObjectPrefab);
        _mainTurnOrderGui = turnOrder.GetComponent<TurnOrderGui>();
        var playerWindow = Instantiate(_playerWindowPrefab);
        _playerWindows = new[] { playerWindow.GetComponent<BattlePlayerWindow>() };
        var magicWindow = Instantiate(_playerMagicWindowPrefab);
        _playerMagicWindows = new[] { magicWindow.GetComponent<BattleMagicWindow>() };
        var battleNotifications = Instantiate(_battleNotificationPrefab);
        _battleNotificationsGui = battleNotifications.GetComponent<BattleNotificationsGui>();
        var battleReward = Instantiate(_battleRewardScreenPrefab);
        _battleRewardScreen = battleReward.GetComponent<BattleRewardScreen>();

        _battleGuiTransitionComponent = FindObjectOfType<BattleGuiTransitionComponent>();

        SubscribeMagicWindowsToGuiLoadingEvents();

    }

    public void EnabmeAllCanvasForLoadingScreen()
    {
        _playerHudComponent.DisplayCanvas(true);
        _enemyHudComponent.DisplayCanvas(true);
        _mainTurnOrderGui.DisplayCanvas(true);
        foreach (var _battlePlayerWindow in _playerWindows)
        {
            _battlePlayerWindow?.DisplayCanvas(true);
        }
        foreach (var _playerMagicWindow in _playerMagicWindows)
        {
            _playerMagicWindow?.DisplayCanvas(true);
        }
        _battleNotificationsGui.DisplayCanvas(true);
    }
    public void DisableAllCanvasForRewardScreen()
    {
        _playerHudComponent.DisplayCanvas(false);
        _enemyHudComponent.DisplayCanvas(false);
        _mainTurnOrderGui.DisplayCanvas(false);
        foreach (var _battlePlayerWindow in _playerWindows)
        {
            _battlePlayerWindow?.DisplayCanvas(false);
        }
        foreach (var _playerMagicWindow in _playerMagicWindows)
        {
            _playerMagicWindow?.DisplayCanvas(false);
        }
        _battleNotificationsGui.DisplayCanvas(false);
    }




    /// <summary>
    /// Subscribes the magic windows to use the loading events so that this will know when we are loading based on the dictionary.
    /// </summary>
    private void SubscribeMagicWindowsToGuiLoadingEvents()
    {
        foreach (var _playerMagicWindow in _playerMagicWindows)
        {
            if (_playerMagicWindow != null)
                _playerMagicWindow.GuiLoadingEvent += OnGuiLoadingEvent;
        }
    }

    /// <summary>
    /// This is used to handle when a gui component is loading or not, the dictionary is what determines if the scene is loading.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="e"></param>
    public void OnGuiLoadingEvent(object obj, GuiLoadingEventArgs e)
    {
        if (e.IsLoading)
            _loadingDictionary.Add(e.Id, obj);
        else
            _loadingDictionary.Remove(e.Id);
    }
}

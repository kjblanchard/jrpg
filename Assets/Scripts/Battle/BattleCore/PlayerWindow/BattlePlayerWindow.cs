using System;
using DG.Tweening;
using UnityEngine;

public class BattlePlayerWindow : MonoBehaviour
{

    public void DisplayCanvas(bool isEnabled) => _canvas.enabled = isEnabled;
    [SerializeField] private Canvas _canvas;

    //Tween names
    private const string _playerWindowOpenScale = "player1WindowOpenScaleTween";
    private const string _playerWindowOpenMove = "player1WindowOpenMoveTween";
    private const string _playerWindowOpenRotate = "player1WindowOpenRotateTween";

    /// <summary>
    /// The button that will be attached to the attack command
    /// </summary>
    private const int _attackButtonNum = 0;
    /// <summary>
    /// The button attached to the magic command
    /// </summary>
    private const int _magicButtonNum = 1;
    /// <summary>
    /// The button attached to the defend button
    /// </summary>
    private const int _defendButtonNum = 2;

    public BattleButton AttackButton => _battleButtons[_attackButtonNum];
    public BattleButton MagicButton => _battleButtons[_magicButtonNum];
    public BattleButton DefendButton => _battleButtons[_defendButtonNum];

    /// <summary>
    /// This is for knowing when the tweens have finished playing for opening and closing the windows
    /// </summary>
    [SerializeField] private DotweenBroadcasterComponent _playerWindowOpenBroadcaster;

    /// <summary>
    /// Holder for all of the battle buttons
    /// </summary>
    [SerializeField] private BattleButton[] _battleButtons;

    /// <summary>
    /// For handling state changes on button presses
    /// </summary>
    private BattleStateMachine _battleStateMachine;

    private BattleMagicWindow _battleMagicWindow;

    private bool _isOpen;


    private void Awake()
    {
        _battleStateMachine = FindObjectOfType<BattleStateMachine>();
    }

    private void Start()
    {
        _battleMagicWindow = FindObjectOfType<BattleMagicWindow>();
        SubscribeToPlayerWindowTweenEvents();
        SubscribeToAttackButtonEvents();
        SubscribeToMagicButtonEvents();
        SubscribeToDefendButtonEvents();

    }

    private void SubscribeToDefendButtonEvents()
    {
        _battleButtons[_defendButtonNum].BattleButtonBroadcaster.ButtonPressedEvent += OnPlayerDefendButtonPress;
    }

    private void SubscribeToMagicButtonEvents()
    {
        _battleButtons[_magicButtonNum].BattleButtonBroadcaster.ButtonPressedEvent += OnPlayerMagicButtonPress;
    }

    private void SubscribeToAttackButtonEvents()
    {
        _battleButtons[_attackButtonNum].BattleButtonBroadcaster.ButtonPressedEvent += OnPlayerAttackButtonPress;
        _battleButtons[_attackButtonNum].BattleButtonBroadcaster.ButtonHoveredEvent += OnPlayerAttackButtonHover;
    }

    private void SubscribeToPlayerWindowTweenEvents()
    {
        _playerWindowOpenBroadcaster.DotweenCompleteEvent += OnPlayerDotweenWindowOpenComplete;
        _playerWindowOpenBroadcaster.DotweenRewindCompleteEvent += OnPlayerWindowCloseComplete;
    }

    /// <summary>
    /// Starts all the tweens to open the window with an animation
    /// </summary>
    public void OpenPlayerWindow()
    {
        if (_isOpen)
            return;
        _isOpen = true;
        DOTween.Restart(_playerWindowOpenMove);
        DOTween.Restart(_playerWindowOpenScale);
        DOTween.Restart(_playerWindowOpenRotate);
        SoundController.Instance.PlaySfxOneShot(SoundController.Sfx.PlayerTurn);
    }

    /// <summary>
    /// Starts all the tweens in reverse to close the window with an animation
    /// </summary>
    public void ClosePlayerWindow()
    {
        if (!_isOpen)
            return;
        if (_battleMagicWindow)
            RewindAllButtons();
        DOTween.PlayBackwards(_playerWindowOpenRotate);
        DOTween.PlayBackwards(_playerWindowOpenScale);
        DOTween.PlayBackwards(_playerWindowOpenMove);
    }

    private void OnPlayerDotweenWindowOpenComplete(object obj, EventArgs e)
    {

    }

    private void OnPlayerWindowCloseComplete(object obj, EventArgs e)
    {
        _isOpen = false;
        _battleStateMachine.ChangeBattleState(BattleStateMachine.BattleStates.ActionPerformState);


    }
    private void OnPlayerAttackButtonHover(object obj, EventArgs e)
    {
        return;

    }

    private void OnPlayerAttackButtonPress(object obj, EventArgs e)
    {
        var currentState = _battleStateMachine.CurrentBattleStateEnum;
        if ((currentState != BattleStateMachine.BattleStates.PlayerTurnState && currentState != BattleStateMachine.BattleStates.PlayerTargetingState) || BattleGui.IsAnimationPlaying)
            return;
        if (currentState == BattleStateMachine.BattleStates.PlayerTargetingState)
        {
            if (BattleState.IsCurrentBattlerAttacking)
                PlayButtonAndStopOthers(AttackButton);
            else
            {
                RewindAllButtons();
            }
            _battleStateMachine.ChangeBattleState(BattleStateMachine.BattleStates.PlayerTurnState);
        }
        _battleMagicWindow.ClosePlayerWindow();
        BattleState.IsCurrentBattlerAttacking = true;
        PlayButtonAndStopOthers(AttackButton);
        _battleStateMachine.ChangeBattleState(BattleStateMachine.BattleStates.PlayerTargetingState);

    }
    private void OnPlayerMagicButtonPress(object obj, EventArgs e)
    {
        if (_battleStateMachine.CurrentBattleStateEnum == BattleStateMachine.BattleStates.PlayerTargetingState)
        {
            _battleStateMachine.ChangeBattleState(BattleStateMachine.BattleStates.PlayerTurnState, new bool[1]);
        }
        if (_battleStateMachine.CurrentBattleStateEnum != BattleStateMachine.BattleStates.PlayerTurnState || BattleGui.IsAnimationPlaying)
            return;
        PlayButtonAndStopOthers(MagicButton);
        _battleMagicWindow.OpenPlayerWindow();


    }
    private void OnPlayerDefendButtonPress(object obj, EventArgs e)
    {
        var currentState = _battleStateMachine.CurrentBattleStateEnum;
        if ((currentState != BattleStateMachine.BattleStates.PlayerTurnState && currentState != BattleStateMachine.BattleStates.PlayerTargetingState) || BattleGui.IsAnimationPlaying)
            return;
        BattleState.IsCurrentBattlerDefending = true;
        _battleMagicWindow.ClosePlayerWindow();
        ClosePlayerWindow();

    }

    private void RewindAllButtons(BattleButton buttonYouWantToPlay = null)
    {
        foreach (var _battleButton in _battleButtons)
        {
            if (_battleButton.IsSelectedAnimation == null)
                continue;
            if ((buttonYouWantToPlay != null && buttonYouWantToPlay == _battleButton))
                continue;
            if (!_battleButton.isPlayingSelectedAnimation) continue;
            _battleButton.isPlayingSelectedAnimation = false;
            _battleButton.IsSelectedAnimation.DORewind();
        }
    }

    private void PlayButtonAndStopOthers(BattleButton buttonYouWantToPlay)
    {
        RewindAllButtons(buttonYouWantToPlay);
        if (buttonYouWantToPlay.isPlayingSelectedAnimation) return;
        buttonYouWantToPlay.isPlayingSelectedAnimation = true;
        buttonYouWantToPlay.IsSelectedAnimation.DORestart();
    }

}

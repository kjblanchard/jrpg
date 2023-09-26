using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// This state should load as many things as possible at the beginning of the battle while the screen is black to reduce loading during the battle.
/// </summary>
public class BattleLoadingState : BattleState
{

    public override void StartState(params bool[] startupBools)
    {
        InstantiateGuiPrefabs();
        SubscribeToBattleFadeEvents();
        PopulateBattleDataFromPersistentData();
        var allBattlers = InstantiateBattlers();

        LoadBattlerNameTexts(allBattlers.Length);
        //TODO this needs to go somewhere
        BattlerActionPerformComponent._battleProjectilePool = _battleComponent.BattleProjectilePool;
        CorrectDuplicateEnemyNames(allBattlers);
        CalculateInitialTurnsForBattlers(allBattlers);
        var initialTurnOrder = CreateInitialTurnOrder(allBattlers);
        InitializeTurnOrderGui(initialTurnOrder);
        InitializePlayerMagic(_battleComponent.BattleData.PlayerBattlers);
        InitializeGuiHuds(_battleComponent.BattleData.PlayerBattlers, _battleComponent.BattleData.EnemyBattlers);
        InitializeBattlerDamageDisplays(allBattlers);
        InitializeBattlersClickHandlers();
        _battleComponent.BattleGui.EnabmeAllCanvasForLoadingScreen();
        InitializeBattlerAbilitySoundFx(allBattlers);
        InitializeAbilityProjectiles(allBattlers);
        StartBattleFadeIn();
    }

    private static void InitializeAbilityProjectiles(Battler[] allBattlers)
    {
        foreach (var _allBattler in allBattlers)
        {
            if (_allBattler.battlerAttackAbility.AnimationSteps != null)
            {
                foreach (var _abilityAnimStep in _allBattler.battlerAttackAbility.AnimationSteps)
                {
                    if (_abilityAnimStep.ProjectileToSpawn != Projectiles.Default)
                        _battleComponent.BattleProjectilePool.CreateInitialInstance(_abilityAnimStep.ProjectileToSpawn);
                }
            }

            foreach (var _battleStatsAbility in _allBattler.BattleStats.Abilities)
            {
                foreach (var _abilityAnimStep in _battleStatsAbility.AnimationSteps)
                {

                    if (_abilityAnimStep.ProjectileToSpawn == default)
                        continue;
                    _battleComponent.BattleProjectilePool.CreateInitialInstance(_abilityAnimStep.ProjectileToSpawn);

                }
            }
        }
    }


    /// <summary>
    /// Instantiates all of the gui items for the battle from prefabs.
    /// </summary>
    private static void InstantiateGuiPrefabs()
    {
        _battleComponent.BattleGui.LoadAllGuiObjects();
    }

    /// <summary>
    /// Adds functions to the battles fade in and fade out events.
    /// </summary>
    private static void SubscribeToBattleFadeEvents()
    {
        _battleComponent.BattleGui.BattleTransitionComponent.BattleFadeInEvent += (sender, args) => _battleComponent.BattleStateMachine.ChangeBattleState(BattleStateMachine.BattleStates.BetweenTurnState);
        _battleComponent.BattleGui.BattleTransitionComponent.BattleFadeOutEvent += (sender, args) =>
        {
            BattleMusicHandler.StopBattleWin();
            DOTween.defaultAutoPlay = AutoPlay.All;
            SceneController.ChangeGameScene(SceneController.GameScenesEnum.DebugRoom);
        };

    }

    /// <summary>
    /// Grab the data from the persistent data (overworld data passed in)
    /// </summary>
    private static void PopulateBattleDataFromPersistentData() => _battleComponent.BattleData.SetBattleData(PersistantData.instance.GetBattleData());

    /// <summary>
    /// Instantiate the battlers in from the battle data
    /// </summary>
    /// <returns>An array of all the battlers that have been loaded in</returns>
    private static Battler[] InstantiateBattlers() => _battleComponent.BattleData.ConfigureAllBattlers();

    /// <summary>
    /// Checks all the enemies and adds a prefix to their name if they are duplicates
    /// </summary>
    private static void CorrectDuplicateEnemyNames(IEnumerable<Battler> battlerList)
    {
        var groupsOfDuplicateEnemies =
                                        from battler in battlerList
                                        group battler by battler.BattleStats.BattlerNameEnum
                                        into battlerTypes
                                        where battlerTypes.Count() > 1
                                        select battlerTypes;

        foreach (var group in groupsOfDuplicateEnemies)
        {
            var letterToAppend = 'A';
            foreach (var _battler in group)
            {
                _battler.BattleStats.AddBattlerNamePostFix(letterToAppend);
                letterToAppend++;
            }
        }

    }

    /// <summary>
    /// Calculates the initial 20 turns for battle and then confirms them so that there is an initial display
    /// </summary>
    /// <param name="battlers"></param>
    private static void CalculateInitialTurnsForBattlers(IEnumerable<Battler> battlers)
    {
        foreach (var _battler in battlers)
        {
            _battler.BattlerTimeManager.CalculatePotentialNext20Turns(1.0f, true);
            _battler.BattlerTimeManager.ConfirmTurn();
        }
    }

    /// <summary>
    /// Load the sound effects for all of the battlers sounds.
    /// </summary>
    /// <param name="allBattlers"></param>
    private static void InitializeBattlerAbilitySoundFx(Battler[] allBattlers) =>
        SoundController.Instance.LoadSfx(allBattlers.SelectMany(battler =>
            battler.BattleStats.Abilities.SelectMany(ability => ability.AbilitySfxToLoad)));

    /// <summary>
    /// Instantiates the battler name texts from the gui.
    /// </summary>
    /// <param name="numOfBattlers"></param>
    private static void LoadBattlerNameTexts(int numOfBattlers) => _battleComponent.BattleGui.BattleNotifications.SpawnNameTexts(numOfBattlers);

    /// <summary>
    /// Generates and confirms the initial 20 turns of the battle
    /// </summary>
    /// <param name="allBattlers">All battlers that are going to </param>
    /// <returns></returns>
    private static Battler[] CreateInitialTurnOrder(Battler[] allBattlers)
    {
        var next20Turns = BattlerClock.GenerateTurnList(allBattlers);
        BattlerClock.ConfirmNext20Battlers();
        return next20Turns;
    }

    /// <summary>
    /// Initializes the turn order gui from the turn order
    /// </summary>
    /// <param name="turnOrder">The current battler turn order</param>
    private static void InitializeTurnOrderGui(Battler[] turnOrder) => _battleComponent.BattleGui.TurnOrder.UpdateBattlerPicturesInTurnOrderGui(turnOrder);

    /// <summary>
    /// Initializes both of the Huds from the battlers.
    /// </summary>
    private static void InitializeGuiHuds(Battler[] playerBattlers, Battler[] enemyBattlers)
    {
        _battleComponent.BattleGui.PlayerHud.LoadBattlersIntoHud(playerBattlers);
        _battleComponent.BattleGui.EnemyHud.LoadBattlersIntoHud(enemyBattlers);
        _battleComponent.BattleGui.PlayerHud.UpdatePlayerHud();
        _battleComponent.BattleGui.EnemyHud.UpdatePlayerHud();
    }

    /// <summary>
    /// Puts the magic abilities into the players magic hud, based on the abilities that they have.
    /// </summary>
    /// <param name="playerBattlers"></param>
    private static void InitializePlayerMagic(IEnumerable<Battler> playerBattlers)
    {
        foreach (var currentBattler in playerBattlers)
        {
            if (currentBattler == null)
                continue;
            _battleComponent.BattleGui.GetMagicWindow(currentBattler).LoadAbilitiesIntoButtons(currentBattler.BattleStats.Abilities, currentBattler);
        }
    }

    /// <summary>
    /// Initializes the damage display for all of the battlers.
    /// </summary>
    /// <param name="allBattlers"></param>
    private static void InitializeBattlerDamageDisplays(IEnumerable<Battler> allBattlers)
    {
        _battleComponent.BattleGui.BattleNotifications.SpawnDamageTexts();
        SubscribeBattlersToDamageDisplay(allBattlers);
    }


    /// <summary>
    /// Calls into the battle gui and starts the fade in.
    /// </summary>
    private static void StartBattleFadeIn() => _battleComponent.BattleGui.BattleTransitionComponent.StartFadeIn();


    private static void SubscribeBattlersToDamageDisplay(IEnumerable<Battler> allBattlers)
    {
        foreach (var _allBattler in allBattlers)
        {
            _allBattler.BattlerDamageComponent.DamageCausedEvent += (_, e) =>
            {
                var textToDisplay = _battleComponent.BattleGui.BattleNotifications.GetTmpTextFromQueue();
                textToDisplay.transform.position = (_allBattler.BattleStats.IsPlayer)
                    ? _allBattler.BattlerLocationHandler.GetDamageDisplayInScreenPosPlayer
                    : _allBattler.BattlerLocationHandler.GetDamageDisplayInScreenPos;
                var color = DetermineColorForDamageDisplay(e);
                e = (e > 0) ? e : -e;
                textToDisplay.PlayDamage(e.ToString(), color);
                textToDisplay.PutBackInQueue = () =>
                {
                    _battleComponent.BattleGui.BattleNotifications.ReturnDamageTextToQueue(textToDisplay);
                };
            };

            _allBattler.BattlerDamageComponent.MpDamageCausedEvent += (_, e) =>
            {
                if (e >= 0)
                    return;
                var textToDisplay = _battleComponent.BattleGui.BattleNotifications.GetTmpTextFromQueue();
                var displayLocation = (_allBattler.BattleStats.IsPlayer)
                    ? _allBattler.BattlerLocationHandler.GetMpDamageDisplayInScreenPosPlayer
                    : _allBattler.BattlerLocationHandler.GetMpDamageDisplayInScreenPos;
                textToDisplay.transform.position = displayLocation;
                var color = Color.blue;
                e = -e;
                textToDisplay.PlayDamage(e.ToString(), color);
                textToDisplay.PutBackInQueue = () =>
                {
                    _battleComponent.BattleGui.BattleNotifications.ReturnDamageTextToQueue(textToDisplay);
                };
            };
        }
    }

    private static Color DetermineColorForDamageDisplay(int damageGiven)
    {
        if (damageGiven > 0)
            return Color.red;
        return damageGiven == 0 ? Color.clear : Color.green;
    }

    /// <summary>
    /// Loads the players and enemies clicks to handle what happens when you click on them.
    /// </summary>
    private static void InitializeBattlersClickHandlers()
    {
        InitializeClickArray(_battleComponent.BattleData.PlayerBattlers);
        InitializeClickArray(_battleComponent.BattleData.EnemyBattlers);
    }

    /// <summary>
    /// Fills the click handler with data based on the battlers passed in.
    /// </summary>
    /// <param name="battlersToCreateDataWith"></param>
    /// <returns></returns>
    private static void InitializeClickArray(IReadOnlyList<Battler> battlersToCreateDataWith)
    {
        var array = new BattlerClickHandler[battlersToCreateDataWith.Count];
        for (var i = 0; i < battlersToCreateDataWith.Count; i++)
        {
            var battler = battlersToCreateDataWith[i];
            if (battler == null) continue;
            var battlerClick = array[i] = battler.BattlerClickHandler;
            battlerClick._battleButtonBroadcaster.ButtonPressedEvent +=
                GenerateButtonPressedFunction(battler);
            battlerClick._battleButtonBroadcaster.ButtonHoveredEvent += GenerateButtonHoveredFunction(battler);
            battlerClick._battleButtonBroadcaster.ButtonHoveredLeaveEvent +=
                GenerateButtonHoverLeaveFunction(battler);
        }
    }

    /// <summary>
    /// Generates a function when the battler is clicked on.
    /// </summary>
    /// <param name="battlerClicked"></param>
    /// <returns></returns>
    private static BattleButtonBroadcaster.BattleButtonActionEventHandler GenerateButtonPressedFunction(Battler battlerClicked)
    {
        return
            (obj, e) =>
                {
                    if (_battleComponent.BattleStateMachine.CurrentBattleStateEnum != BattleStateMachine.BattleStates.PlayerTargetingState || battlerClicked.BattleStats.IsDead)
                        return;
                    _targetBattler = battlerClicked;
                    _battleComponent.BattleGui.GetPlayerWindow(_currentBattler).ClosePlayerWindow();
                    _battleComponent.BattleGui.GetMagicWindow(_currentBattler).ClosePlayerWindow();
                };
    }

    /// <summary>
    /// Generates a function for when the player is hovered.
    /// </summary>
    /// <param name="battlerHovered"></param>
    /// <returns></returns>
    private static BattleButtonBroadcaster.BattleButtonActionEventHandler GenerateButtonHoveredFunction(
        Battler battlerHovered)
    {
        return
           (obj, e) =>
            {
                if (_battleComponent.BattleStateMachine.CurrentBattleStateEnum != BattleStateMachine.BattleStates.PlayerTargetingState || battlerHovered.BattleStats.IsDead)
                    return;
                DisplayCharacterName(battlerHovered);
                battlerHovered.spriteComp.color = Color.yellow;
            };
    }

    /// <summary>
    /// Generates a function for when the battler isn't hovered anymore.
    /// </summary>
    /// <param name="battlerHovered"></param>
    /// <returns></returns>
    private static BattleButtonBroadcaster.BattleButtonActionEventHandler GenerateButtonHoverLeaveFunction(
        Battler battlerHovered)
    {
        return
           (obj, e) =>
            {
                if (_battleComponent.BattleStateMachine.CurrentBattleStateEnum != BattleStateMachine.BattleStates.PlayerTargetingState || battlerHovered.BattleStats.IsDead)
                    return;
                battlerHovered.enabled = false;
                battlerHovered.spriteComp.color = Color.white;
                battlerHovered.battlerNameDisplay.enabled = false;
            };
    }

    private static void DisplayCharacterName(Battler battlerNameToDisplay)
    {
        //Get the tmpText from the queue if it isn't generated yet.
        battlerNameToDisplay.battlerNameDisplay ??=
            _battleComponent.BattleGui.BattleNotifications.GetBattlerNameTextFromQueue();

        battlerNameToDisplay.battlerNameDisplay.text = battlerNameToDisplay.BattleStats.BattlerDisplayName;
        battlerNameToDisplay.battlerNameDisplay.color = DetermineWhichColorToDisplay(battlerNameToDisplay);
        battlerNameToDisplay.battlerNameDisplay.transform.position =
            battlerNameToDisplay.BattlerLocationHandler.GetNameDisplayInScreenPos;
        battlerNameToDisplay.battlerNameDisplay.enabled = true;
    }


    private static Color DetermineWhichColorToDisplay(Battler battler)
    {
        var battlerCurrentHpPercent = battler.BattleStats.BattlerCurrentHpPercent();
        if (battlerCurrentHpPercent > 50)
            return Color.green;
        return battlerCurrentHpPercent > 25 ? Color.yellow : Color.red;

    }


    public override void StateUpdate()
    {
    }

    public override void EndState()
    {
    }

    public override void ResetState()
    {
    }

}

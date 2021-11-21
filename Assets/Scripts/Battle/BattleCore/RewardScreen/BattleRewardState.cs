using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleRewardState : BattleState
{
    private readonly List<PlayerResult> _playerResults = new List<PlayerResult>();
    private bool _isLoaded;
    private bool _isCurrentlyPlayingGainingSound;
    private bool IsFinishedPlayingAnim => _playerResults.All(playerResult => playerResult.IsFinished);

    public override void StartState(params bool[] startupBools)
    {
        var battleRewardScreen = _battleComponent.BattleGui.BattleRewardScreen;
        var expGained = _battleComponent.BattleData.EnemyBattlers.ToList()
            .Sum(battler => battler.BattleStats.BattlerExpReward);
        var playerBattlers = _battleComponent.BattleData.PlayerBattlers;

        foreach (var _battler in playerBattlers)
        {
            if (_battler is null) continue;
            var spawnedPrefab = Instantiate(battleRewardScreen.ResultGameObjectPrefab, battleRewardScreen.PrefabSpawnLocation.transform);
            var spawnedPlayerResulg = spawnedPrefab.GetComponent<PlayerResult>();
            spawnedPlayerResulg.LoadBattlerIntoPlayerResult(_battler.BattleStats, expGained);
            //PlayerClickEvent += spawnedPlayerResulg.OnPlayerClickEvent;
            _playerResults.Add(spawnedPlayerResulg);
        }
        _battleComponent.BattleGui.DisableAllCanvasForRewardScreen();

        battleRewardScreen.DisplayCanvas(true);
        _playerResults.ForEach(playerResult => playerResult.IsLoaded = true);
        StartCoroutine(WaitForLoad());
    }


    void Update()
    {
        var isAnyBattlerGainingExp = false;
        _playerResults.ForEach(playerResult =>
        {
            if (isAnyBattlerGainingExp)
                return;
            if (playerResult.IsCurrentlyGainingExp)
                isAnyBattlerGainingExp = true;
        });
        if (isAnyBattlerGainingExp)
        {
            if (!_isCurrentlyPlayingGainingSound)
            {
                SoundController.Instance.PlayBgm(SoundController.Bgm.BattleRewardGaining);
                _isCurrentlyPlayingGainingSound = true;
            }
        }
        else
        {
            if (_isCurrentlyPlayingGainingSound)
            {
                SoundController.Instance.StopBgm(SoundController.Bgm.BattleRewardGaining, false);
                _isCurrentlyPlayingGainingSound = false;
            }
        }

    }

    private void OnMouseClick()
    {
        if (_battleComponent.BattleStateMachine.CurrentBattleStateEnum != BattleStateMachine.BattleStates.BattleRewardState)
            return;
        if (_isLoaded)
            _playerResults.ForEach(result => result.OnPlayerClickEvent(this, EventArgs.Empty));
            //OnPlayerClick(this, EventArgs.Empty);
        if (IsFinishedPlayingAnim)
            _battleComponent.BattleGui.BattleTransitionComponent.StartFadeOut();

    }

    public override void StateUpdate()
    {
        throw new NotImplementedException();
    }

    public override void EndState()
    {
    }

    public override void ResetState()
    {
        throw new NotImplementedException();
    }

    private IEnumerator WaitForLoad()
    {
        yield return new WaitForSeconds(0.25f);
        _isLoaded = true;
    }

}

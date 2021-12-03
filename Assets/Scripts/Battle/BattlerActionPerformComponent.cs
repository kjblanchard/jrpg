using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class BattlerActionPerformComponent
{
    //TODO move this somewhere else...
    public static BattleProjectilePool _battleProjectilePool;


    public static void PerformAction(Battler currentBattler, Battler targetBattler, Ability abilityToPerform, int damageToCause, Action<int> applyDamageFunction, Action finishedFunction)
    {
        var sequenceToPlay = DOTween.Sequence();

        var beforeAnimSteps = HandleCastMagicInit(currentBattler, targetBattler, abilityToPerform, sequenceToPlay);
        var afterAnimSteps = HandleCastMagicEnd(currentBattler, targetBattler, abilityToPerform, sequenceToPlay);

        var steps = beforeAnimSteps.Concat(abilityToPerform.AnimationSteps).Concat(afterAnimSteps);
        foreach (var _currentAbilityAnimationStep in steps)
        {
            HandleAnimation(currentBattler, _currentAbilityAnimationStep, sequenceToPlay);
            HandlePlaySound(_currentAbilityAnimationStep, sequenceToPlay);
            HandleProjectile(currentBattler, targetBattler, _currentAbilityAnimationStep, sequenceToPlay);
            HandleTween(currentBattler, targetBattler, _currentAbilityAnimationStep, sequenceToPlay);
            HandleDamage(damageToCause, applyDamageFunction, _currentAbilityAnimationStep, sequenceToPlay);
        }


        sequenceToPlay.onComplete = () => finishedFunction();
        sequenceToPlay.Play();

    }

    private static IEnumerable<AbilityAnimStep> HandleCastMagicInit(Battler currentBattler, Battler targetBattler, Ability abilityToPerform,
        Sequence sequenceToPlay)
    {
        if (abilityToPerform.SortType != Ability.AbilitySortType.Magic) return Array.Empty<AbilityAnimStep>();
        var walkForwards = new AbilityAnimStep(AbilityAnimStep.LocationToMove.PerformingFront)
        {
            AnimLength = 0.75f,
            PlayerAnimationToStart = AbilityAnimStep.AnimToStartPlaying.Walking,
        };
        var castMagic = new AbilityAnimStep(AbilityAnimStep.LocationToMove.Default)
        {
            AnimLength = 1,
            PlayerAnimationToStart = AbilityAnimStep.AnimToStartPlaying.Casting,
            ProjectileToSpawn = Projectiles.MagicCircle,
            SoundToPlay = SoundController.Sfx.CastMagic,
        };
        return new AbilityAnimStep[]
        {
           walkForwards, castMagic
        };

    }
    private static IEnumerable<AbilityAnimStep> HandleCastMagicEnd(Battler currentBattler, Battler targetBattler, Ability abilityToPerform,
        Sequence sequenceToPlay)
    {
        if (abilityToPerform.SortType != Ability.AbilitySortType.Magic)
            return Array.Empty<AbilityAnimStep>();
        var moveBack = new AbilityAnimStep(AbilityAnimStep.LocationToMove.PerformingCenter)
        {
            AnimLength = 0.25f,
            PlayerAnimationToStart = AbilityAnimStep.AnimToStartPlaying.MoveBack,
        };
        var idleAndDamage = new AbilityAnimStep(AbilityAnimStep.LocationToMove.Default)
        {
            AnimLength = 0,
            PlayerAnimationToStart = AbilityAnimStep.AnimToStartPlaying.Idle,
            ShouldPlayDamage = true
        };
        return new[]
        {
            moveBack,
            idleAndDamage
        };
    }

    private static void HandleAnimation(Battler currentBattler, AbilityAnimStep _currentAbilityAnimationStep,
        Sequence sequenceToPlay)
    {
        if (_currentAbilityAnimationStep.PlayerAnimationToStart != AbilityAnimStep.AnimToStartPlaying.Default)
        {
            sequenceToPlay.AppendCallback(() =>
                currentBattler.BattlerAnimationComponent.ChangeAnimation(_currentAbilityAnimationStep
                    .PlayerAnimationToStart));

        }
    }

    private static void HandleDamage(int damageToCause, Action<int> applyDamageFunction,
        AbilityAnimStep _currentAbilityAnimationStep, Sequence sequenceToPlay)
    {
        if (_currentAbilityAnimationStep.ShouldPlayDamage)
            sequenceToPlay.AppendCallback(() => applyDamageFunction(damageToCause));
    }

    private static void HandleProjectile(Battler currentBattler, Battler targetBattler,
        AbilityAnimStep _currentAbilityAnimationStep, Sequence sequenceToPlay)
    {
        if (_currentAbilityAnimationStep.ProjectileToSpawn == Projectiles.Default) return;

        var obj = _battleProjectilePool.GetProjectileFromQueue(_currentAbilityAnimationStep
            .ProjectileToSpawn);
        obj.transform.position = GetSpawnLocation(obj.LocationToSpawn, currentBattler, targetBattler);
        if (_currentAbilityAnimationStep.ShouldWaitForProjectileToFinish)
        {
            PerformProjectileAnimations(currentBattler, targetBattler, obj, sequenceToPlay);
        }
        else
        {
            sequenceToPlay.AppendCallback(() => obj.StartAnimation());
        }
    }

    private static void HandleTween(Battler currentBattler, Battler targetBattler,
        AbilityAnimStep _currentAbilityAnimationStep, Sequence sequenceToPlay)
    {
        if (_currentAbilityAnimationStep.Location != AbilityAnimStep.LocationToMove.Default)
            sequenceToPlay.Append(AbilityAnimStep.GenerateTweener(currentBattler, targetBattler,
                _currentAbilityAnimationStep));
        else
            sequenceToPlay.AppendInterval(_currentAbilityAnimationStep.AnimLength);
    }

    private static void HandlePlaySound(AbilityAnimStep _currentAbilityAnimationStep, Sequence sequenceToPlay)
    {
        if (_currentAbilityAnimationStep.SoundToPlay != default)
            sequenceToPlay.AppendCallback(() =>
                SoundController.Instance.PlaySfxOneShot(_currentAbilityAnimationStep.SoundToPlay));
    }

    public static void PerformProjectileAnimations(Battler currentBattler, Battler targetBattler,
        AbilityAnimProjectile projectile, Sequence blankSequence)
    {
        foreach (var _currentAbilityAnimationStep in projectile._animationSteps)
        {
            if (_currentAbilityAnimationStep.SoundToPlay != default)
            {
                blankSequence.AppendCallback(() =>
                    SoundController.Instance.PlaySfxOneShot(_currentAbilityAnimationStep.SoundToPlay));
            }

            if (_currentAbilityAnimationStep.ProjectileAnimationToStart != StartProjectileAnimation.Default)
            {
                switch (_currentAbilityAnimationStep.ProjectileAnimationToStart)
                {
                    case StartProjectileAnimation.Default:
                        break;
                    case StartProjectileAnimation.Moving:
                        blankSequence.AppendCallback(projectile.StartMoveAnimation);
                        break;
                    case StartProjectileAnimation.Start:
                        blankSequence.AppendCallback(projectile.StartAnimation);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

            }

            if (_currentAbilityAnimationStep.Location != AbilityAnimStep.LocationToMove.Default)
            {
                blankSequence.Append(AbilityAnimStep.GenerateTweener(currentBattler, targetBattler,
                    _currentAbilityAnimationStep, projectile));
            }
            else
                blankSequence.AppendInterval(_currentAbilityAnimationStep.AnimLength);

        }
    }

    private static Vector3 GetSpawnLocation(AbilityAnimStep.LocationToMove location, Battler currentBattler, Battler targetBattler)
    {

        return location switch

        {
            AbilityAnimStep.LocationToMove.Default => new Vector3(),
            AbilityAnimStep.LocationToMove.TargetCenter => targetBattler.BattlerLocationHandler.GetBattlerLocation(BattlerLocationHandler.BattlerLocation.Default),
            AbilityAnimStep.LocationToMove.TargetRight => new Vector3(),
            AbilityAnimStep.LocationToMove.TargetLeft => new Vector3(),
            AbilityAnimStep.LocationToMove.TargetFront => new Vector3(),
            AbilityAnimStep.LocationToMove.TargetBack => new Vector3(),
            AbilityAnimStep.LocationToMove.PerformingCenter => currentBattler.BattlerLocationHandler.GetBattlerLocation(default),
            AbilityAnimStep.LocationToMove.PerformingRight => new Vector3(),
            AbilityAnimStep.LocationToMove.PerformingLeft => new Vector3(),
            AbilityAnimStep.LocationToMove.PerformingFront => currentBattler.BattlerLocationHandler.GetBattlerLocation(BattlerLocationHandler.BattlerLocation.Left) - new Vector3(0, 0.3f, 0),
            AbilityAnimStep.LocationToMove.PerformingBack => new Vector3(),
            AbilityAnimStep.LocationToMove.PerformingBottom => currentBattler.BattlerLocationHandler.GetBattlerLocation(BattlerLocationHandler.BattlerLocation.Bottom),
            AbilityAnimStep.LocationToMove.TargetFrontWithPerformingOffset => new Vector3(),
            AbilityAnimStep.LocationToMove.TargetTop => targetBattler.BattlerLocationHandler.GetBattlerLocation(BattlerLocationHandler.BattlerLocation.Top),
            AbilityAnimStep.LocationToMove.TargetBottom => targetBattler.BattlerLocationHandler.GetBattlerLocation(BattlerLocationHandler.BattlerLocation.Bottom),
            AbilityAnimStep.LocationToMove.PerformingTop => currentBattler.BattlerLocationHandler.GetBattlerLocation(BattlerLocationHandler.BattlerLocation.Top),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}



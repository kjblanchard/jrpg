using System;
using System.Runtime.InteropServices.WindowsRuntime;
using DG.Tweening;
using UnityEngine;

public class BattlerActionPerformComponent
{
    //TODO move this somewhere else...
    public static BattleProjectilePool _battleProjectilePool;


    public static void PerformAction(Battler currentBattler, Battler targetBattler, Ability abilityToPerform, int damageToCause, Action<int> applyDamageFunction, Action finishedFunction)
    {
        var sequence = DOTween.Sequence();
        foreach (var _currentAbilityAnimationStep in abilityToPerform.AnimationSteps)
        {
            if (_currentAbilityAnimationStep.SoundToPlay != default)
                sequence.AppendCallback(() =>
                    SoundController.Instance.PlaySfxOneShot(_currentAbilityAnimationStep.SoundToPlay));
            if (_currentAbilityAnimationStep.Location != AbilityAnimStep.LocationToMove.Default)
                sequence.Append(AbilityAnimStep.GenerateTweener(currentBattler, targetBattler, _currentAbilityAnimationStep));
            else
                sequence.AppendInterval(_currentAbilityAnimationStep.AnimLength);

            if (_currentAbilityAnimationStep.ProjectileToSpawn != Projectiles.Default)
            {
                var obj = _battleProjectilePool.GetProjectileFromQueue(_currentAbilityAnimationStep
                    .ProjectileToSpawn);
                obj.transform.position = obj.LocationToSpawn switch
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
                    _ => throw new ArgumentOutOfRangeException()
                };
                if (_currentAbilityAnimationStep.ShouldWaitForProjectileToFinish)
                {
                    if (obj._animationSteps.Length < 1)
                        obj.StartAnimation();
                    else
                    {
                        PerformSubAction(currentBattler, targetBattler, obj, sequence);
                    }

                }
                else
                {
                    sequence.AppendCallback(() => obj.StartAnimation());
                }
            }

            if (_currentAbilityAnimationStep.ShouldPlayDamage)
                sequence.AppendCallback(() => applyDamageFunction(damageToCause));

            if (_currentAbilityAnimationStep.AnimationToStartPlaying != AbilityAnimStep.AnimToStartPlaying.Default)
                sequence.AppendCallback(() => currentBattler.BattlerAnimationComponent.ChangeAnimation(_currentAbilityAnimationStep.AnimationToStartPlaying));
        }

        sequence.onComplete = () => finishedFunction();
        sequence.Play();

    }

    public static void PerformSubAction(Battler currentBattler, Battler targetBattler,
        AbilityAnimProjectile projectile, Sequence blankSequence)
    {
        var sequence = blankSequence;
        foreach (var _currentAbilityAnimationStep in projectile._animationSteps)
        {
            if (_currentAbilityAnimationStep.SoundToPlay != default)
                sequence.AppendCallback(() =>
                    SoundController.Instance.PlaySfxOneShot(_currentAbilityAnimationStep.SoundToPlay));
            if (_currentAbilityAnimationStep.Location != AbilityAnimStep.LocationToMove.Default)
                sequence.Append(AbilityAnimStep.GenerateTweener(currentBattler, targetBattler,
                    _currentAbilityAnimationStep, projectile));
            else
                sequence.AppendInterval(_currentAbilityAnimationStep.AnimLength);


            if (_currentAbilityAnimationStep.animationToStart != StartProjectileAnimation.Default)
            {

                switch (_currentAbilityAnimationStep.animationToStart)
                {
                    case StartProjectileAnimation.Default:
                        break;
                    case StartProjectileAnimation.Moving:
                        sequence.AppendCallback(projectile.StartMoveAnimation);
                        break;
                    case StartProjectileAnimation.Start:
                        sequence.AppendCallback(projectile.StartAnimation);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }


                sequence.onComplete = () => DOTween.Sequence();
                sequence.Play();
            }
        }
    }
}



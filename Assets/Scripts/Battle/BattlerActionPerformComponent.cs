using System;
using DG.Tweening;

public class BattlerActionPerformComponent
{
    //TODO move this somewhere else...
    public static BattleProjectilePool _battleProjectilePool;


    public static void PerformAction(Battler currentBattler, Battler targetBattler, Ability abilityToPerform, int damageToCause, Action<int> applyDamageFunction, Action finishedFunction)
    {
        var sequence = DOTween.Sequence();
        foreach (var _currentAbilityAnimationStep in abilityToPerform.AnimationSteps)
        {
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
                    AbilityAnimStep.LocationToMove.Default => throw new Exception("Wut"),
                    AbilityAnimStep.LocationToMove.Target => throw new Exception("Wut"),
                    AbilityAnimStep.LocationToMove.Home => currentBattler.transform.position,
                    AbilityAnimStep.LocationToMove.TargetCenter => targetBattler.transform.position,
                    _ => throw new ArgumentOutOfRangeException()
                };
                if (!_currentAbilityAnimationStep.ShouldWaitForProjectileToFinish)
                    sequence.AppendCallback(() =>
                    {
                        obj.StartAnimation();
                    });
            }

            if (_currentAbilityAnimationStep.ShouldPlayDamage)
                sequence.AppendCallback(() => applyDamageFunction(damageToCause));

            if (_currentAbilityAnimationStep.AnimationToStartPlaying != AbilityAnimStep.AnimToStartPlaying.Default)
                sequence.AppendCallback(() => currentBattler.BattlerAnimationComponent.ChangeAnimation(_currentAbilityAnimationStep.AnimationToStartPlaying));
        }

        sequence.onComplete = () => finishedFunction();
        sequence.Play();

    }
}

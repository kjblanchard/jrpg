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
                obj.transform.position = targetBattler.transform.position;
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

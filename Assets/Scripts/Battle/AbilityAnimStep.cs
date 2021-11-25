using System;
using UnityEngine;
using DG.Tweening;

[Serializable]
public class AbilityAnimStep
{
    public AbilityAnimStep(LocationToMove whereToMove, Vector3 offset = new Vector3())
    {
        Location = whereToMove;
        CustomOffset = offset;
        DOTween.defaultAutoPlay = AutoPlay.None;
    }

    public LocationToMove Location;
    public Vector3 CustomOffset;
    public float AnimLength = 1;
    public bool ShouldPlayDamage;
    public AnimToStartPlaying PlayerAnimationToStart;
    public Projectiles ProjectileToSpawn;
    public SoundController.Sfx SoundToPlay;
    public bool ShouldWaitForProjectileToFinish;
    public StartProjectileAnimation ProjectileAnimationToStart;


    public static Tweener GenerateTweener(Battler attackerBattlerToReference, Battler targetBattlerToReference, AbilityAnimStep stepToReference, AbilityAnimProjectile projectile = null)
    {
        var destination = stepToReference.Location switch
        {
            LocationToMove.Default => Vector3.zero,
            LocationToMove.PerformingCenter => attackerBattlerToReference.transform.position + stepToReference.CustomOffset,
            LocationToMove.TargetCenter => targetBattlerToReference.transform.position,
            LocationToMove.TargetRight => Vector3.zero,
            LocationToMove.TargetLeft => Vector3.zero,
            LocationToMove.TargetFront => HandleTargetFrontWithOffset(targetBattlerToReference, attackerBattlerToReference, stepToReference.CustomOffset),
            LocationToMove.TargetBack => Vector3.zero,
            LocationToMove.PerformingRight => Vector3.zero,
            LocationToMove.PerformingLeft => Vector3.zero,
            LocationToMove.PerformingFront => HandlePerformingFront(targetBattlerToReference, attackerBattlerToReference, stepToReference.CustomOffset),
            LocationToMove.PerformingBack => Vector3.zero,
            LocationToMove.PerformingBottom => attackerBattlerToReference.BattlerLocationHandler.GetBattlerLocation(BattlerLocationHandler.BattlerLocation.Bottom),
            LocationToMove.TargetFrontWithPerformingOffset => HandleTargetFrontWithOffset(targetBattlerToReference, attackerBattlerToReference, stepToReference.CustomOffset),
            LocationToMove.TargetTop => targetBattlerToReference.BattlerLocationHandler.GetBattlerLocation(BattlerLocationHandler.BattlerLocation.Top) + stepToReference.CustomOffset,
            LocationToMove.TargetBottom => targetBattlerToReference.BattlerLocationHandler.GetBattlerLocation(BattlerLocationHandler.BattlerLocation.Bottom),
            LocationToMove.PerformingTop => attackerBattlerToReference.BattlerLocationHandler.GetBattlerLocation(BattlerLocationHandler.BattlerLocation.Top) + stepToReference.CustomOffset,
            _ => throw new ArgumentOutOfRangeException()
        };
        return projectile == null ? attackerBattlerToReference.transform.DOMove(destination, stepToReference.AnimLength) : projectile.transform.DOMove(destination, stepToReference.AnimLength);
    }

    [Serializable]
    public enum AnimToStartPlaying
    {
        Default,
        Attack,
        Move,
        MoveBack,
        Idle,
        Victory,
        TakeDamage,
        Walking,
        Casting,
    }

    private static Vector3 HandlePerformingFront(Battler target, Battler attacker, Vector3 localOffset)
    {
        if (attacker.BattleStats.IsPlayer)
            return attacker.BattlerLocationHandler.GetBattlerLocation(BattlerLocationHandler.BattlerLocation.Left) + localOffset;
        return attacker.BattlerLocationHandler.GetBattlerLocation(BattlerLocationHandler.BattlerLocation.Right) +
               localOffset;
    }

    private static Vector3 HandleTargetFrontWithOffset(Battler target, Battler attacker, Vector3 localOffset)
    {
        if (target.BattleStats.IsPlayer)
            return target.BattlerLocationHandler.GetBattlerLocation(BattlerLocationHandler.BattlerLocation.Left) +
                   localOffset - attacker.BattlerLocationHandler.GetBattlerHalfSpriteX;
        return target.BattlerLocationHandler.GetBattlerLocation(BattlerLocationHandler.BattlerLocation.Right) +
               localOffset + attacker.BattlerLocationHandler.GetBattlerHalfSpriteX;

    }







    [Serializable]
    public enum LocationToMove
    {
        Default,
        TargetCenter,
        TargetRight,
        TargetLeft,
        TargetFront,
        TargetFrontWithPerformingOffset,
        TargetBack,
        TargetTop,
        TargetBottom,
        PerformingCenter,
        PerformingRight,
        PerformingLeft,
        PerformingFront,
        PerformingBack,
        PerformingTop,
        PerformingBottom,
    }
}

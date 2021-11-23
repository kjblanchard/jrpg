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
    public AnimToStartPlaying AnimationToStartPlaying;
    public Projectiles ProjectileToSpawn;
    public bool ShouldWaitForProjectileToFinish;


    public static Tweener GenerateTweener(Battler attackerBattlerToReference, Battler targetBattlerToReference, AbilityAnimStep stepToReference)
    {
        var destination = stepToReference.Location switch
        {
            LocationToMove.Default => Vector3.zero,
            LocationToMove.PerformingCenter => attackerBattlerToReference.transform.position + stepToReference.CustomOffset,
            LocationToMove.TargetCenter => Vector3.zero,
            LocationToMove.TargetRight => Vector3.zero,
            LocationToMove.TargetLeft => Vector3.zero,
            LocationToMove.TargetFront => HandleTargetFrontWithOffset(targetBattlerToReference, attackerBattlerToReference, stepToReference.CustomOffset),
            LocationToMove.TargetBack => Vector3.zero,
            LocationToMove.PerformingRight => Vector3.zero,
            LocationToMove.PerformingLeft => Vector3.zero,
            LocationToMove.PerformingFront => HandlePerformingFront(targetBattlerToReference, attackerBattlerToReference, stepToReference.CustomOffset),
            LocationToMove.PerformingBack => Vector3.zero,
            LocationToMove.PerformingBottom => attackerBattlerToReference.BattlerLocationHandler.GetBattlerLocation(BattlerLocationHandler.BattlerLocation.Bottom),
            LocationToMove.TargetFrontWithPerformingOffset => HandleTargetFrontWithOffset(targetBattlerToReference,attackerBattlerToReference,stepToReference.CustomOffset),
            _ => throw new ArgumentOutOfRangeException()
        };
        var tweener =
            attackerBattlerToReference.transform.DOMove(destination, stepToReference.AnimLength);
        return tweener;
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
        return attacker.BattlerLocationHandler.GetBattlerLocation(BattlerLocationHandler.BattlerLocation.Left) + localOffset;
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
        PerformingCenter,
        PerformingRight,
        PerformingLeft,
        PerformingFront,
        PerformingBack,
        PerformingBottom,
    }
}

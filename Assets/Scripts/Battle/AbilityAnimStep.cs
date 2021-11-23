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
            LocationToMove.Default => attackerBattlerToReference.transform.position,
            LocationToMove.Target =>  targetBattlerToReference.BattlerLocationHandler.GetBattlerLocation(BattlerLocationHandler.BattlerLocation.Right) + attackerBattlerToReference.BattlerLocationHandler.GetBattlerHalfSpriteX + stepToReference.CustomOffset,
            LocationToMove.Home => attackerBattlerToReference.transform.position,
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
    }







    [Serializable]
    public enum LocationToMove
    {
        Default,
        Target,
        Home,
        TargetCenter,
    }
}

using System;
using UnityEngine;

public class AbilityAnimProjectile : MonoBehaviour
{
    public event ProjectileFinishedEventArgs ProjectileFinishedEvent;
    public delegate void ProjectileFinishedEventArgs(object obj, ProjectileEventArgs e);

    [SerializeField] public Animator _projectileAnimator;
    [SerializeField] public AbilityAnimStep[] _animationSteps;
    [SerializeField] public Projectiles _projectileType;
    [SerializeField] public StartProjectileAnimation animationToStart;
    [SerializeField] public AbilityAnimStep.LocationToMove LocationToSpawn;

    private const string _projectileAnimStartTrigger = "startAnimation";
    private const string _projectileMovingStartTrigger = "startMoveAnimation";



    public void PlaySfx(SoundController.Sfx sfxToPlay) => SoundController.Instance.PlaySfxOneShot(sfxToPlay);

    public void StartMoveAnimation()
    {
        _projectileAnimator.enabled = true;
        _projectileAnimator.SetTrigger(_projectileMovingStartTrigger);
    }

    public void StartAnimation()
    {
        _projectileAnimator.enabled = true;
        _projectileAnimator.SetTrigger(_projectileAnimStartTrigger);
    }

    public void AnimationFinished()
    {
        _projectileAnimator.enabled = false;
        OnProjectileFinishedEvent(this);
    }


    private void OnProjectileFinishedEvent(object obj) => ProjectileFinishedEvent?.Invoke(obj, new ProjectileEventArgs(this.gameObject, _projectileType));
}

[Serializable]
public enum Projectiles
{
    Default,
    SwordSlash,
    Ice,
    MagicCircle,
    Bolt,
}

[Serializable]
public enum StartProjectileAnimation
{
    Default,
    Moving,
    Start,
}

public class ProjectileEventArgs : EventArgs
{
    public ProjectileEventArgs(GameObject gameobj, Projectiles projectile)
    {
        GameObject = gameobj;
        Projectile = projectile;

    }
    public GameObject GameObject;
    public Projectiles Projectile;
}

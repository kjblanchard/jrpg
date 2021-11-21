using System;
using UnityEngine;

public class AbilityAnimProjectile : MonoBehaviour
{
    public event ProjectileFinishedEventArgs ProjectileFinishedEvent;
    public delegate void ProjectileFinishedEventArgs(object obj, ProjectileEventArgs e);

    [SerializeField] public Animator _projectileAnimator;
    [SerializeField] public AbilityAnimStep[] _animationSteps;
    [SerializeField] public bool _shouldWaitForCallback;
    [SerializeField] public Projectiles _projectileType;

    private const string _projectileAnimStartTrigger = "startAnimation";



    public void PlaySfx(SoundController.Sfx sfxToPlay) => SoundController.Instance.PlaySfxOneShot(sfxToPlay);

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
    SwordSlash
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

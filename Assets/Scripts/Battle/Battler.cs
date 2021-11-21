using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class Battler : MonoBehaviour

{
    public BattleStats BattleStats { get; private set; }
    public DamageComponent BattlerDamageComponent { get; private set; }
    public StatusEffectComponent StatusEffectComponent { get; private set; }
    public BattlerGambitComponent BattlerGambitComponent { get; private set; }
    public BattlerLocationHandler BattlerLocationHandler { get; private set; }
    public SpriteRenderer spriteComp;
    public BattlerTimeManager BattlerTimeManager { get; private set; }
    public BattlerClickHandler BattlerClickHandler;
    public TMP_Text battlerNameDisplay;
    public BattlerAnimationComponent BattlerAnimationComponent => _battlerAnimComponent;
    [SerializeField] private BattlerAnimationComponent _battlerAnimComponent;


    /// <summary>
    /// The battlers stats that should not be changed, this is assigned here for ENEMIES, so that we can assign their stats.  Probably move these to json eventually
    /// </summary>
    [SerializeField] private BattlerBaseStats _battlerBaseStats;
    [SerializeField] private DOTweenAnimation _deathMoveTween;
    [SerializeField] private DOTweenAnimation _deathColorTween;
    [SerializeField] public Ability battlerAttackAbility;
    [SerializeField] public Ability battlerDefendAbility;

    private void Awake()
    {
        StatusEffectComponent = new StatusEffectComponent(this);
        BattleStats = new BattleStats(_battlerBaseStats, StatusEffectComponent);
        BattlerTimeManager = new BattlerTimeManager(BattleStats);
        BattlerDamageComponent = new DamageComponent(BattleStats);
        BattlerGambitComponent = new BattlerGambitComponent(_battlerBaseStats.GambitGroups, this);
        BattlerLocationHandler = new BattlerLocationHandler(spriteComp);

        BattlerDamageComponent.DamageCausedEvent += OnDamageTaken;

        BattlerDamageComponent.DeathCausedEvent += OnDeath;
    }

    /// <summary>
    /// This is used to assign the players base battle stats and should only be used at the beginning of the battle when it is created.
    /// </summary>
    /// <param name="playerBattleStats"></param>
    public void AssignPlayerBaseBattleStats(BattlerBaseStats playerBattleStats)
    {
        //TODO this will need to actually generate the players stats and add it to it.
        _battlerBaseStats = playerBattleStats;
    }

    public void OnDamageTaken(object obj, int e)
    {
        if (e <= 0)
            return;
        if (BattleStats.IsPlayer)
            BattlerAnimationComponent.ChangeAnimation(AbilityAnimStep.AnimToStartPlaying.TakeDamage);
        else
        {
            spriteComp.transform
                .DOLocalMove(new Vector3(-0.25f, 0.25f,spriteComp.transform.position.z),0.3f)
                .SetLoops(2, LoopType.Yoyo)
                .SetEase(Ease.OutCubic)
                .Play();


        }

    }

    public void OnDeath(object obj, EventArgs e)
    {
        if (BattleStats.IsPlayer)
            return;
        SoundController.Instance.PlaySfxOneShot(SoundController.Sfx.BattleEnemyDeath);
        //_deathMoveTween.DORestart();
        _deathColorTween.DORestart();
        //_deathMoveTween.DORestart();

    }

}

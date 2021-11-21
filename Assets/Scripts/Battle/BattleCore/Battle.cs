using UnityEngine;

public class Battle : MonoBehaviour
{
    public BattleData BattleData => _battleData;
    public BattleGui BattleGui => _battleGui;
    public BattleStateMachine BattleStateMachine => _battleStateMachine;
    public BattleProjectilePool BattleProjectilePool => _battleProjectilePool;

    [SerializeField] private BattleProjectilePool _battleProjectilePool;
    [SerializeField] private BattleGui _battleGui;
    [SerializeField] private BattleCamera _battleCamera;
    [SerializeField] private BattleData _battleData;
    [SerializeField] private BattleStateMachine _battleStateMachine;

    void Start()
    {
        _battleStateMachine.ChangeBattleState(BattleStateMachine.BattleStates.LoadingState);
    }

}

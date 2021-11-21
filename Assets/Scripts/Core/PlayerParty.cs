using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParty : MonoBehaviour
{
    public BattlerBaseStats[] CurrentParty => _playerPartyBattlers;
    [SerializeField] private BattlerBaseStats[] _playerPartyBattlers;

}

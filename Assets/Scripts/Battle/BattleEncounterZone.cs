using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// The Encounter zone - this is used to hold all of the encounters that a zone can have, and their weights
/// </summary>
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Battle/BattleEncounterZone", order = 3)]
public class BattleEncounterZone : ScriptableObject
{
    public BattleEncounter[] BattleEncounters;

    /// <summary>
    /// A battle encounter, this Has a battle group, the weight, the location, and the music to play for the battle.
    /// </summary>
    [System.Serializable]
    public class BattleEncounter
    {
        public EnemyBattleGroup BattleGroups;
        public int EncounterWeight;
        public SceneController.GameScenesEnum LocationForBattle;
        public SoundController.Bgm MusicToPlay;
    }
    /// <summary>
    /// Enum of all the battle musics that can be played when entering a battle
    /// </summary>
    [System.Serializable]
    public enum BattleMusic
    {
        Regular,
        Boss,
        Alt
    }

}


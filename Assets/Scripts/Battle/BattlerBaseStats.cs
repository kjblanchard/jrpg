using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Battle/BattlerStats", order = 1)]
public class BattlerBaseStats : ScriptableObject
{
    
    public string BattlerName;
    public int BattlerHp;
    public int BattlerMp;
    public int BattlerStr;
    public int BattlerSpd;
    public bool IsPlayer = false;
    public BattlerNames BattlerNameEnum;

    public Ability[] Abilities;

    public MultiDimensionalGambits[] GambitGroups = new MultiDimensionalGambits[3];
    public int Exp;
    public Sprite CharPortrait;
    public Color32 CharColor = new Color32(255, 0, 0,200);

    public int[] ExpRequired;

    [System.Serializable]
    public class MultiDimensionalGambits
    {
        public List<BattlerGambit> BattlerGambits;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogLine
{
    [SerializeField] public string LineToDisplay;
    [SerializeField] public string NameToDisplay = null;
    [SerializeField] public int SelectionChoice = -1;
    [SerializeField] public bool CausesSelection = false;

}

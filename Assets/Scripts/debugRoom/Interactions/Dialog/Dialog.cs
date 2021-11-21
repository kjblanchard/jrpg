using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Dialog", order = 1)]
public class Dialog : ScriptableObject
{
    [System.Serializable]
    public class DialogParams
    {

        [SerializeField] public string Dialog;
        [SerializeField] public string Name = null;
        [SerializeField] public bool IsChoice;
        [SerializeField] public string[] ChoiceOptions;
        [SerializeField] public int SelectionChoice;
    }
    [SerializeField] public DialogParams[] LinesOfDialog;
}

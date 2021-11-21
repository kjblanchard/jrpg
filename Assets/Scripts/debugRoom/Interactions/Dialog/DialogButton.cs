using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogButton : MonoBehaviour
{
    [SerializeField] private int _selectionNumber;
    [SerializeField] private TMP_Text _buttonText;
    [SerializeField] private DialogController _dialogController;

    public void ReturnSelectionNumber()
    {
        _dialogController.HandleDialogButtonPressed(_selectionNumber);
    }

    public void UpdateButtonText(string newText)
    {
        _buttonText.text = newText;
    }
}

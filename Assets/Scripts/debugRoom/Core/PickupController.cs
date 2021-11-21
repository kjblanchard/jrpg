using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

/// <summary>
/// The controller used for handling pickups, it is referenced by the player interaction handler
/// </summary>
public class PickupController : MonoBehaviour
{
    [SerializeField] private TMP_Text _textBoxToUpdate;
    [SerializeField] private DotweenBroadcasterComponent dialogBoxOpenTween;
    [SerializeField] private DotweenBroadcasterComponent dialogBoxCloseTween;
    public bool InDialog { get; private set; }
    private int _currentLocationInDialog;
    private bool _dialogBoxLoading;
    private const string _pickupOpenTweenString = "pickupBoxOpen";
    private const string _pickupCloseTweenString = "pickupBoxClose";


    private void Start()
    {

        dialogBoxOpenTween.DotweenCompleteEvent += OnDialogBoxOpenCompletedEvent;
        dialogBoxCloseTween.DotweenCompleteEvent += OnDialogBoxClosedCompletedEvent;

    }

    /// <summary>
    /// Triggers the interaction dialog for pickups
    /// </summary>
    /// <param name="pickupItem">The Scriptable object dialog that will be gone though</param>
    /// <returns>Returns true if the dialog should continue, and false if the dialog is ending</returns>
    public void TriggerInteractionDialog(PickupInteractionComponent pickupItem)
    {
        if (_dialogBoxLoading)
            return ;
        if (InDialog)
        {
            AdvanceDialog(pickupItem);
            return;

        }
        InitializeDialogInteraction(pickupItem);
    }

    private void InitializeDialogInteraction(PickupInteractionComponent dialogToGoThrough)
    {
        InDialog = true;
        _dialogBoxLoading = true;
        _currentLocationInDialog = 0;
        _textBoxToUpdate.gameObject.SetActive(false);
        var whatToSay = "You Just found " + ItemLookupDictionary[dialogToGoThrough.ItemForPickup.ItemNumber] + "!!";
        _textBoxToUpdate.text = whatToSay;
        DOTween.Play(_pickupOpenTweenString);
    }


    private void AdvanceDialog(PickupInteractionComponent dialogToGoThrough)
    {
        if (_currentLocationInDialog >= 0)
        {
            DOTween.Play(_pickupCloseTweenString);
            return;
        }
        _currentLocationInDialog++;
        DisplayDialog(dialogToGoThrough);
        return;
    }

    private void EndDialog(PickupInteractionComponent dialog)
    {
        InDialog = false;
        _currentLocationInDialog = 0;
        DOTween.Rewind(_pickupCloseTweenString);
        DOTween.Rewind(_pickupOpenTweenString);


    }
    private void DisplayDialog(PickupInteractionComponent dialogToGoThrough)
    {
        _dialogBoxLoading = false;
        _textBoxToUpdate.gameObject.SetActive(true);
    }

    //TODO This should be removed and an actual Item database should be made that can be looked up
    private Dictionary<int, string> ItemLookupDictionary = new Dictionary<int, string>
    {
        {0, "Big Sword"},
        {1, "Small Sword"}
    };
    public void OnDialogBoxOpenCompletedEvent(object thing, EventArgs e)
    {
        DisplayDialog(null);

    }
    public void OnDialogBoxClosedCompletedEvent(object thing, EventArgs e)
    {
        EndDialog(null);
    }

}

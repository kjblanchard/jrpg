using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PlayerInteractionComponent : MonoBehaviour

{
    /// <summary>
    /// If the interaction component is currently within an interaction.
    /// looks at the other controllers status each frame and updates itself
    /// </summary>
    public bool CurrentlyInteracting { get; private set; }

    [SerializeField] private DialogController _dialogController;
    [SerializeField] private PickupController _playerPickupController;
    [SerializeField] private GameObject _interactionSprite;
    private InteractableComponent _currentInteractableComponent;

    private void Update()
    {
        CurrentlyInteracting = _dialogController.InDialog || _playerPickupController.InDialog;
    }

    /// <summary>
    /// This is called by a Remote interactable component.  It will turn on the interaction sprite, and also update your current interactable
    /// </summary>
    /// <param name="newInteractable">The actual component that you will be interacting with</param>
    /// <param name="interactionSpriteStatus">If The interaction sprite should be turned on or off</param>
    public void UpdateInteraction(InteractableComponent newInteractable, bool interactionSpriteStatus)
    {
        _interactionSprite.SetActive(interactionSpriteStatus);
        _currentInteractableComponent = newInteractable;
    }

    /// <summary>
    /// This is called by the player to actually handle the interaction
    /// </summary>
    public void PerformInteraction()
    {
        if (!_currentInteractableComponent)
            return;
        switch (_currentInteractableComponent.InteractionType)
        {
            //TODO think of a way to make this not have to cast on every interaction check.
            case TypeOfInteraction.Dialog:
                var dialogCasted = (DialogInteractionComponent)_currentInteractableComponent;
                HandleDialogInteraction(dialogCasted.DialogToDisplay);
                break;
            case TypeOfInteraction.Pickup:
                var pickupCasted = (PickupInteractionComponent)_currentInteractableComponent;
                HandlePickupInteraction(pickupCasted);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void HandleDialogInteraction(Dialog dialogToDisplay)
    {
         _dialogController.HandlePlayerRightClick(dialogToDisplay);
    }

    private void HandlePickupInteraction(PickupInteractionComponent pickupComponent)
    {
        _playerPickupController.TriggerInteractionDialog(pickupComponent);
    }


}

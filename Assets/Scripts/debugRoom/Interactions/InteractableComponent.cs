using UnityEngine;

/// <summary>
/// Require circle collider for overlapping
/// </summary>
[RequireComponent(typeof(CircleCollider2D))]
public abstract class InteractableComponent : MonoBehaviour
{
    public TypeOfInteraction InteractionType { get; protected set; }

    void OnTriggerEnter2D(Collider2D collider)
    {

        if (!collider.gameObject.CompareTag("PlayerInteraction"))
            return;
        var playerInteractionComponent = collider.gameObject.GetComponent<PlayerInteractionComponent>();
        if (playerInteractionComponent)
            playerInteractionComponent.UpdateInteraction(this, true);

    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (!collider.gameObject.CompareTag("PlayerInteraction"))
            return;
        var playerInteractionComponent = collider.gameObject.GetComponent<PlayerInteractionComponent>();
        if (playerInteractionComponent)
            playerInteractionComponent.UpdateInteraction(null, false);
    }
}
public enum TypeOfInteraction
{
    Dialog,
    Pickup
}


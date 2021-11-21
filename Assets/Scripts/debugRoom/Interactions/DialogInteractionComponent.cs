using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogInteractionComponent : InteractableComponent
{
    [SerializeField] public Dialog DialogToDisplay;

    public void Awake()
    {
        InteractionType = TypeOfInteraction.Dialog;
    }
}

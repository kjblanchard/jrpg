using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupInteractionComponent : InteractableComponent
{
    [SerializeField] public ItemForPickup ItemForPickup;

    public void Awake()
    {
        InteractionType = TypeOfInteraction.Pickup;
    }
    public void Start()
    {
        LoadItem();

    }

    public void LoadItem()
    {
        //Check quest log, destroy if it is picked up.
    }
}

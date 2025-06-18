using GameSystems.Inventory;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IInteractable
{
  
    bool CanAcceptInteractionType(InteractionAttempt interactionAttempt);
    

    bool Interact(InteractionAttempt interactionAttempt);

    void OnInteractingEnd();
}

public interface IInteractor
{
    
}

public enum InteractionIntent
{
    None,
    InsertItem,
    RemoveItem,
    Interact,
    Attack,
    Build,
    Harvest_Wood,
    Harvest_Stone,
    Harvest_Plants,
    TillSoil,
    Water,
}

public class InteractionAttempt
{
    public IInteractor interactor;
    public GameItem Item { get; set; }
    public InventorySlot Slot { get; set; }
    public InteractionIntent Intent { get; set;}
    public List<InteractionIntent> Intents { get; set; } = new List<InteractionIntent>();
}



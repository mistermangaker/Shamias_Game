using GameSystems.Inventory;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IInteractable
{

    //public UnityAction<IInteractable> OnInteraction { get; set; }
    //public UnityAction<IInteractable> OnInteractionEnd { get; set; }

    bool Interact(InteractionAttempt interactionAttempt);

    void OnInteractingEnd();
}

public interface IInteractor
{
    
}

public enum InteractionIntent
{
    InsertItem,
    RemoveItem,
    Inspect,
    Interact,
    Attack,
    Build,
    Harvest_Wood,
    Harvest_Stone,
    Harvest_Plants,
    TillSoil,
    Water,
    None,
    Any,
}

public class InteractionAttempt
{
    public IInteractor interactor;
    public GameItem Item { get; set; }
    public InteractionIntent Intent { get; set;}
    public List<InteractionIntent> Intents { get; set; } = new List<InteractionIntent>();
}

public class InteractionResponse
{
    public IInteractable interactable;
    public GameItem Item { get; set; }

    public bool InteractionSuccess;
}

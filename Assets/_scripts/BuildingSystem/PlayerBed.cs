using UnityEngine;

public struct OnPlayerSleepRequested :IEvent
{
}

public class PlayerBed : MonoBehaviour, IInteractable, IToolTip, IHighLightable
{
    public bool CanAcceptInteractionType(InteractionAttempt interactionAttempt)
    {
       // Debug.Log("Getting request");
        if(interactionAttempt.Intent == InteractionIntent.Interact)
        {
            return true;
        }
        return false;
    }

    public OnToolTipRequested GetToolTip()
    {
        return new OnToolTipRequested
        {
            toolTipHeader = "sleep",
            toolTipBody = "bedtime",
            intent = InteractionIntent.Interact
        };
    }

    public void Highlight()
    {
       // Debug.Log("HighLighted");
    }

    public bool Interact(InteractionAttempt interactionAttempt)
    {
        if(interactionAttempt.Intent == InteractionIntent.Interact)
        {
            
            EventBus<OnPlayerSleepRequested>.Raise(new OnPlayerSleepRequested());
            return true;
        }
        return false;
    }

    public void OnInteractingEnd()
    {
        
    }

    public void UnHighLight()
    {
       // Debug.Log("UnHighLight");
    }
}

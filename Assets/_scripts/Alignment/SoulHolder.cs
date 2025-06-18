using GameSystems.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;


public class SoulHolder : BuildingBase
{

    [SerializeField] private ItemData SoulItem;

    [SerializeField] private SoulData soulBoundToThisBuilding;

    private List<AlignmentChangeSuggestion> suggestions = new List<AlignmentChangeSuggestion>();

   // [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();

    EventBinding<OnSoulHoldersActivate> onSoulHoldersActivate;

    protected override void Awake()
    {
        base.Awake();
        onSoulHoldersActivate = new EventBinding<OnSoulHoldersActivate>(CheckAndApplyAlignmentSuggestions);
        EventBus<OnSoulHoldersActivate>.Register(onSoulHoldersActivate);
    }


    public void AddSoulData(SoulData soulData)
    {
        soulBoundToThisBuilding = soulData;
        if (soulBoundToThisBuilding == null) return;
        EventBus<OnRegisterSoulData>.Raise(new OnRegisterSoulData
        {
            buildingId = Id,
            SoulData = soulData,
        });
    }

    public void RemoveSoulData()
    {
        soulBoundToThisBuilding = null;
        EventBus<OnDeregisterSoulData>.Raise(new OnDeregisterSoulData { buildingId = Id });
    }


    private void OnDestroy()
    {
        EventBus<OnSoulHoldersActivate>.Deregister(onSoulHoldersActivate);
    }

    public void CheckAndApplyAlignmentSuggestions()
    {
        soulBoundToThisBuilding.HandleApplyingAlignmentChanges(suggestions);
        suggestions = new List<AlignmentChangeSuggestion>();
    }

    public void AddAlignmentSuggestion(AlignmentChangeSuggestion suggestion)
    {
        suggestions.Add(suggestion);
    }
    public void AddAlignmentSuggestion(List<AlignmentChangeSuggestion> suggestions)
    {
        Debug.Log(suggestions.Count);
        this.suggestions.AddRange(suggestions);
    }
    

    [ContextMenu("debug randomly change SoulAlignment SoulAlignment")]
    public void RandomlyChangeAlignment()
    {
        int amount = Random.Range(-100, 100);
        AlignmentType type =  (AlignmentType)Random.Range(0, 14);
        Debug.Log($"adding {amount} to {type}");
        soulBoundToThisBuilding.EffectSoulALignment(amount, type);
        Debug.Log(soulBoundToThisBuilding.GetRelativeValueOfType(type));
        Debug.Log(soulBoundToThisBuilding.GetRangeValueOfType(type));
    }


    public void ChangeAlignment(AlignmentType alignment, int amount)
    {
        Debug.Log($"changing {alignment} by {amount}");
        soulBoundToThisBuilding.EffectSoulALignment(amount, alignment);
        Debug.Log(soulBoundToThisBuilding.GetRelativeValueOfType(alignment));
    }




    public override bool Interact(InteractionAttempt interactionAttempt)
    {
        Debug.Log("interact");
        Debug.Log(interactionAttempt.Intent);
        if (interactionAttempt.Intent == InteractionIntent.Interact)
        {
            GameItem item = GameItem.DefaultItem(SoulItem);
            item.SetSoulData(soulBoundToThisBuilding);
            RemoveSoulData();
            EventBus<OnDropItemAtPositionRequested>.Raise(new OnDropItemAtPositionRequested(item, transform.position, 1));
            return true;
        }
        return false;
    }

    public override void OnInteractingEnd()
    {
       
    }


    public override bool CanAcceptInteractionType(InteractionAttempt interactionAttempt)
    {
        return interactionAttempt.Intent == InteractionIntent.Interact;
    }

    public override OnToolTipRequested GetToolTip()
    {
        OnToolTipRequested tooltip = new OnToolTipRequested
        {
            intent = InteractionIntent.Interact,
            toolTipHeader = "soul boundObject",
            toolTipBody = ""
        };
        return tooltip;
    }

    public override void Damage(int damage, DamageType damageType)
    {
        if(damageType == DamageType.Slash)
        {
            DamageBuilding(damage);
        }
        else if(damageType == DamageType.Blunt) 
        {
            DamageBuilding(damage/2);
        }
    }
}


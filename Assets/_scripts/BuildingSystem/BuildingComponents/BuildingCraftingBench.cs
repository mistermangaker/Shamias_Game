using GameSystems.Inventory;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class BuildingCraftingBench : BuildingBase
{
    public override bool Interact(InteractionAttempt interactor)
    {
        Debug.Log(gameObject.name);
       
        int layermask = 1<<LayerMask.NameToLayer("InteractableLayer");

        List<InventorySystem> nearbyInventories = new List<InventorySystem>();
        List<Collider> colliders = Physics.OverlapSphere(gameObject.transform.position, 2f, layermask).ToList();
        foreach (Collider collider in colliders)
        {
            WorldSpaceInventory inv = collider.GetComponent<WorldSpaceInventory>();
            if (inv != null)
            {
                nearbyInventories.Add(inv.InventorySystem);
            }
        }
       Debug.Log(nearbyInventories.Count);
         
        EventBus<OnWorkbenchScreenRequested>.Raise(new OnWorkbenchScreenRequested
        {
            nearbyInventories = nearbyInventories,
            requestPosition = transform.position,
        });
        return true;
    }

    protected override void HandleHighLighting()
    {
        EventBus<OnIteractableHovered>.Raise(new OnIteractableHovered
        {
            interactable = this,
            interactionPosition = transform.position,
            name = "workbench"
        });
    }
   
    public override void OnInteractingEnd()
    {
        
    }

    public override OnToolTipRequested GetToolTip()
    {
        return new OnToolTipRequested
        {
            toolTipHeader = "Crafting Bench",
            intent = InteractionIntent.Interact
        };
    }

    public override void Damage(int damage, DamageType damageType)
    {
        if (damageType == DamageType.Harvest_Wood)
        {
            DamageBuilding(damage);
        }
    }
}

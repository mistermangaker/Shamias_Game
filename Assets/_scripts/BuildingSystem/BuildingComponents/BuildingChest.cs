using GameSystems.Inventory;
using UnityEngine;


public class BuildingChest : BuildingBase
{
   

    protected override void HandleHighLighting()
    {
        EventBus<OnIteractableHovered>.Raise(new OnIteractableHovered
        {
            name = "Chest",
            interactable = this,
            interactionPosition = transform.position
        });
    }

    public override bool Interact(InteractionAttempt interactor)
    {
        Debug.Log("interact");
        if (interactor.Intent == InteractionIntent.Attack||interactor.Intent == InteractionIntent.Harvest_Wood)
        {
            int damage = interactor.Slot.GameItem.GameItemData?.ItemAttackDamage ?? 0;
            DamageBuilding(damage);
        }
        else if (interactor.Intent == InteractionIntent.InsertItem)
        {
            if (buildingInventory != null)
            {
                if (buildingInventory.InventorySystem.AddToInventory(interactor.Item, 1, out int remainder))
                {
                    return true;
                }
                return false;
            }
        }
        else if (interactor.Intent == InteractionIntent.Interact)
        {
            if (buildingInventory != null)
            {
                buildingInventory.Interact(interactor.interactor);
            }
        }
        
       
       return false ;
    }

    public override void OnInteractingEnd()
    {
        
    }

    public override void RemoveBuilding()
    {
        foreach(InventorySlot slot in buildingInventory.InventorySystem.InventorySlots)
        {
            
            if (slot.ItemData!=null)
            {
                Debug.Log(slot.ItemData.ItemId);
                Debug.Log(slot.StackSize);
                DropItems(slot.GameItem, slot.StackSize);
            }
            
        }
        base.RemoveBuilding();
    }

    public override OnToolTipRequested GetToolTip()
    {
        return new OnToolTipRequested
        {
            toolTipHeader = "Chest",
            intent = InteractionIntent.Interact,
            toolTipBody = "",
        };
    }

    public override void Damage(int damage, DamageType damageType)
    {
        if(damageType == DamageType.Harvest_Wood)
        {
            DamageBuilding(damage);
        }
    }
}

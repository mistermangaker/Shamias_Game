using GameSystems.Inventory;
using UnityEngine;

public class BuildingChest : BuildingBase
{

    public override bool Interact(InteractionAttempt interactor)
    {
        if(interactor.Intents.Contains(InteractionIntent.Interact))
        {
            if (buildingInventory != null)
            {
                buildingInventory.Interact(interactor.interactor);
            }
        }
        else if (interactor.Intents.Contains(InteractionIntent.InsertItem))
        {
            if (buildingInventory != null)
            {
                if(buildingInventory.InventorySystem.AddToInventory(interactor.Item, 1, out int remainder))
                {
                    return true;
                }
                return false;
            }
        }
        else if(interactor.Intents.Contains(InteractionIntent.Attack))
        {
            RemoveBuilding();
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
}

using GameSystems.Inventory;
using UnityEngine;
using UnityEngine.EventSystems;

public class CraftingResultSlotUI : ItemSlotUI
{
    public override bool CanRecieveItemPlacementofThisType(GameItem item)
    {
        return false;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        MouseButton button = (eventData.button == PointerEventData.InputButton.Right) ? MouseButton.Right : MouseButton.Left;
        parentDisplay?.SlotClicked(this, button);
    }
}

using GameSystems.Inventory;
using UnityEngine;
using UnityEngine.EventSystems;

public class CraftingSlotsUI : ItemSlotUI
{

    [SerializeField] protected ItemFilter filter;

    public override bool CanRecieveItemPlacementofThisType(GameItem item)
    {

        if (filter != null) {

            if (filter.IsNegtiveFilter) return !filter.Contains(item.GameItemData);
            else return filter.Contains(item.GameItemData);

        }
        else return true;

    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        MouseButton button = (eventData.button == PointerEventData.InputButton.Right) ? MouseButton.Right : MouseButton.Left;
        parentDisplay?.SlotClicked(this, button);
    }
}

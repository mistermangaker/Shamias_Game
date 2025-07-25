using UnityEngine;
using UnityEngine.Events;

namespace GameSystems.Inventory
{
    public class WorldSpaceInventory : InventoryHolder
    {
        private IInteractor interactor1 = null;
        public static UnityAction<WorldSpaceInventory> OnInteraction { get; set; }
        public static UnityAction<WorldSpaceInventory> OnInteractionEnd { get; set; }

        protected override void Awake()
        {
            base.Awake();
            Id = GetComponent<BuildingSerializableGUID>().Id;
            
        }

        public void Interact(IInteractor interactor)
        {
            if (interactor1 == null)
                interactor1 = interactor;
            EventBus<OnDynamicInventoryRequested>.Raise(new OnDynamicInventoryRequested
            {
                inventorySystem =InventorySystem,
                offset = offset,
            });
            //OnDynamicInventoryDisplayRequested?.Invoke(InventorySystem, offset);
            
        }

        public void OnInteractingEnd()
        {
            OnInteractionEnd?.Invoke(this);
        }

        public void SetCurrentInteractor(IInteractor interactor)
        {
            interactor1 = interactor;
        }

        public void ClearCurrentInteractor()
        {
            interactor1 = null;
        }
    }

}

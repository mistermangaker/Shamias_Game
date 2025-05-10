using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace GameSystems.Inventory
{
    public class PlayerInventory : InventoryHolder
    {
        public static PlayerInventory Instance;
        public static UnityAction OnPlayerInventoryChanged;

        public static UnityAction<InventorySystem, int> OnPlayerBackPackDisplayRequested;

       

        protected override void Awake()
        {
            base.Awake();
            Instance = this;
        }

        public void RequestPlayerInventory()
        {
            OnPlayerBackPackDisplayRequested?.Invoke(InventorySystem, offset);
        }

        public bool AddToInventory(GameItem item, int amount)
        {
            return AddToInventory(item, amount, out int Remaining);
        }
        public bool AddToInventory(GameItem item, int amount, out int Remaining)
        {
            Remaining = 0;
            if (InventorySystem.AddToInventory(item, amount, out int remainder))
            {
                if (remainder <= 0)
                {

                    return true;
                }
                amount -= remainder;
               
            }
            Remaining = amount;
            return false;
        }


    }
}


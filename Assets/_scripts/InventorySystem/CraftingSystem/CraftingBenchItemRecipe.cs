using GameSystems.Inventory;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameSystems.Crafting
{
    [CreateAssetMenu(fileName = "CraftingBenchItemRecipe", menuName = "Scriptable Objects/CraftingBenchItemRecipe")]
    public class CraftingBenchItemRecipe : ScriptableObject
    {
        [field: SerializeField] public int Xsize {  get; private set; }
        [field: SerializeField] public int Ysize {  get; private set; }
        [field: SerializeField] public List<ItemData> RequiredItems { get; private set; } 
        [field: SerializeField] public ItemData CraftingResult { get; private set; }
        [field: SerializeField] public ItemData CraftingItemNeeded { get; private set; }

        public ItemRecipe ItemRecipe
        {
            get
            {
                return new ItemRecipe
                {
                    xSize = Xsize,
                    ySize = Ysize,
                    requiredItems = RequiredItems,
                    craftingItemNeeded = CraftingItemNeeded
                };
            }
        }


    }

    [Serializable]
    public class ItemRecipe
    {
        public int xSize;
        public int ySize;
        public List<ItemData> requiredItems;
        public ItemData craftingItemNeeded;

        public override bool Equals(object obj)
        {
            return obj is ItemRecipe && Equals((ItemRecipe)obj);
        }

        private bool Equals(ItemRecipe other)
        {

            //if (this.xSize != other.xSize)
            //{
            //    Debug.Log("not the samex");
            //    return false;
            //}
            //if (this.ySize != other.ySize)
            //{
            //    Debug.Log("not the samey");
            //    return false;
            //}
            //if (this.craftingItemNeeded != other.craftingItemNeeded)
            //{
            //    Debug.Log("not the craftingitem");
            //    return false;
            //}
            if (this.xSize != other.xSize || this.ySize != other.ySize|| this.craftingItemNeeded != other.craftingItemNeeded) return false;
            for (int i = 0; i < requiredItems.Count; i++)
            {
                if (other.requiredItems[i] != requiredItems[i])
                {
                   //Debug.Log($"not same item {other.requiredItems[i]?.ItemId} {requiredItems[i]?.ItemId}");
                    return false;
                }
            }
            return true;
        }

        public override string ToString()
        {
            string itemnames = "";
            foreach (ItemData item in requiredItems)
            {
                itemnames += item != null? item.ItemId + " " : "null ";
            }
            string craftingToolName = craftingItemNeeded != null ? craftingItemNeeded.ItemId + " " : "null ";
            return $"{xSize} {ySize} {itemnames} {craftingToolName}";
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(xSize, ySize,requiredItems,craftingItemNeeded);
        }

        public static ItemRecipe GenerateItemRecipeFromShapedCraftingGrid(int XSIZE, int YSIZE, List<ItemData> requiredItems, ItemData craftingItem)
        {
            int listsize = XSIZE * YSIZE;
            if(requiredItems.Count < listsize)
            {
                Debug.Log("fuck");
            }
            List<ItemData> list = new List<ItemData>(listsize);
            int minX = XSIZE;
            int minY = YSIZE;
            int maxX = 0;
            int maxY = 0;
            int SlotIndex = 0;
            for (int y = 1; y < YSIZE + 1; y++)
            {
                for (int x = 1; x < XSIZE + 1; x++)
                {
                    ItemData itemData = requiredItems[SlotIndex];
                    if (itemData != null)
                    {
                        if (x < minX) minX = x;
                        if (y < minY) minY = y;
                        if (x > maxX) maxX = x;
                        if (y > maxY) maxY = y;
                    }
                    SlotIndex++;
                }
            }
            SlotIndex = 0;
            for (int y = 1; y < YSIZE + 1; y++)
            {
                for (int x = 1; x < XSIZE + 1; x++)
                {
                    ItemData itemData = requiredItems[SlotIndex];
                    if (x >= minX && x <= maxX && y >= minY && y <= maxY)
                    {
                        list.Add(itemData);
                    }
                    SlotIndex++;
                }
            }
            int xSize = (maxX - minX) + 1;
            int ySize = (maxY - minY) + 1;
            return new ItemRecipe {xSize =xSize,ySize=ySize, requiredItems =list,craftingItemNeeded=craftingItem };
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(CraftingBenchItemRecipe))]
    public class ItemRecipeEditor : Editor
    {
      
    }
#endif
}


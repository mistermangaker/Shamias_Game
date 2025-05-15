using Callendar;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystems.Inventory
{
    [Serializable]
    public class ItemComponents
    {
        
    }


    [Serializable]
    public struct GameItem 
    {
        public static readonly string EmptyItemData = "empty";
        public string ItemTypeID { 
            get
            {
                if (GameItemData != null)
                {
                   return GameItemData.ItemId;
                }
                else { return EmptyItemData; }
            }
           
        }
        [field: SerializeField] public ItemData GameItemData { get; private set; }
        [field: SerializeField] public int Durability { get; private set; } 
        [field: SerializeField] public DateTimeStamp CreationTime { get; private set; }

        public List<InteractionIntent> InteractionIntents =>  GameItemData != null? GameItemData.PrimaryInteractionIntents : new List<InteractionIntent>();
        public bool IsConsumeable => GameItemData != null? GameItemData.ConsumeableOnUse : false;

        public void SetItemData(ItemData data)
        {
            GameItemData = data;
        }
        public static GameItem DefaultItem(ItemData ItemType) { return new Builder().Build(ItemType); }
        public static GameItem DefaultItem(string TypeID) {  return new Builder().Build(TypeID); }

        public static bool operator ==(GameItem left, GameItem right) => left.Equals(right);
        public static bool operator !=(GameItem left, GameItem right) => !(left == right);

        public override bool Equals(object obj)
        {
            return obj is GameItem && this.Equals((GameItem)obj);
        }

        private bool Equals(GameItem other)
        {
            return CreationTime.Equals(other.CreationTime, DateTimeStamp.ComparisionTo.ToDay) && other.GameItemData == this.GameItemData;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.ItemTypeID, this.CreationTime, this.Durability);
        }


        public class Builder
        {
            int durability =-1;
            DateTimeStamp creationTime = DateTimeStamp.Empty;
            public List<ItemComponents> itemComponents = new List<ItemComponents>();
            public Builder WithItemComponents(ItemComponents itemComponents)
            {
                this.itemComponents.Add(itemComponents);
                return this;
            }
            public Builder WithItemComponents(List<ItemComponents> itemComponents)
            {
                this.itemComponents = itemComponents;
                return this;
            }

            public Builder WithDurability(int durability)
            {
                this.durability = durability;
                return this;
            }
            public Builder WithCustomCreationTime(DateTimeStamp dateTimeStamp)
            {
                this.creationTime = dateTimeStamp;
                return this;
            }
            public GameItem Build(string itemName)
            {
                ItemData itemData = DataBase.GetItem(itemName);
                    //DataBaseManager.Instance.ItemDataBase.GetItem(itemName);
                return Build(itemData);
            }

            public GameItem Build(ItemData baseItemData)
            {
                GameItem gameItem = new GameItem
                {
                    GameItemData = baseItemData
                };
                if (baseItemData != null)
                {
                    gameItem.Durability = this.durability;
                    if (this.durability == -1) gameItem.Durability = baseItemData.DefaultDurability;
                    
                    if (baseItemData.UsesTimeStamps)
                    {
                        gameItem.CreationTime = this.creationTime;
                        if (creationTime.Equals(DateTimeStamp.Empty)) gameItem.CreationTime = TimeManager.Instance.CurrentGameTime;
                    }
                    
                    
                }
                
                return gameItem;
            }
        }

    }
}


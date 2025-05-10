using GameSystems.Inventory;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;


public class DroppedGameItem : MonoBehaviour
{
    public UnityAction OnPickUp;
    public UnityAction OnInitialized;

    public ItemData ItemData;
    public GameItem GameItem;
    public int amount = 1;

    public bool IsWorldSpawnItem = true;
    public bool CanBePickedUp = true;
    private bool pickedup = false;
    [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();

    public void SetId(SerializableGuid id)
    {
        Id = id;
    }

    public void SpawnNewItem(GameItem item,int amount, bool value = false)
    {
        Init(item, amount, value); 
        Id = SerializableGuid.NewGuid();
        OnGroundItemManager.Instance.RegisterDroppedItem(Id, GameItem, transform.position, amount);
    }

    public void Init(GameItem item, int amount, bool value =false)
    {
        ItemData = item.ItemData;
        GameItem = item;
        this.amount = amount;
        CanBePickedUp = value;
        IsWorldSpawnItem = false;
        OnInitialized?.Invoke();
    }

    private void Start()
    {
        if (IsWorldSpawnItem)
        {
            if(OnGroundItemManager.Instance.AlreadyCollectedItem(Id)) Destroy(gameObject);
        }
        else
        {
            OnGroundItemManager.Instance.RegisterDroppedItem(Id, GameItem, transform.position, amount);
        }
        OnInitialized?.Invoke();
    }

    private void OnValidate()
    {
        if(ItemData == null) return;
        GameItem = new GameItem.Builder().Build(ItemData);
        if(Id == SerializableGuid.Empty) Id = SerializableGuid.NewGuid();
        
    }


    private void OnTriggerStay(Collider collision)
    {
        if (collision.CompareTag("Player") && CanBePickedUp && !pickedup && amount>0)
        {
            PlayerInventory holder = collision.gameObject.GetComponent<PlayerInventory>();
            if (holder.AddToInventory(GameItem, amount, out int remainder))
            {
                pickedup = true;    
                amount = 0;
                
                StartCoroutine(DestroySelf());
            }
            else
            {
               
              amount = remainder;
                
            }
        }
       
    }

    
    private void OnTriggerExit(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            CanBePickedUp = true;
        }
    }

    private IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(0.5f);
        OnGroundItemManager.Instance.DeReisterDroppedItem(Id);
        if (IsWorldSpawnItem) OnGroundItemManager.Instance.AddToAlreadyCollectedWorldSpawnItems(Id);
        Destroy(gameObject);
    }

}

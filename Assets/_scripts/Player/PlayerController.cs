using GameSystems.BuildingSystem;
using GameSystems.Inventory;
using GameSystems.SaveLoad;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;


public enum PlayerMovementActionOrders
{
    None,
    GoToLocationAndDoAction,
    GoToLocationAndInteract,
    GoToLocationAndAttack
}

public struct PlayerMovementOrders : IEvent
{
    public PlayerMovementActionOrders actionOrders;
    public Vector3 destination;
    public IInteractable interactable;
    public InteractionAttempt interactionAttempt;
}

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour, IBind<PlayerSaveData>, IInteractor
{

    //public static UnityAction<InventorySlot> OnItemUsed;


    public static PlayerController Instance;

    private Rigidbody rb;

    private Vector3 _lookDirection;
    public Vector3 LookDirection => _lookDirection;

    [field: SerializeField] private PlayerSaveData _saveData;
    [SerializeField] private Transform interactionSpot;
    [SerializeField] public float MaxPlayerInteractionDistance { get; private set; } = 2f;

    private bool canMove = true;

    private Vector3 _moveDirection;
    public Vector3 MoveDirection => _moveDirection;

    private PlayerMovementOrders currentPlayerOrders;

    [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();

    public UnityAction<PlayerMovementOrders> OnPatherReachedDestination;


    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
       
    }

    EventBinding<PlayerMovementOrders> MovementOrders;

    private void Start()
    {
 
        PlayerInputManager.OnPlayerMovement += AssignMoveDirection;
        PlayerInputManager.OnPlayerLookDirection += AssignLookDirection;
        //PlayerInputManager.OnMouseButtonPerformed += HandleLocalPlayerInteraction;


        rb = GetComponent<Rigidbody>();

        MovementOrders = new EventBinding<PlayerMovementOrders>(HandleNewMovementOrders);
        EventBus<PlayerMovementOrders>.Register(MovementOrders);
    }

    private void OnDestroy()
    {
        PlayerInputManager.OnPlayerMovement -= AssignMoveDirection;
        PlayerInputManager.OnPlayerLookDirection -= AssignLookDirection;
        //PlayerInputManager.OnMouseButtonPerformed -= HandleLocalPlayerInteraction;

        EventBus<PlayerMovementOrders>.Deregister(MovementOrders);

    }

    private void FixedUpdate()
    {
        interactionSpot.position = transform.position+LookDirection;
        HandleMovement();
    }

  
    public bool PatherAtLocation
    {
        get
        {
            if (currentPlayerOrders.destination == Vector3.zero) return true;
            else
            {
                return Vector3.Distance(transform.position, currentPlayerOrders.destination) < MaxPlayerInteractionDistance;
            } 
        }
    }

    

    private bool PatherHasDestinationSet => currentPlayerOrders.destination != Vector3.zero;
    private void HandleMovement()
    {
        if (PatherHasDestinationSet)
        {
            if (PatherAtLocation)
            {

                HandleLocationInteraction(currentPlayerOrders);
              
                ClearPatherMoveDestination();
                
            }
            else
            {
                Vector3 dir = GetDirectionTo(currentPlayerOrders.destination);
                AssignLookDirection(dir);
                MoveTowards(dir);
            }
        }
        else
        {
            AssignLookDirection(_moveDirection);
            MoveTowards(_moveDirection);
        }
    }

    
   
    private void HandleNewMovementOrders(PlayerMovementOrders orders)
    {
        currentPlayerOrders = orders;
    }

    private void ClearPatherMoveDestination()
    {
        currentPlayerOrders = new();
    }

    private void MoveTowards(Vector3 direction)
    {
        if(canMove) rb.linearVelocity = direction * PlayerInternalState.Instance.PlayerMoveSpeed;
        else rb.angularVelocity = Vector3.zero;
    }

    private Vector3 GetDirectionTo(Vector3 destination)
    {
        Vector3 temp = (destination - transform.position).normalized;
        temp.y = 0;
        return temp;
    }

    public void AssignMoveDirection(Vector2 dir)
    {
        Vector3 inputdir = new Vector3(dir.x, 0, dir.y);
        _moveDirection = transform.TransformDirection(inputdir);
        _moveDirection.Normalize();
        ClearPatherMoveDestination();

    }
    public void AssignLookDirection(Vector2 dir)
    {
        _lookDirection = new Vector3(dir.x, 0f, dir.y);
    }



    private void HandleLocationInteraction(PlayerMovementOrders orders)
    {
        if(orders.actionOrders == PlayerMovementActionOrders.GoToLocationAndDoAction)
        {
            if (orders.interactionAttempt.Intent == InteractionIntent.Build)
            {
                DoConstruction(orders.interactionAttempt.Slot, orders.destination);
            }
            else if (orders.interactionAttempt.Intent == InteractionIntent.TillSoil)
            {
                TillSoil(orders.interactionAttempt.Slot, orders.destination);
            }
            if(orders.interactionAttempt.Intent == InteractionIntent.InsertItem && orders.interactable == null)
            {
                DropItemOnGround(orders.interactionAttempt.Slot, orders.destination);
            }
        }
        else if(orders.actionOrders == PlayerMovementActionOrders.GoToLocationAndInteract)
        {
            if(orders.interactable != null)
            {
                orders.interactable.Interact(orders.interactionAttempt);
                HandleItemUseage(orders.interactionAttempt.Slot);
            }
        }
       
    }

    private void DropItemOnGround(InventorySlot slot, Vector3 position)
    {
        if (slot == null || slot.ItemData == null) return;

        EventBus<OnDropItemAtPositionRequested>.Raise(new OnDropItemAtPositionRequested(slot.GameItem, PlayerController.Instance.transform.position, slot.StackSize));

        slot.ClearSlot();
       // OnItemUsed?.Invoke(slot);
    }

    private void DoConstruction(InventorySlot slot, Vector3 position)
    {
        Debug.Log("building");
        if(slot == null || slot.IsEmpty) return;
        GameItem item = slot.GameItem;
        if (ConstructionLayerManager.Instance.TryBuildBuidling(position, item.GameItemData.Buildable))
        {
            _lookDirection = Vector3.MoveTowards(transform.position, position, 1f);
            slot.RemoveFromStack(1);
            
        }
    }

    private void TillSoil(InventorySlot slot, Vector3 position)
    {
        _lookDirection = Vector3.MoveTowards(transform.position, position, 1f);
        ConstructionLayerManager.Instance.TryTilGround(position);
        HandleItemUseage(slot);
    }

    private void HandleItemUseage(InventorySlot slot)
    {
        if (slot.GameItem.IsConsumeable)
        {
            PlayerInventory.Instance.InventorySystem.RemoveFromInventory(slot, 1);
           // OnItemUsed?.Invoke(slot);
        }
    }


    private void HandleLocalPlayerInteraction()
    {
        //Debug.Log("click");
        IInteractable interactable = null;
        foreach(var collider in Physics.OverlapSphere(interactionSpot.position, 0.5f))
        {
            interactable = collider.gameObject.GetComponent<IInteractable>();
            if (interactable != null)
            {
                InteractionAttempt newAttempt = PlayerActionHandler.Instance.TryInteractingWithInteractable(interactable);
                {
                    if(newAttempt != null)
                    {
                        interactable.Interact(newAttempt);
                        return;
                    }
                }
            }
        }
        
    }


    public void EnableMovement()
    {
        canMove = true;
    }

    public void DisableMovement()
    {
        canMove = false;
    }

    public void Bind(PlayerSaveData data)
    {
        this._saveData = data;
        _saveData.Id = Id;
        transform.position = _saveData.Position;
        _lookDirection = _saveData.FactingDirections;
    }

    // saving this because its simpler than saving the data every frame
    public void Save(ref PlayerSaveData data)
    {
        _saveData.Position = transform.position;
        _saveData.FactingDirections = LookDirection;
        data = this._saveData;
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(interactionSpot.position, 0.5f);
    }

}

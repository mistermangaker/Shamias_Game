using GameSystems.BuildingSystem;
using GameSystems.Inventory;
using GameSystems.SaveLoad;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using static UnityEngine.InputSystem.InputAction;
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour, IBind<PlayerSaveData>, IInteractor
{
    public static UnityAction<InventorySlot> OnItemUsed;

    public static UnityAction<InteractionIntent> OnPlayerAction;

    public static PlayerController Instance;
    public BuildableTiles BuildableForVisuals => CurrentlyHeldItem.ItemData?.Buildable;
    private GameItem CurrentlyHeldItem;
    private InventorySlot inventorySlot;

    private Rigidbody rb;

    [SerializeField] private float movespeed =5f;
    private Vector3 _lookDirection;
    public Vector3 LookDirection => _lookDirection;

    [field: SerializeField] private PlayerSaveData _saveData;

    private Vector3 _moveDirection;
    public Vector3 MoveDirection => _moveDirection;

    [SerializeField] private float interactCircleSize = 1f;
    [SerializeField] private float cameraLookOffset = 0.5f;
    [SerializeField] private Transform interactionSpot;
    [SerializeField] private Transform cameraLookSpot;
    [SerializeField] private LayerMask interactLayerMask;

    [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();

   // public BuildingPlacer buildingPlacer;

    private IInteractable currentlySelectedInteractable;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        PlayerInputManager.OnPlayerMovement += AssignMoveDirection;
        PlayerInputManager.OnPlayerLookDirection += AssignLookDirection;
        PlayerInputManager.OnInteractionPerformed += PerFormInteraction;
        PlayerInputManager.OnAlternateInteractionPerformed += PerFormAltInteraction;


        PlayerInputManager.OnMouseButtonPerformed += OnMouseAction;

        // PlayerInputManager.UnityActionMouseButtonPerformed += 

        rb = GetComponent<Rigidbody>();
        //buildingPlacer = GetComponent<BuildingPlacer>();
    }

    private void OnDestroy()
    {
        PlayerInputManager.OnPlayerMovement -= AssignMoveDirection;
        PlayerInputManager.OnPlayerLookDirection -= AssignLookDirection;
        PlayerInputManager.OnInteractionPerformed -= PerFormInteraction;
        PlayerInputManager.OnAlternateInteractionPerformed -= PerFormAltInteraction;

        PlayerInputManager.OnMouseButtonPerformed -= OnMouseAction;

    }

    private void FixedUpdate()
    {
        rb.linearVelocity = _moveDirection * movespeed;
        interactionSpot.transform.position = transform.position + _lookDirection;
        cameraLookSpot.position = transform.position + _lookDirection * cameraLookOffset;
        Collider collider = Physics.OverlapSphere(interactionSpot.position, interactCircleSize, interactLayerMask).FirstOrDefault();
        if (collider != null)
        {
            
            currentlySelectedInteractable = collider.gameObject.GetComponent<IInteractable>();
        }
        else
        {
            currentlySelectedInteractable = null;
        }
    }

    private void AssignMoveDirection(Vector2 dir)
    {
        _moveDirection = new Vector3(dir.x, 0f, dir.y);
    }
    private void AssignLookDirection(Vector2 dir)
    {
        _lookDirection = new Vector3(dir.x, 0f, dir.y);
    }

    public void OnMouseAction()
    {
        if(CurrentlyHeldItem.ItemData != null)
        {
            OnPlayerAction?.Invoke(CurrentlyHeldItem.InteractionIntent);
            
            if(CurrentlyHeldItem.ItemData.Buildable != null && CurrentlyHeldItem.ItemData.InteractionIntents.Contains(InteractionIntent.Build))
            {
                if (ConstructionLayerManager.Instance.TryBuildBuidling(PlayerInputManager.Instance.MouseToGroundPlane, CurrentlyHeldItem.ItemData.Buildable))
                {
                    PlayerInventory.Instance.InventorySystem.RemoveFromInventory(inventorySlot, 1);
                    OnItemUsed?.Invoke(inventorySlot);
                }
            }
        }
        if(currentlySelectedInteractable != null )
        {
            Debug.Log(currentlySelectedInteractable);
            HandleInteraction();
        }
        
        
    }

    public void HandleInteraction()
    {
        InteractionAttempt interaction = new InteractionAttempt();
        interaction.interactor = this;
       
        if (CurrentlyHeldItem.ItemData != null)
        {
            interaction.Item = CurrentlyHeldItem;
            interaction.Intent = CurrentlyHeldItem.InteractionIntent;
        }
        else
        {
            interaction.Item = CurrentlyHeldItem;
            interaction.Intents.Add(InteractionIntent.Interact);
        }
        if (currentlySelectedInteractable.Interact(interaction))
        {
            if (CurrentlyHeldItem.IsConsumeable)
            {
                Debug.Log("using item");
                PlayerInventory.Instance.InventorySystem.RemoveFromInventory(inventorySlot, 1);
                //inventorySlot.RemoveFromStack(1);
                OnItemUsed?.Invoke(inventorySlot);
            }
        }
    }


    private void PerFormInteraction()
    {
        if (currentlySelectedInteractable != null)
        {
            InteractionAttempt interactionAttempt = new InteractionAttempt();
            interactionAttempt.Item = CurrentlyHeldItem;
            if (CurrentlyHeldItem.ItemData != null)
            {
                interactionAttempt.Intents = CurrentlyHeldItem.ItemData.InteractionIntents;
            }
            else
            {
                interactionAttempt.Intents.Add(InteractionIntent.Interact);
            }
            interactionAttempt.Intent = InteractionIntent.Interact;
            interactionAttempt.interactor = this;
            currentlySelectedInteractable.Interact(interactionAttempt);
        }
    }
    private void PerFormAltInteraction()
    {
        if (currentlySelectedInteractable != null)
        {
            InteractionAttempt interactionAttempt = new InteractionAttempt();
            interactionAttempt.Item = CurrentlyHeldItem;
            if(CurrentlyHeldItem.ItemData != null)
            {
                interactionAttempt.Intents = CurrentlyHeldItem.ItemData.InteractionIntents;
            }
            else
            {
                interactionAttempt.Intents.Add(InteractionIntent.Interact);
            }
            
            interactionAttempt.Intent = InteractionIntent.Interact;
            interactionAttempt.interactor = this;
            currentlySelectedInteractable.Interact(interactionAttempt);
        }
    }


    public void SetCurrentlyHeldItem(InventorySlot slot)
    {
        
        inventorySlot = slot;
        CurrentlyHeldItem = slot.GameItem;
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
        Gizmos.DrawWireSphere(interactionSpot.position, 0.5f);
    }

}

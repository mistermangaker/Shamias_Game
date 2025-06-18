using GameSystems.BuildingSystem;
using GameSystems.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;


public class InteractableManager : MonoBehaviour
{
    public InteractableManager Instance { get; private set; }
    [SerializeField] private LayerMask mask;
    [SerializeField] private Transform interactionSpot;
    [SerializeField] private float MaxPlayerInteractionDistance = 2f;

    private IInteractable _mouseHoveredinteractable;
    private IToolTip _mouseHoveredTooltip;
    private IHighLightable _mouseHoveredhighLight;
    private IDamagable _damageable;

    private GameObject selectedGameObject;
    private Vector3 selectedGameObjectPosition => selectedGameObject != null ? selectedGameObject.transform.position : Vector3.zero;

    private EventBinding<OnPlayerEquipedItemChanged> playerItemChanged;
    public BuildableTiles BuildableForVisuals => slot?.GameItem.GameItemData?.Buildable;
    private bool hasGameItem => (slot != null && slot?.ItemData != null) ? true : false;
    private GameItem currentlyHeldItem => slot.GameItem;
    private InventorySlot slot;


    private IInteractable nearestInteractableToPlayer;


    private void Awake()
    {
        Instance = this;
        playerItemChanged = new EventBinding<OnPlayerEquipedItemChanged>((OnPlayerEquipedItemChanged item) => slot = item.Slot);
        EventBus<OnPlayerEquipedItemChanged>.Register(playerItemChanged);
    }

    private void Start()
    {
        PlayerInputManager.OnMouseButtonPerformed += HandleInteractionOrder;
        PlayerInputManager.OnInteractionPerformed += HandleLocalPlayerInteraction;

       PlayerController.Instance.OnPatherReachedDestination += HandlePatherAtLocation;
    }

    private void OnDestroy()
    {
        PlayerInputManager.OnMouseButtonPerformed -= HandleInteractionOrder;
        PlayerInputManager.OnInteractionPerformed -= HandleLocalPlayerInteraction;


        EventBus<OnPlayerEquipedItemChanged>.Deregister(playerItemChanged);

        PlayerController.Instance.OnPatherReachedDestination -= HandlePatherAtLocation;
    }

    private void FixedUpdate()
    {
       // if(UIManager.Instance.AnyInteractionBlockingWindowsOpen) return;
        MouseToGroundPlane();
        GetClosestInteractableToPlayer();
    }
    private void HandlePatherAtLocation(PlayerMovementOrders orders)
    {
        if (orders.actionOrders == PlayerMovementActionOrders.GoToLocationAndDoAction)
        {
            if (orders.interactionAttempt.Intent == InteractionIntent.Build)
            {
                DoConstruction(orders.interactionAttempt.Slot, orders.destination);
            }
            else if (orders.interactionAttempt.Intent == InteractionIntent.TillSoil)
            {
                TillSoil(orders.interactionAttempt.Slot, orders.destination);
            }
            if (orders.interactionAttempt.Intent == InteractionIntent.InsertItem && orders.interactable == null)
            {
                DropItemOnGround(orders.interactionAttempt.Slot, orders.destination);
            }
        }
        else if (orders.actionOrders == PlayerMovementActionOrders.GoToLocationAndInteract)
        {
            if (orders.interactable != null)
            {
                orders.interactable.Interact(orders.interactionAttempt);
                HandleItemUseage(orders.interactionAttempt.Slot);
            }
        }

    }

    private void HandleInteractionOrder()
    {
        if (MouseObjectUI.Instance.IsEmpty && _mouseHoveredinteractable != null)
        {
            if (!slot.IsEmpty&&slot.ItemData.PrimaryInteractionIntent == InteractionIntent.Attack) Attack(slot, interactionSpot.position);
            InteractionAttempt attempt = TryInteractingWithInteractable(_mouseHoveredinteractable);
            if (attempt == null) 
            {
                
                return;
            }
            Vector3 interactablePoition = selectedGameObjectPosition;
            if (PlayerIsWithinRangeOfLocation(interactablePoition))
            {
                _mouseHoveredinteractable.Interact(attempt);
            }
            else
            {
                EventBus<PlayerMovementOrders>.Raise(new PlayerMovementOrders
                {
                    actionOrders = PlayerMovementActionOrders.GoToLocationAndInteract,
                    interactable = _mouseHoveredinteractable,
                    destination = interactablePoition,
                    interactionAttempt = attempt,
                });
                return;
            }
           
        }
        else if (!MouseObjectUI.Instance.IsEmpty && !MouseObjectUI.IsPointerOverUIObject())
        {
            HandleMouseItemUseage();
        }
        else if (!slot.IsEmpty)
        {
            Vector3 mouseTogroundPlane = PlayerInputManager.Instance.MouseToGroundPlane;
            if (PlayerIsWithinRangeOfLocation(mouseTogroundPlane))
            {
                if (currentlyHeldItem.PrimaryInteractionIntent == InteractionIntent.TillSoil)
                {
                    TillSoil(slot, mouseTogroundPlane);
                }
                else if (currentlyHeldItem.PrimaryInteractionIntent == InteractionIntent.Build)
                {
                    DoConstruction(slot, mouseTogroundPlane);
                }
                else if(currentlyHeldItem.PrimaryInteractionIntent == InteractionIntent.Water)
                {
                    WaterSoil(slot, mouseTogroundPlane);
                }
            }
            if(slot.ItemData.PrimaryInteractionIntent == InteractionIntent.Attack)
            {
                Attack(slot, interactionSpot.position);
            }
        }
    }

    private void HandleMouseItemUseage()
    {
        if(_mouseHoveredinteractable == null)
        {
            Vector3 mouseTogroundPlane = PlayerInputManager.Instance.MouseToGroundPlane;
            if (PlayerIsWithinRangeOfLocation(mouseTogroundPlane))
            {
                DropItemOnGround(MouseObjectUI.Instance.InventorySlot, mouseTogroundPlane);
            }
            else
            {
                PlayerMovementOrders movementOrders = new PlayerMovementOrders();
                movementOrders.interactable = null;
                InteractionAttempt newAttempt = new InteractionAttempt();

                GameItem mouseItem = MouseObjectUI.Instance.SlotItem;
                newAttempt.Slot = MouseObjectUI.Instance.InventorySlot;
                movementOrders.destination = mouseTogroundPlane;
                movementOrders.actionOrders = PlayerMovementActionOrders.GoToLocationAndDoAction;
            }
        }
        else
        {
            InteractionAttempt newAttempt = TryInteractingWithInteractable(_mouseHoveredinteractable);
            if (newAttempt == null) return;
            if (PlayerIsWithinRangeOfLocation(selectedGameObjectPosition))
            {
                _mouseHoveredinteractable.Interact(newAttempt);
            }
            else
            {
                EventBus<PlayerMovementOrders>.Raise(new PlayerMovementOrders
                {
                    actionOrders = PlayerMovementActionOrders.GoToLocationAndInteract,
                    interactable = _mouseHoveredinteractable,
                    destination = selectedGameObjectPosition,
                    interactionAttempt = newAttempt
                });
            }
            Unhover();    
        }
       
    }

    private void MouseToGroundPlane()
    {
        Vector3 pos = PlayerInputManager.Instance.MousePosition;
        pos.z = Camera.main.nearClipPlane;
        Ray ray = Camera.main.ScreenPointToRay(pos);

        Debug.DrawRay(ray.origin, ray.direction * 100, Color.green);
        RaycastHit[] hit = Physics.RaycastAll(ray, 100, mask);
        RaycastHit2D[] hit2D = Physics2D.RaycastAll(ray.origin, ray.direction);
        
        foreach (RaycastHit h in hit)
        {
            IInteractable interactable = h.collider.gameObject.GetComponentInParent<IInteractable>();
            if (interactable != null)
            {
                if(_mouseHoveredinteractable == interactable) return;
                selectedGameObject =h.collider.gameObject;
                _mouseHoveredinteractable = interactable;
                EventBus<OnToolTipUnrequested>.Raise(new OnToolTipUnrequested());
                _mouseHoveredhighLight?.UnHighLight();
                _mouseHoveredhighLight = null;
                _mouseHoveredTooltip = null;
                _damageable = null;

                IToolTip toolTip = selectedGameObject.GetComponentInParent<IToolTip>();
                if (toolTip != null)
                {
                    _mouseHoveredTooltip = toolTip;
                    EventBus<OnToolTipRequested>.Raise(_mouseHoveredTooltip.GetToolTip());
                }
                IHighLightable highlight = selectedGameObject.GetComponentInParent<IHighLightable>();
                if (highlight != null)
                {
                    _mouseHoveredhighLight = highlight;
                    _mouseHoveredhighLight.Highlight();
                }
                IDamagable damagable = selectedGameObject.GetComponentInParent<IDamagable>();
                if(damagable != null)
                {
                    _damageable = damagable;
                }
                return;
            }
        }
       
       Unhover();
        
    }

    private void Unhover()
    {
        _mouseHoveredinteractable = null;
        EventBus<OnToolTipUnrequested>.Raise(new OnToolTipUnrequested());
        _mouseHoveredTooltip = null;
        _mouseHoveredhighLight?.UnHighLight();
        _mouseHoveredhighLight = null;
        selectedGameObject = null;
        _damageable = null;
    }

    private void HandleLocalPlayerInteraction()
    {
        if(nearestInteractableToPlayer == null) return; 
        nearestInteractableToPlayer.Interact(TryInteractingWithInteractable(nearestInteractableToPlayer));
    }

    public bool PlayerIsWithinRangeOfLocation(Vector3 location)
    {
        return Vector3.Distance(PlayerController.Instance.transform.position, location) < MaxPlayerInteractionDistance;
    }

    private void GetClosestInteractableToPlayer()
    {
        List<GameObject> list = new List<GameObject>();
        foreach (var collider in Physics.OverlapSphere(PlayerController.Instance.transform.position, 1.5f))
        {
            IInteractable newinteractable = collider.gameObject.GetComponent<IInteractable>();
            if (newinteractable != null)
            {
                list.Add(collider.gameObject);
            }
        }
        if(list.Count > 0)
        {
            nearestInteractableToPlayer = list.OrderByDescending(i => Vector3.Distance(i.transform.position, PlayerController.Instance.transform.position)).FirstOrDefault().GetComponent<IInteractable>();
        }
        else
        {
            nearestInteractableToPlayer=null;
        }

        if (nearestInteractableToPlayer != null)
        {
            // show neareast tooltip
        }
        
    }





    private void DropItemOnGround(InventorySlot slot, Vector3 position)
    {
        if (slot == null || slot.ItemData == null) return;

        EventBus<OnDropItemAtPositionRequested>.Raise(new OnDropItemAtPositionRequested(slot.GameItem, PlayerController.Instance.transform.position, slot.StackSize));

        slot.ClearSlot();
        
    }

    private void DoConstruction(InventorySlot slot, Vector3 position)
    {
        Debug.Log("building");
        if (slot == null || slot.IsEmpty) return;
        GameItem item = slot.GameItem;
        if (ConstructionLayerManager.Instance.TryBuildBuidling(position, item.GameItemData.Buildable))
        {
            LookAt(position);
            slot.RemoveFromStack(1);
        }
    }
    private void Attack(InventorySlot slot, Vector3 position)
    {
        LookAt(position);
        PlayerAttack.Instance.MeleeAttack(position, slot);
        HandleItemUseage(slot);
    }

    private void TillSoil(InventorySlot slot, Vector3 position)
    {
        LookAt(position);
        ConstructionLayerManager.Instance.TryTilGround(position);
        HandleItemUseage(slot);
    }

    private void WaterSoil(InventorySlot slot, Vector3 position)
    {
        LookAt(position);
        ConstructionLayerManager.Instance.TryWaterGround(position);
        HandleItemUseage(slot);
    }
    private void LookAt(Vector3 position)
    {
        PlayerController.Instance.AssignLookDirection(Vector3.MoveTowards(transform.position, position, 1f));
    }
    private void HandleItemUseage(InventorySlot slot)
    {
        if (slot.GameItem.IsConsumeable)
        {
            PlayerInventory.Instance.InventorySystem.RemoveFromInventory(slot, 1);
        }
    }

   

    public InteractionAttempt TryInteractingWithInteractable(IInteractable interactable)
    {
        if (interactable == null) return null;
        InteractionAttempt newAttempt = new InteractionAttempt();
        if (hasGameItem)
        {
            InteractionAttempt temp = new InteractionAttempt();
            temp.Intent = currentlyHeldItem.PrimaryInteractionIntent;
            temp.Slot = slot;

            if (interactable.CanAcceptInteractionType(temp))
            {
                newAttempt.Intent = currentlyHeldItem.PrimaryInteractionIntent;
                newAttempt.Slot = slot;
            }
            else
            {
                newAttempt.Intent = InteractionIntent.Interact;
                newAttempt.Slot = slot;
            }

        }
        else
        {
            InteractionAttempt temp = new InteractionAttempt
            {
                Intent = InteractionIntent.RemoveItem,

            };

            if (interactable.CanAcceptInteractionType(temp))
            {
                newAttempt.Intent = InteractionIntent.RemoveItem;
                newAttempt.Slot = slot;
            }
            else
            {
                newAttempt.Intent = InteractionIntent.Interact;
                newAttempt.Slot = slot;
            }
        }
        return newAttempt;
    }
}

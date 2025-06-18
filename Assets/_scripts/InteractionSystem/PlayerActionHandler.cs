using GameSystems.BuildingSystem;
using GameSystems.Inventory;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public struct OnIteractableHovered : IEvent
{
    public string name;
    public List<InteractionIntent> possibleIntents;
    public IInteractable interactable;
    public Vector3 interactionPosition;
    public GameObject gameobject;
    public Buildable Buildable;
}
public struct OnIteractableUnHovered : IEvent
{

}
public class PlayerActionHandler : MonoBehaviour
{
    [SerializeField] public Material highlightMaterial;
    public static PlayerActionHandler Instance;

    

    [SerializeField] private InteractableHoverScript script;
    public BuildableTiles BuildableForVisuals => slot?.GameItem.GameItemData?.Buildable;

    private EventBinding<OnIteractableHovered> onInteractableHovered;
    private EventBinding<OnIteractableUnHovered> onInteractableUnHovered;
    EventBinding<OnPlayerEquipedItemChanged> playerItemChanged;

    private IInteractable interactable;
    private Vector3 interactableDestination;

    private bool hasGameItem => (slot != null && slot?.ItemData != null) ? true : false;
    private GameItem currentlyHeldItem => slot.GameItem;
    private InventorySlot slot;

    
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        PlayerInputManager.OnMouseButtonPerformed += HandleInteractionOrder;
       
    }
    private void OnEnable()
    {
        playerItemChanged = new EventBinding<OnPlayerEquipedItemChanged>(HandleNewItemEquiped);
        EventBus<OnPlayerEquipedItemChanged>.Register(playerItemChanged);
        onInteractableHovered = new EventBinding<OnIteractableHovered>(HandleHover);
        EventBus<OnIteractableHovered>.Register(onInteractableHovered);
        onInteractableUnHovered = new EventBinding<OnIteractableUnHovered>(UnHover);
        EventBus<OnIteractableUnHovered>.Register(onInteractableUnHovered);
    }
    private void OnDisable()
    {
        PlayerInputManager.OnMouseButtonPerformed -= HandleInteractionOrder;
        EventBus<OnPlayerEquipedItemChanged>.Deregister(playerItemChanged);
        EventBus<OnIteractableHovered>.Deregister(onInteractableHovered);
        EventBus<OnIteractableUnHovered>.Deregister(onInteractableUnHovered);
       
    }

    public void Update()
    {
        MouseToGroundPlane();
    }

   
    private void HandleNewItemEquiped(OnPlayerEquipedItemChanged item)
    {
        slot = item.Slot;
    }
    private void HandleInteractionOrder()
    {
        if (MouseObjectUI.Instance.IsEmpty && interactable != null)
        {
            HandleInteractingWithInteractable(interactable);
        }
        else if (!MouseObjectUI.Instance.IsEmpty&&!IsPointerOverUIObject())
        {
            HandleMouseItemUseage();
        }
    }

    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    public InteractionAttempt TryInteractingWithInteractable(IInteractable interactable)
    {
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

    private void HandleInteractingWithInteractable(IInteractable interactable)
    {
        
        EventBus<PlayerMovementOrders>.Raise(new PlayerMovementOrders
        {
            actionOrders = PlayerMovementActionOrders.GoToLocationAndInteract,
            interactable = interactable,
            destination = interactableDestination,
            interactionAttempt = TryInteractingWithInteractable(interactable),
        });
    }


    private void HandleMouseItemUseage()
    {
        if (interactable == null)
        {
            PlayerMovementOrders movementOrders = new PlayerMovementOrders();
            movementOrders.interactable = null;

            InteractionAttempt newAttempt = new InteractionAttempt();
            GameItem mouseItem = MouseObjectUI.Instance.SlotItem;
            newAttempt.Slot = MouseObjectUI.Instance.InventorySlot;
            movementOrders.destination = PlayerInputManager.Instance.MouseToGroundPlane;
            movementOrders.actionOrders = PlayerMovementActionOrders.GoToLocationAndDoAction;
            if (PlayerInputManager.Instance.IsAltActionButtonDown)
            {
                newAttempt.Intent = InteractionIntent.InsertItem;
            }
            else
            {
                if (mouseItem.PrimaryInteractionIntent == InteractionIntent.Build)
                {
                    newAttempt.Intent = InteractionIntent.Build;
                }
                else if (mouseItem.PrimaryInteractionIntent == InteractionIntent.TillSoil)
                {
                    newAttempt.Intent = InteractionIntent.TillSoil;
                }
            }
           
            movementOrders.interactionAttempt = newAttempt;
            EventBus<PlayerMovementOrders>.Raise(movementOrders);
        }
        else if (interactable != null)
        {
            
            InteractionAttempt newAttempt = new InteractionAttempt();

            InteractionAttempt temp = new InteractionAttempt();
            if (hasGameItem)
            {
                temp = new InteractionAttempt
                {
                    Intent = InteractionIntent.InsertItem,
                    Item = slot.GameItem,
                };
            }
            else
            {
                temp = new InteractionAttempt
                {
                    Intent = InteractionIntent.RemoveItem,
                    Item = slot.GameItem,
                };
            }

            if (interactable.CanAcceptInteractionType(temp))
            {
                newAttempt.Intent = InteractionIntent.InsertItem;
                newAttempt.Slot = slot;
            }
            else
            {
                newAttempt.Intent = InteractionIntent.Interact;
                newAttempt.Slot = slot;
            }
            EventBus<PlayerMovementOrders>.Raise(new PlayerMovementOrders
            {
                actionOrders = PlayerMovementActionOrders.GoToLocationAndInteract,
                interactable = interactable,
                destination = interactableDestination,
                interactionAttempt = newAttempt
            });
        }
        UnHover();
    }

    [SerializeField] private LayerMask mask;
    private HighlightableSprite _MouseToGroundPlane;
    public HighlightableSprite MouseToGroundPlane()
    {
        
            Vector3 pos = PlayerInputManager.Instance.MousePosition;
            pos.z = Camera.main.nearClipPlane;
            Ray ray = Camera.main.ScreenPointToRay(pos);

            Debug.DrawRay(ray.origin, ray.direction * 100, Color.green);
            RaycastHit[] hit = Physics.RaycastAll(ray, 100, mask);
            RaycastHit2D[] hit2D = Physics2D.RaycastAll(ray.origin, ray.direction);
            
            foreach (RaycastHit h in hit)
            {
                HighlightableSprite highlight = h.collider.gameObject.GetComponent<HighlightableSprite>();
                if (highlight != null && _MouseToGroundPlane != highlight)
                {
                    _MouseToGroundPlane?.UnHover();
                    _MouseToGroundPlane = highlight;
                    highlight.Hover();
                    return highlight;
                }
            else if (highlight == _MouseToGroundPlane)
            {
                return _MouseToGroundPlane;
            }
        }
            
            _MouseToGroundPlane?.UnHover();
            _MouseToGroundPlane = null;
            return null;
        
    }


    private void UnHover()
    {
        interactable = null;
        interactableDestination = Vector3.zero;
        script.ClearText();
    }
    private void HandleHover(OnIteractableHovered info)
    {
        interactable = info.interactable;
        interactableDestination = info.interactionPosition;
        string possibleIntents = "";
        possibleIntents = info.possibleIntents?[0].ToString();
        script.SetText(info.name, possibleIntents);
    }





}

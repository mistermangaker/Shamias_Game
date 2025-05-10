using GameSystems.Inventory;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryUIController : MonoBehaviour
{
    public static InventoryUIController Instance;
    public DynamicInventoryDisplay inventoryPanel;
    public DynamicInventoryDisplay playerBackPack;
    [SerializeField] private ItemHoverScript itemHoverScript;
    //public List<DynamicInventoryDisplay> currentlyOpenInventories = new List<DynamicInventoryDisplay>();

    public bool InventoryPanelIsOpen
    {
        get
        {
            return inventoryPanel.gameObject.activeInHierarchy;
        }
    }

    public bool PlayerBackPackIsOpen
    {
        get
        {
            return playerBackPack.gameObject.activeInHierarchy;
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        //InventoryHolder.OnDynamicInventoryDisplayRequested += DisplayInventory;
        //PlayerInventory.OnPlayerBackPackDisplayRequested += DisplayBackPack;
        InventorySlotUI.OnHoverRequested += DisplayHoverPanel;
        InventorySlotUI.OnHoverExited += HideHoverPanel;


    }
    private void OnDisable()
    {
        //InventoryHolder.OnDynamicInventoryDisplayRequested -= DisplayInventory;
        //PlayerInventory.OnPlayerBackPackDisplayRequested -= DisplayBackPack;
        InventorySlotUI.OnHoverRequested -= DisplayHoverPanel;
        InventorySlotUI.OnHoverExited -= HideHoverPanel;
    }

    public void DisplayInventory(InventorySystem invToDisplay, int offset)
    {
       // Debug.Log(inventoryPanel.gameObject?.name);
        inventoryPanel.gameObject.SetActive(true);
        inventoryPanel.RefreshDynamicInventoryDisplay(invToDisplay,offset);
        PlayerInventory.Instance?.RequestPlayerInventory();
    }


    public void DisplayBackPack(InventorySystem inventory, int offset)
    {
        playerBackPack.gameObject?.SetActive(true);
        playerBackPack.RefreshDynamicInventoryDisplay(inventory, offset);
    }

    public void ClosePlayerBackBack()
    {
        playerBackPack.gameObject.SetActive(false);
    }


    public void CloseInventoryScreen()
    {
        inventoryPanel.gameObject.SetActive(false);
        ClosePlayerBackBack();
        IInteractable interactable = inventoryPanel.gameObject.GetComponent<IInteractable>();
        if (interactable != null)
        {
            interactable.OnInteractingEnd();
        }
    }

    private void DisplayHoverPanel(GameItem item)
    {
        itemHoverScript.SetItem(item);
    }
    private void HideHoverPanel()
    {
        itemHoverScript.ClearItem();
    }
}

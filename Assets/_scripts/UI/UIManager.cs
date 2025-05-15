using GameSystems.Inventory;
using GameSystems.ShopSystem;
using System;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [SerializeField] private InventoryUIController _InventoryUIController;
    [SerializeField] private PauseMenuManager _PauseMenuManager;
    [SerializeField] private ShopKeeperDisplayUI _shopKeeperDisplayUI;
    [SerializeField] private HotBarDisplayScript _hotBarDisplayScript;
 
    public bool AnyInteractionBlockingWindowsOpen
    {
        get
        {
            //return true;
            return InventoryUIController.Instance.InventoryPanelIsOpen || ShopKeeperDisplayUI.Instance.IsShopScreenOpen;
        }
    }

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        //_InventoryUIController = InventoryUIController.Instance;
        //_PauseMenuManager = PauseMenuManager.Instance;
        //_shopKeeperDisplayUI = ShopKeeperDisplayUI.Instance;
        //_hotBarDisplayScript = HotBarDisplayScript.Instance;
        //Debug.Log(_InventoryUIController);
        //Debug.Log(_PauseMenuManager);
        //Debug.Log(_shopKeeperDisplayUI);
        //Debug.Log(_hotBarDisplayScript);
    }

    private void OnEnable()
    {
        ShopKeeper.OnShopWindowRequested += DisplayShowWindow;

        InventoryHolder.OnDynamicInventoryDisplayRequested += DisplayWorldSpaceInventory;
        PlayerInventory.OnPlayerBackPackDisplayRequested += DisplayPlayerBackPack;
    }

    private void OnDisable()
    {
        ShopKeeper.OnShopWindowRequested -= DisplayShowWindow;

        InventoryHolder.OnDynamicInventoryDisplayRequested += DisplayWorldSpaceInventory;
        PlayerInventory.OnPlayerBackPackDisplayRequested += DisplayPlayerBackPack;
    }

    private void Update()
    {
        TryScrollHotBar(PlayerInputManager.Instance.MouseScrollWheelDirection);
    }



    public void DisplayWorldSpaceInventory(InventorySystem invToDisplay, int offset)
    {
        InventoryUIController.Instance.DisplayInventory(invToDisplay, offset);
    }


    private void DisplayPlayerBackPack(InventorySystem inventory, int offset)
    {

        InventoryUIController.Instance.DisplayBackPack(inventory, offset);
    }


    private void DisplayShowWindow(ShopSystem shopSystem, PlayerInventory playerInventory)
    {
        if(_shopKeeperDisplayUI.IsShopScreenOpen) return;
        PlayerInventory.Instance.RequestPlayerInventory();
        _shopKeeperDisplayUI.HandleShopKeeperDisplayOpening(shopSystem, playerInventory);
    }

    public bool IsBuildingScreenOpen
    {
        get
        {
            return InventoryUIController.Instance.InventoryPanelIsOpen;
        }
    }

    public void TryScrollHotBar(float amount)
    {
        //if(amount == 0) return;
        if (CanScrollMouseWheel())
        {
            HotBarDisplayScript.Instance.ChangeIndex((int) amount);
        }
    }

    public bool CanScrollMouseWheel()
    {
        if (_PauseMenuManager.GameIsPaused) return false;
        if (_PauseMenuManager.OptionsMenuIsOpen) return false;
        if(_shopKeeperDisplayUI.IsShopScreenOpen) return false;
        return true;
    }

    public bool AllRelevantUIScreensClosedOrHandled()
    {
        if (InventoryUIController.Instance.InventoryPanelIsOpen)
        {
            Debug.Log("closing inventoryPanel");
            InventoryUIController.Instance.CloseInventoryScreen();
            return false;
        }
        if (InventoryUIController.Instance.PlayerBackPackIsOpen)
        {
            Debug.Log("closing backpack");
            InventoryUIController.Instance.ClosePlayerBackBack();
            return false;
        }
        if(ShopKeeperDisplayUI.Instance.IsShopScreenOpen)
        {
            Debug.Log("closing shopkeeperwindow");
            ShopKeeperDisplayUI.Instance.HandleShopKeeperDisplayClosing();
            return false;
        }
        if (PauseMenuManager.Instance.SavePopupIsOpen)
        {
            Debug.Log("closing savePopup");
            PauseMenuManager.Instance.HandleClosingSavePopUp();
            return false;
        }
        if (PauseMenuManager.Instance.LoadMenuIsOpen)
        {
            Debug.Log("closing LoadMenu");
            PauseMenuManager.Instance.HandleClosingLoadMenu();
            return false;
        }
        if (PauseMenuManager.Instance.OptionsMenuIsOpen)
        {
            Debug.Log("closing ClosingOptionsMneu");
            PauseMenuManager.Instance.HandleClosingOptionsMenu();
            return false;
        }

        return true;
    }

    public bool TryChangePlayerInventoryPanel()
    {
        if (InventoryUIController.Instance.PlayerBackPackIsOpen)
        {
            InventoryUIController.Instance.ClosePlayerBackBack();
            return false;
        }
        if (!InventoryUIController.Instance.PlayerBackPackIsOpen)
        {
            PlayerInventory.Instance?.RequestPlayerInventory();
            return true;
        }

        return true;
    }

}

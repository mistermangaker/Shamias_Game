using GameSystems.Inventory;
using GameSystems.ShopSystem;

using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public bool AnyInteractionBlockingWindowsOpen
    {
        get
        {
            return InventoryUIController.Instance.InventoryPanelIsOpen || ShopKeeperDisplayUI.Instance.IsShopScreenOpen || CraftingBenchUI.Instance.CraftingScreenIsOpen;
        }
    }


    private EventBinding<OnWorkbenchScreenRequested> onCraftingScreenRequested;
    private EventBinding<OnDynamicInventoryRequested> onDynamicInventoryRequested;
    private EventBinding<OnShopScreenRequested> onShopScreenRequested;
  

    private void Awake()
    {
        Instance = this;
    }


    private void OnEnable()
    {
        onShopScreenRequested = new EventBinding<OnShopScreenRequested>(DisplayShowWindow);
        EventBus<OnShopScreenRequested>.Register(onShopScreenRequested);
            
        onDynamicInventoryRequested = new EventBinding<OnDynamicInventoryRequested>(HandleDynamicInventoryRequest);
        EventBus<OnDynamicInventoryRequested>.Register(onDynamicInventoryRequested);

        onCraftingScreenRequested = new EventBinding<OnWorkbenchScreenRequested>(PopulateCraftingScreen);
        EventBus<OnWorkbenchScreenRequested>.Register(onCraftingScreenRequested);
    }

    private void OnDisable()
    {
        EventBus<OnShopScreenRequested>.Deregister(onShopScreenRequested);

        EventBus<OnWorkbenchScreenRequested>.Deregister(onCraftingScreenRequested);

        EventBus<OnDynamicInventoryRequested>.Deregister(onDynamicInventoryRequested);
    }

    private void Update()
    {
        TryScrollHotBar(PlayerInputManager.Instance.MouseScrollWheelDirection);
    }

   
    private void HandleDynamicInventoryRequest(OnDynamicInventoryRequested arg)
    {
        InventoryUIController.Instance.DisplayInventory(arg.inventorySystem, arg.offset);
    }
    private void PopulateCraftingScreen(OnWorkbenchScreenRequested arg)
    {
        if (CraftingBenchUI.Instance.CraftingScreenIsOpen)
        {
            InventoryUIController.Instance.ClosePlayerBackBack();
            CraftingBenchUI.Instance.HandleIsOpen();
            return;
        }
        CraftingBenchUI.Instance.PopulateCraftingScreen(arg);
        DisplayPlayerBackPack();
    }

    public void DisplayWorldSpaceInventory(InventorySystem invToDisplay, int offset)
    {
        InventoryUIController.Instance.DisplayInventory(invToDisplay, offset);
    }


    private void DisplayPlayerBackPack()
    {

        InventoryUIController.Instance.DisplayBackPack(PlayerInventory.Instance.InventorySystem, PlayerInventory.Instance.Offset);
    }


    private void DisplayShowWindow(OnShopScreenRequested shopSystem)
    {
        if (ShopKeeperDisplayUI.Instance.IsShopScreenOpen) 
        { 
            InventoryUIController.Instance.ClosePlayerBackBack(); 
            return; 
        }

        DisplayPlayerBackPack();
        ShopKeeperDisplayUI.Instance.HandleShopKeeperDisplayOpening(shopSystem.Shop, PlayerInventory.Instance);
    }

    

    public void TryScrollHotBar(float amount)
    {
        if (CanScrollMouseWheel())
        {
            HotBarDisplayScript.Instance.ChangeIndex((int) amount);
        }
    }

    public bool CanScrollMouseWheel()
    {
        if (PauseMenuManager.Instance.GameIsPaused) return false;
        if (PauseMenuManager.Instance.OptionsMenuIsOpen) return false;
        if (ShopKeeperDisplayUI.Instance.IsShopScreenOpen) return false;
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
        if (CraftingBenchUI.Instance.CraftingScreenIsOpen)
        {
            CraftingBenchUI.Instance.HandleIsOpen();
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
            DisplayPlayerBackPack();
            //PlayerInventory.Instance?.RequestPlayerInventory();
            return true;
        }

        return true;
    }

}

using GameSystems.BuildingSystem;
using GameSystems.Inventory;
using UnityEngine;

public class HotBarDisplayScript : StaticInventoryDisplay
{

    public static HotBarDisplayScript Instance {get; private set;}

    //private PlayerController playerController;
    private int _maxIndexSize;
    private int _currentIndex;

    private GameItem _currentlyHeldgameItem => slots[_currentIndex].SlotItem;
    public GameItem GameItem => _currentlyHeldgameItem;
    //private void Update()
    //{
    //    ChangeIndex((int)PlayerInputManager.Instance.MouseScrollWheelDirection);
    //}

    private void Awake()
    {
        Instance = this;
        
        _currentIndex = 0;
        _maxIndexSize = slots.Length -1;
        //PlayerController.OnItemUsed += UpdateSlot;
    }
    private void OnDisable()
    {
      //  PlayerController.OnItemUsed -= UpdateSlot;
    }


    protected override void Start()
    {
        base.Start();
       // playerController = PlayerController.Instance;
        slots[_currentIndex].ToggleHightLight();
    }


    // TODO: refactor this code so its only for display and doesnt touch the construction layer or player controller
    public void ChangeIndex(int change)
    {
        slots[_currentIndex].ToggleHightLight();
        _currentIndex += change;
        if (_currentIndex > _maxIndexSize)
        {
            _currentIndex = 0;
        }
        if (_currentIndex < 0)
        {
            _currentIndex = _maxIndexSize;
        }
        slots[_currentIndex].ToggleHightLight();
        EventBus<OnPlayerEquipedItemChanged>.Raise(new OnPlayerEquipedItemChanged
        {
            Slot = slots[_currentIndex].AssignedInventorySlot,
            Item = _currentlyHeldgameItem,
        }) ;
        //PlayerController.Instance.SetCurrentlyHeldItem(slots[_currentIndex].AssignedInventorySlot);
       // ConstructionLayerManager.Instance.SetActiveBuildable(_currentlyHeldgameItem.ItemData?.Buildable);
    }

}

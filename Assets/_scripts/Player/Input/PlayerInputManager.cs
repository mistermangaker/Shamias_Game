using GameSystems.Inventory;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public enum MouseButton
{
    Left,
    Right
}

public class PlayerInputManager : MonoBehaviour
{

    private static PlayerInputManager _instance;
    public static PlayerInputManager Instance
    {
        get
        {
            
            return _instance;
        }
        private set => _instance = value;
    }

    public Vector2 MousePosition { get; private set; }
    public Vector2 MouseInWorldPoint => Camera.main.ScreenToWorldPoint(MousePosition);

    public LayerMask groundlayer;

    private Vector3 m_lastpos;
    public Vector3 MouseToGroundPlane
    {
        get
        {
            Vector3 pos = MousePosition;
            pos.z = Camera.main.nearClipPlane;
            Ray ray = Camera.main.ScreenPointToRay(pos);
            RaycastHit hit;
            Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);
            if(Physics.Raycast(ray, out hit, 100, groundlayer))
            {
                m_lastpos = hit.point;
            }
            return m_lastpos;

        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(MouseToGroundPlane, 0.5f);
    }

    private bool _isLeftMouseButtonPressed;
    private bool _isRightMouseButtonPressed;

    private bool _isAltActionButtonDown;
    public bool IsAltActionButtonDown => _isAltActionButtonDown;
   

    private PlayerInput _InputActions;


    private UIManager _UIManager;

    public static UnityAction<Vector2> OnPlayerMovement;
    public static UnityAction<Vector2> OnPlayerLookDirection;

    public static UnityAction OnPlayerInventoryOpen;

    public static UnityAction OnInteractionPerformed;
    public static UnityAction OnAlternateInteractionPerformed;

    public static UnityAction OnMouseButtonPerformed;


    public static UnityAction OnPause;
    public static UnityAction<bool> OnPauseRequested;
    public static UnityAction OnResume;

    public float MouseScrollWheelDirection
    {
        get
        {
            if (_InputActions.Player.MouseWheel.ReadValue<float>() > 0.1f) return 1f;
            if (_InputActions.Player.MouseWheel.ReadValue<float>() < -0.1f) return -1f;
            return 0f;
        }
    }

    private void Awake()
    {
        Instance = this;
        _InputActions = new PlayerInput();
        _InputActions.Enable();
        _InputActions.UI.Disable();
        _InputActions.Player.Pause.performed += Pause_performed;
        _InputActions.UI.UnPause.performed += UnPause_performed;

        _InputActions.Player.Move.performed += Move_performed;
        _InputActions.Player.Move.canceled += Move_performed;

        _InputActions.Player.Interact.performed += Interact_performed;
        _InputActions.Player.Interact.started += Interact_performed;

        _InputActions.Player.AltButton.performed += AltButton_performed;
        _InputActions.Player.AltButton.canceled += AltButton_canceled;

        _InputActions.Player.InventoryOpenAction.performed += InventoryOpenAction_performed;

        _InputActions.Player.MousePosition.performed += OnMousePositionPerformed;
        _InputActions.Player.MouseAction.performed += OnMouseActionPerformed;
        _InputActions.Player.MouseAction.canceled += OnMouseActionCancelled;
        _InputActions.Player.AltMouseAction.performed += OnAlternateMouseActionPerformed;
        _InputActions.Player.AltMouseAction.canceled += OnAlternateMouseActionCanceled;


        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void Start()
    {
        _UIManager = UIManager.Instance;
    }
    private void OnDisable()
    {

        _InputActions.Player.Pause.performed -= Pause_performed;
        _InputActions.UI.UnPause.performed -= UnPause_performed;

        _InputActions.Player.Move.performed -= Move_performed;
        _InputActions.Player.Move.canceled -= Move_performed;

        _InputActions.Player.Interact.performed -= Interact_performed;
        _InputActions.Player.Interact.started -= Interact_performed;

        _InputActions.Player.AltButton.performed -= AltButton_performed;
        _InputActions.Player.AltButton.canceled -= AltButton_canceled;

        _InputActions.Player.InventoryOpenAction.performed -= InventoryOpenAction_performed;

        _InputActions.Player.MousePosition.performed -= OnMousePositionPerformed;
        _InputActions.Player.MouseAction.performed -= OnMouseActionPerformed;
        _InputActions.Player.MouseAction.canceled -= OnMouseActionCancelled;
        _InputActions.Player.AltMouseAction.performed -= OnAlternateMouseActionPerformed;
        _InputActions.Player.AltMouseAction.canceled -= OnAlternateMouseActionCanceled;
        _InputActions.Disable();

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Interact_performed(InputAction.CallbackContext obj)
    {
        if(obj.started)
        {
            
            if (!_UIManager.AllRelevantUIScreensClosedOrHandled()) return;
            
            if (IsAltActionButtonDown) OnAlternateInteractionPerformed?.Invoke();
            else OnInteractionPerformed?.Invoke();
        }
       
    }

    
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SwitchToGamePlayMode();
    }

    private void SwitchToGamePlayMode()
    {
        _InputActions.Player.Enable();
        _InputActions.UI.Disable();
    }

    private void SwitchToMenusMode()
    {
        _InputActions.Player.Disable();
        _InputActions.UI.Enable();
    }

    

    private void InventoryOpenAction_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        _UIManager.TryChangePlayerInventoryPanel();
       
    }

    private void Move_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Vector2 dir = obj.ReadValue<Vector2>();
        OnPlayerMovement?.Invoke(dir);
        if (obj.performed)
        {
            OnPlayerLookDirection?.Invoke(dir);
        }
    }

    private void UnPause_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        _InputActions.Player.Enable();
        _InputActions.UI.Disable();
        OnResume?.Invoke();
    }

    

    private void Pause_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (!_UIManager.AllRelevantUIScreensClosedOrHandled()) return;
        SwitchToMenusMode();
        OnPause?.Invoke();
    }

    public bool IsMouseButtonPressed(MouseButton button)
    {
        return button == MouseButton.Left ? _isLeftMouseButtonPressed : _isRightMouseButtonPressed;
    }

    private void OnMouseActionPerformed(InputAction.CallbackContext context)
    {
        _isLeftMouseButtonPressed = true;
        if(_UIManager.AnyInteractionBlockingWindowsOpen) return;
       
        if (context.performed)
        {
            OnMouseButtonPerformed?.Invoke();
        }
        
    }
    private void OnMouseActionCancelled(InputAction.CallbackContext context)
    {
        _isLeftMouseButtonPressed = false;
       
    }
    private void OnAlternateMouseActionPerformed(InputAction.CallbackContext context)
    {
        _isRightMouseButtonPressed = true;
       
    }
    private void OnAlternateMouseActionCanceled(InputAction.CallbackContext context)
    {
        _isRightMouseButtonPressed = false;
    }

    private void OnMousePositionPerformed(InputAction.CallbackContext context)
    {
        MousePosition = context.ReadValue<Vector2>();
    }

    private void AltButton_canceled(InputAction.CallbackContext obj)
    {
        _isAltActionButtonDown = false;
    }

    private void AltButton_performed(InputAction.CallbackContext obj)
    {
        _isAltActionButtonDown = true;
    }

}

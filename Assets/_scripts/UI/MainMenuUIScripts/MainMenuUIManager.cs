using UnityEngine;
using UnityEngine.UI;

public class MainMenuUIManager : MonoBehaviour
{
    public static MainMenuUIManager Instance;
    [SerializeField] private Button newGameButton, loadGameButton, optionsButton, closeGameButton;
    [SerializeField] private OptionsMenuUI uiOptions;
    [SerializeField] private MainMenuLoadScreenUI uiLoad;
    [SerializeField] private NewGameMenu uiNewGame;
    private PlayerInput playerInput;

    private void Awake()
    {
        Instance = this;
        playerInput = new PlayerInput();
        playerInput.UI.Enable();
        playerInput.UI.UnPause.performed += UnPause_performed;
        uiLoad.HandleClosingLoadScreenUI();
        uiNewGame.HandleClosingNewGameMenu();
        uiOptions.HandleHidingOptionMenu();
    }
    private void UnPause_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        CloseMenus();
    }

    private void OnDestroy()
    {
        playerInput.UI.UnPause.performed -= UnPause_performed;
    }
    private void Start()
    {
        newGameButton.onClick.AddListener(() =>
        {
            uiNewGame.ShowNewGameMenu();
        });
        loadGameButton.onClick.AddListener(() =>
        {
            uiLoad.ShowLoadScreenUI();
        });
        optionsButton.onClick.AddListener(() =>
        {
            uiOptions.ShowOptionsMenu();
        });
        closeGameButton.onClick.AddListener(() =>
        {
            Debug.Log("click! closing game!");
        });

    }

    private void CloseMenus()
    {
        if (uiLoad.IsOpen)
        {
            Debug.Log("closeing load menu");
            uiLoad.HandleClosingLoadScreenUI();
            return;
        }
        if (uiNewGame.IsOpen)
        {
            Debug.Log("closeing new game");
            uiNewGame.HandleClosingNewGameMenu();
            return;
        }
        if(uiOptions.IsOpen)
        {
            Debug.Log("closeing options");
            uiOptions.HandleHidingOptionMenu();
            return ;
        }
       
    }

}

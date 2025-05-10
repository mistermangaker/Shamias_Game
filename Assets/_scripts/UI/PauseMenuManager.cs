using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    public static PauseMenuManager Instance;
    [SerializeField] private Button resumeButton, loadButton, saveButton, optionsButton, quitToMenuButton;
    [SerializeField] private GameObject pauseMenu, saveLoadMenu, optionsMenu, saveMenu;
    [SerializeField] private SaveAndLoadScreenUI SaveAndLoadScreenUI;
    [SerializeField] private SavePopUpSingleUI savePopup;

    public bool hasSaved;

    public bool GameIsPaused
    {
        get
        {
            return Time.timeScale == 0;
        }
    }
    public bool AnyRelevantMenuWindowsAreOpen
    {
        get
        {
            return SavePopupIsOpen || SavePopupIsOpen || OptionsMenuIsOpen;
        }
    }
    public bool SavePopupIsOpen
    {
        get { return savePopup.gameObject.activeSelf; }
    }
    public bool LoadMenuIsOpen
    {
        get
        {
            return SaveAndLoadScreenUI.gameObject.activeSelf;
        }
    }

    public bool OptionsMenuIsOpen
    {
        get
        {
            return optionsMenu.gameObject.activeSelf;
        }
    }

    public void HandleQuitingToMainMenu()
    {

    }
    public void HandleClosingOptionsMenu()
    {
        HideOptionsMenu();
    }
    public void HandleClosingLoadMenu()
    {
        HideSaveLoadMenu();
    }
    public void HandleClosingSavePopUp()
    {
        HideSaveMenu();
    }

    private void Awake()
    {
        Instance = this;
        if (Time.timeScale == 0) ResumeGame();
        PlayerInputManager.OnPause += PauseGame;
        PlayerInputManager.OnResume += ResumeGame;
        
        pauseMenu.SetActive(false);
        HideSaveLoadMenu();
        HideSaveMenu();
        HideOptionsMenu();
    }
    private void Start()
    {
        resumeButton.onClick.AddListener(() => ResumeGame());
        loadButton.onClick.AddListener(() => ShowSaveLoadMenu());
        saveButton.onClick.AddListener(() => {
          
            ShowSaveMenu();
            });
        optionsButton.onClick.AddListener(() => ShowOptionsMenu());
        quitToMenuButton.onClick.AddListener(() => {
            ResumeGame();
            // load scene for the main menu
        });
    }
    private void OnEnable()
    {
        hasSaved = false;
    }

    private void OnDisable()
    {
        PlayerInputManager.OnPause -= PauseGame;
        PlayerInputManager.OnResume -= ResumeGame;
       
    }


    private void PauseGame()
    {
        Time.timeScale = 0;
        ShowPauseMenu();
    }

    private void ResumeGame()
    {
        HidePauseMenu();
        Time.timeScale = 1;
    }

    private void ShowPauseMenu()
    {
        pauseMenu.SetActive(true);
    }
    private void HidePauseMenu()
    {
        pauseMenu.SetActive(false);
        HideSaveLoadMenu();
        HideOptionsMenu();
    }
    private void ShowSaveLoadMenu()
    {
        saveLoadMenu.SetActive(true);
        SaveAndLoadScreenUI.ShowSaves();
    }

    public void HideSaveMenu()
    {
        saveMenu.SetActive(false);
    }
    public void ShowSaveMenu()
    {
        saveMenu.SetActive(true);
        savePopup.SetText();
    }
    private void HideSaveLoadMenu()
    {
        saveLoadMenu.SetActive(false);
    }

    private void ShowOptionsMenu()
    {
        optionsMenu.SetActive(true);
    }
    private void HideOptionsMenu()
    {
        optionsMenu.SetActive(false);
    }


}

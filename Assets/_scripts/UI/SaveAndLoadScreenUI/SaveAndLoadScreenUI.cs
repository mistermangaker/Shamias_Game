using UnityEngine;
using GameSystems.SaveLoad;
using System.Collections.Generic;

public class SaveAndLoadScreenUI : MonoBehaviour
{
    public static SaveAndLoadScreenUI Instance { get; private set; }
    [SerializeField] private GameObject SaveSlotsUiPrefab;
    [SerializeField] private GameObject SaveSlotContainer;
    [SerializeField] private GameObject savePopup;
    private List<GameSaveUIInformation> saveInfo = new List<GameSaveUIInformation>();
    private GameSaveUIInformation currentgameToLoad;
   // public bool hasGameBeensavedYet = false;
    private void Awake()
    {
        Instance = this;
        //gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        savePopup.SetActive(false);
    }
    
    public void TryLoadSave(GameSaveUIInformation info)
    {
        currentgameToLoad = info;
        if (PauseMenuManager.Instance.hasSaved)
        {
            LoadGame();
        }
        else
        {
            savePopup.SetActive(true);
        }
    }

    public void LoadGame()
    {
        SaveLoadSystem.Instance.LoadGame(currentgameToLoad.Name);
    }

    public void ShowSaves()
    {
        saveInfo.Clear();
        saveInfo = SaveLoadSystem.Instance.GetSaveData();
        Debug.Log(saveInfo.Count);
        
        foreach (Transform child in SaveSlotContainer.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (GameSaveUIInformation info in saveInfo)
        {
            SaveSlotSingleUI ui = Instantiate(SaveSlotsUiPrefab, SaveSlotContainer.transform).GetComponent<SaveSlotSingleUI>();
            if (ui != null)
            {
                ui.SetupUI(info);
            }
        }
    }

    public void HidePopup()
    {
        savePopup.SetActive(false);
    }
    public void ShowPopup()
    {
        savePopup.SetActive(true);
    }
}

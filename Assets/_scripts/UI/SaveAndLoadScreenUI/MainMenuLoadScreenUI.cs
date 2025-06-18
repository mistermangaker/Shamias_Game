using GameSystems.SaveLoad;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuLoadScreenUI : MonoBehaviour
{
    public static MainMenuLoadScreenUI Instance { get; private set; }
    [SerializeField] private GameObject SaveSlotsUiPrefab;
    [SerializeField] private GameObject SaveSlotContainer;
    [SerializeField] private GameObject container;

    private List<GameSaveUIInformation> saveInfo = new List<GameSaveUIInformation>();
    private GameSaveUIInformation currentgameToLoad;

    public bool IsOpen
    {
        get
        {
            return container.activeSelf;
        }
    }
    private void Awake()
    {
        Instance = this;
        HandleClosingLoadScreenUI();
    }

    public void ShowLoadScreenUI()
    {
        container.SetActive(true);
        ShowSaves();
    }
    public void HandleClosingLoadScreenUI()
    {
        container.SetActive(false);
    }


    public void TryLoadSave(GameSaveUIInformation info)
    {
        currentgameToLoad = info;
        LoadGame();
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
            MainMenuSaveslotSingleUI ui = Instantiate(SaveSlotsUiPrefab, SaveSlotContainer.transform).GetComponent<MainMenuSaveslotSingleUI>();
            if (ui != null)
            {
                ui.SetupUI(info);
            }
        }
    }
}

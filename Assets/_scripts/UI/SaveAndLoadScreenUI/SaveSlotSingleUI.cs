using GameSystems.SaveLoad;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlotSingleUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI saveName;
    [SerializeField] private Button loadButton;
    
    private GameSaveUIInformation currentgameInfo;

    public void SetupUI(GameSaveUIInformation gameSaveUI)
    {
        currentgameInfo = gameSaveUI;
        saveName.text = currentgameInfo.Name;
    }

    private void Start()
    {
        loadButton.onClick.AddListener(() =>
        {
            SaveAndLoadScreenUI.Instance.TryLoadSave(currentgameInfo);
        });
        
    }
}

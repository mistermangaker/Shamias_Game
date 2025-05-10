using GameSystems.SaveLoad;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SavePopUpSingleUI : MonoBehaviour
{
    [SerializeField] private Button SaveButton, BackButton;
    [SerializeField] private TMP_InputField inputField;

    
    public void SetText()
    {
        inputField.text = SaveLoadSystem.Instance.GetCurrentSaveName();
    }

    private void Start()
    {
        SaveButton.onClick.AddListener(() => { SaveLoadSystem.Instance.MakeNewGameSave(inputField.text); });
        BackButton.onClick.AddListener(() => { PauseMenuManager.Instance.HideSaveMenu(); PauseMenuManager.Instance.hasSaved = true; });
    }

}

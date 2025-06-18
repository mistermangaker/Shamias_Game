using GameSystems.SaveLoad;
using UnityEngine;
using UnityEngine.UI;

public class NewGameMenu : MonoBehaviour
{
    [SerializeField] private Button newGameButton;
    [SerializeField] private GameObject Container;

    public bool IsOpen
    {
        get
        {
            return Container.activeSelf;
        }
    }

    private void Awake()
    {
        newGameButton.onClick.AddListener(() => { 
        SaveLoadSystem.Instance.NewGame();
        });

        HandleClosingNewGameMenu();
    }

    public void ShowNewGameMenu()
    {
        Container.SetActive(true);
    }

    public void HandleClosingNewGameMenu()
    {
        Container.SetActive(false);
    }
}

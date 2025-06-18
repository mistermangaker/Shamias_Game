using UnityEngine;

public class OptionsMenuUI : MonoBehaviour
{
    public static OptionsMenuUI Instance { get; private set; }
    [SerializeField] private GameObject container;

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
        container.SetActive(false);
    }

    public void HandleHidingOptionMenu()
    {
        container.SetActive(false);
    }
    public void ShowOptionsMenu()
    {
        container.SetActive(true);
    }
}

using UnityEngine;
using UnityEngine.UI;

public class LoadPopUpSingleGUI : MonoBehaviour
{
    [SerializeField] private Button loadAnywayButton, backButton;

    private void Start()
    {
        loadAnywayButton.onClick.AddListener(() => { SaveAndLoadScreenUI.Instance.LoadGame(); });
        backButton.onClick.AddListener(() => { SaveAndLoadScreenUI.Instance.HidePopup(); });
    }
}

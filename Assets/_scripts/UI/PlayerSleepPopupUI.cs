using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSleepPopupUI : MonoBehaviour
{
    [SerializeField] private Button confirmButton, closeButton;
    [SerializeField] private GameObject window;
    private float timer = 1f;
    public bool isOpen
    {
        get
        {
            return window.activeSelf;
        }
    }

    private EventBinding<OnPlayerSleepRequested> onPlayerSleepRequested;
    private void Awake()
    {
        onPlayerSleepRequested = new EventBinding<OnPlayerSleepRequested>(HandleSleepLogic);
        EventBus<OnPlayerSleepRequested>.Register(onPlayerSleepRequested);

        confirmButton.onClick.AddListener(() =>
        {
            timer = 2f;
            HandleClosingWindow();
            TimeManager.Instance.TransitionToNextMorning();
        });

        closeButton.onClick.AddListener(() => 
        {
            HandleClosingWindow();
        });
        HandleClosingWindow();
    }
    private void OnDestroy()
    {
        EventBus<OnPlayerSleepRequested>.Deregister(onPlayerSleepRequested);
    }

    private void HandleSleepLogic()
    {
        if(timer>=0)return;
        if(TimeManager.Instance.CurrentGameTime.Hour >22 || TimeManager.Instance.CurrentGameTime.Hour < 2)
        {
            
            ScreenFadeManager.Instance.SetFadeOut(2f);
            TimeManager.Instance.TransitionToNextMorning();
            
        }
        else
        {
            OpenWindow();
        }
    }

    public void OpenWindow()
    {
        window.SetActive(true);
    }

    public void HandleClosingWindow()
    {
        window?.SetActive(false);
    }

    private void Update()
    {
        if (timer <= 0) return;
        else timer -= Time.deltaTime;
    }

}

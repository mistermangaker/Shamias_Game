using UnityEngine;
using UnityEngine.UI;

public class ScreenFadeManager : MonoBehaviour
{

    public static ScreenFadeManager Instance;
    [SerializeField] private Image backgroundFade;

    private float fadeTime;
    private float fadeDuration;


    public void SetFadeOut(float time)
    {
       // Debug.Log("yep");
        Show();
        fadeTime = time;
        fadeDuration = fadeTime;
    }
    private void Awake()
    {
        Instance= this;
        Hide();
    }

    private void Show()
    {
        backgroundFade.gameObject.SetActive(true);
    }
    private void Hide()
    {
       // Debug.Log("hiding");
        backgroundFade.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (fadeTime <= 0) return;

        fadeTime -= Time.deltaTime;
        float alpha = fadeTime / fadeDuration;
        Color temp = backgroundFade.color;
        backgroundFade.color = new Color(temp.r, temp.g, temp.b, alpha);
        if(fadeDuration <= 0)
        {
            Hide();
        }
    }
}

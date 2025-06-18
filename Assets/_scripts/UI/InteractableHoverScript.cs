using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public struct OnToolTipRequested : IEvent
{
    public string toolTipHeader;
    public string toolTipBody;

    public InteractionIntent intent;
    public List<InteractionIntent> possibleInteractions;
}

public struct OnToolTipUnrequested : IEvent
{

}


public class InteractableHoverScript : MonoBehaviour
{
    [Serializable]
    private struct NamedSprite
    {
        public string name;
        public Sprite sprite;
        
    }
    [SerializeField] private List<NamedSprite> sprites;

    [SerializeField] public TextMeshProUGUI thingText, actionText;
    [SerializeField] private Sprite actionSprite;

    private Canvas canvas;
    [SerializeField] private Vector3 offset = new Vector3(150, -150, 0);

    private RectTransform rect;
    private RectTransform canvasTransform;

    private EventBinding<OnToolTipRequested> onToolTipRequested;
    private EventBinding<OnToolTipUnrequested> onToolTipUnRequested;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        canvasTransform = canvas.GetComponent<RectTransform>();
        gameObject.SetActive(false);
        rect = GetComponent<RectTransform>();

        onToolTipRequested = new EventBinding<OnToolTipRequested>(HandleToolTipRequest);
        EventBus<OnToolTipRequested>.Register(onToolTipRequested);

        onToolTipUnRequested = new EventBinding<OnToolTipUnrequested>(Clear);
        EventBus<OnToolTipUnrequested>.Register(onToolTipUnRequested);
    }
    private void OnDestroy()
    {
        EventBus<OnToolTipRequested>.Deregister(onToolTipRequested);
        EventBus<OnToolTipUnrequested>.Deregister(onToolTipUnRequested);
    }

    private void HandleToolTipRequest(OnToolTipRequested e)
    {
        SetText(e.toolTipHeader, e.toolTipBody);
        SetSprite(e.intent.ToString());
    }

    private void SetSprite(string spriteName)
    {
        foreach (var sprite in sprites)
        {
            if(sprite.name == spriteName)
            {
                SetSprite(sprite.sprite);
                return;
            }
        }
    }
    private void SetSprite(Sprite sprite)
    {
        actionSprite = sprite;
    }
    private void ClearSprite()
    {
        actionSprite = null;
    }

    private void Clear()
    {
        ClearSprite();
        ClearText();
    }
    public void SetText(string interactableName, string action)
    {
        gameObject.SetActive (true);
        thingText.text = interactableName;
        actionText.text = action;
    }

    public void ClearText()
    {
        gameObject.SetActive(false);
        thingText.text = "";
        actionText.text = "";
    }
    private void LateUpdate()
    {
        transform.position = Input.mousePosition;

        var sizeDelta = canvasTransform.sizeDelta - rect.sizeDelta;
        var panelPivot = rect.pivot;
        var position = rect.anchoredPosition;
        position.x = Mathf.Clamp((position.x + offset.x), -sizeDelta.x * panelPivot.x, sizeDelta.x * (1 - panelPivot.x));
        position.y = Mathf.Clamp((position.y + offset.y), -sizeDelta.y * panelPivot.y, sizeDelta.y * (1 - panelPivot.y));
        rect.anchoredPosition = position / canvas.scaleFactor;

    }
}

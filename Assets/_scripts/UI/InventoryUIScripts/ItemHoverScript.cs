using GameSystems.Inventory;
using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class ItemHoverScript : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI itemText, DescriptionText;
    private Canvas canvas;
    [SerializeField] private Vector3 offset = new Vector3(150, -150, 0);

    private RectTransform rect;
    private RectTransform canvasTransform;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        canvasTransform = canvas.GetComponent<RectTransform>();
        gameObject.SetActive(false);
        rect = GetComponent<RectTransform>();
    }
    public void SetItem(GameItem item)
    {
        gameObject.SetActive(true);
        itemText.text = item.ItemData.DisplayName;
        DescriptionText.text = item.ItemData.ItemDescription;
    }
    public void ClearItem()
    {
        gameObject.SetActive(false);
        itemText.text = string.Empty;
        DescriptionText.text = string.Empty;
    }

    private void Update()
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

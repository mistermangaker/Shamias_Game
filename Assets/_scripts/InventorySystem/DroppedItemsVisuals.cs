using UnityEngine;

public class DroppedItemsVisuals : MonoBehaviour
{
    [SerializeField] private DroppedGameItem item;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    void Awake()
    {
        item.OnInitialized += UpdateVisual;
        item.OnPickUp += TriggerPickUp;
    }

    private void TriggerPickUp()
    {

    }

    private void UpdateVisual()
    {
        spriteRenderer.sprite = item.ItemData.Icon;
    }

    private void OnDestroy()
    {
        item.OnInitialized -= UpdateVisual;
        item.OnPickUp -= TriggerPickUp;
    }
}

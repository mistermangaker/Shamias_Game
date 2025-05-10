using TMPro;
using UnityEngine;

public class ShoppingCartItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _itemText;
    public void SetItemText(string text)
    {
        _itemText.text = text;
    }
}

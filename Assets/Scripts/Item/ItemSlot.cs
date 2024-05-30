using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public ItemData item;

    public Image backGroundImage;
    public Image Icon;
    public TextMeshProUGUI quantityText;

    public int index;
    public bool equipped;
    public int quantity;

    Color originSlotBGColor = new Color(1,1,1,0.5f);
    Color highlightedColor = new Color(1, 1, 1, 1f);

    public void Set()
    {
        Icon.gameObject.SetActive(true);
        Icon.sprite = item.icon;
        quantityText.text = quantity > 1 ? quantity.ToString() : string.Empty;
    }

    public void Clear()
    {
        item = null;
        Icon.gameObject.SetActive(false);
        quantityText.text = string.Empty;
    }

    public void HighLightSlot(bool isSelected)
    {
        backGroundImage.color = isSelected ? highlightedColor : originSlotBGColor;
    }
}

using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class InventorySlot
{
    public InventoryItem item;
    public int quantity;
    public GameObject slotObject;
    public Text stackText;
    public Image itemIcon;

    public void SetItem(InventoryItem newItem, int newQuantity)
    {
        item = newItem;
        quantity = newQuantity;
        if (item != null)
        {
            itemIcon.sprite = item.itemIcon;
            itemIcon.enabled = true;
            itemIcon.color = new Color(itemIcon.color.r, itemIcon.color.g, itemIcon.color.b, 1); // Make the icon fully visible
            stackText.text = quantity.ToString();
        }
        else
        {
            itemIcon.enabled = false;
            stackText.text = "";
        }

        SetTransformProperties();
    }

    public void SetTransformProperties()
    {
        // Set DraggableItem RectTransform properties
        RectTransform draggableItemRect = slotObject.transform.Find("DraggableItem").GetComponent<RectTransform>();
        draggableItemRect.anchorMin = Vector2.zero;
        draggableItemRect.anchorMax = Vector2.one;
        draggableItemRect.offsetMin = Vector2.zero;
        draggableItemRect.offsetMax = Vector2.zero;

        // Set ItemIcon RectTransform properties
        RectTransform itemIconRect = itemIcon.GetComponent<RectTransform>();
        itemIconRect.anchorMin = new Vector2(0.5f, 0.5f);
        itemIconRect.anchorMax = new Vector2(0.5f, 0.5f);
        itemIconRect.pivot = new Vector2(0.5f, 0.5f);
        itemIconRect.sizeDelta = new Vector2(35, 35); // 35% of the slot size
        itemIconRect.anchoredPosition = Vector2.zero;

        // Set StackText RectTransform properties
        RectTransform stackTextRect = stackText.GetComponent<RectTransform>();
        stackTextRect.anchorMin = new Vector2(1, 0);
        stackTextRect.anchorMax = new Vector2(1, 0);
        stackTextRect.pivot = new Vector2(1, 0);
        stackTextRect.sizeDelta = new Vector2(20, 40);
        stackTextRect.anchoredPosition = new Vector2(-5, 5); // Adjust as needed
    }
}

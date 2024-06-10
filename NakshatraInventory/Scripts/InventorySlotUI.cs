using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    public InventorySlot slot;

    private void Awake()
    {
        slot = new InventorySlot
        {
            slotObject = gameObject,
            stackText = GetComponentInChildren<Text>(),
            itemIcon = transform.Find("DraggableItem/ItemIcon").GetComponent<Image>()
        };
    }
}

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
            stackText = transform.Find("DraggableItem/StackText")?.GetComponent<Text>(),
            itemIcon = transform.Find("DraggableItem/ItemIcon")?.GetComponent<Image>()
        };

        if (slot.stackText == null)
        {
            Debug.LogError($"StackText not found in DraggableItem for slot: {gameObject.name}");
        }

        if (slot.itemIcon == null)
        {
            Debug.LogError($"ItemIcon not found in DraggableItem for slot: {gameObject.name}");
        }
    }
}

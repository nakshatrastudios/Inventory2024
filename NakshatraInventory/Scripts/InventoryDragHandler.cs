using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public InventorySlot slot;
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (slot.item != null)
        {
            originalPosition = rectTransform.anchoredPosition;
            canvasGroup.alpha = 0.6f; // Make the item semi-transparent while dragging
            canvasGroup.blocksRaycasts = false; // Allow the item to be dragged
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (slot.item != null)
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (slot.item != null)
        {
            // Reset the alpha value for the original slot
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true; // Enable raycast blocking again

            // Check if the pointer is over a valid drop target
            InventorySlot targetSlot = null;
            if (eventData.pointerEnter != null)
            {
                var targetSlotObject = eventData.pointerEnter.GetComponentInParent<InventorySlotUI>();
                if (targetSlotObject != null)
                {
                    targetSlot = targetSlotObject.slot;
                }
            }

            // Snap back to the original position if not dropped on a valid slot
            if (targetSlot == null || targetSlot == slot)
            {
                rectTransform.anchoredPosition = originalPosition;
            }
            else
            {
                // Ensure the item snaps to the center of the new slot
                rectTransform.anchoredPosition = Vector2.zero;
            }

            // Reset the alpha for the target slot (if any)
            if (targetSlot != null)
            {
                var targetHandler = targetSlot.slotObject.transform.Find("DraggableItem").GetComponent<CanvasGroup>();
                if (targetHandler != null)
                {
                    targetHandler.alpha = 1f;
                }
            }
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            InventoryDragHandler draggedHandler = eventData.pointerDrag.GetComponent<InventoryDragHandler>();
            if (draggedHandler != null && draggedHandler.slot != null)
            {
                InventorySlot draggedSlot = draggedHandler.slot;
                InventorySlot targetSlot = slot;

                // Swap the items between the slots
                InventoryItem tempItem = draggedSlot.item;
                int tempQuantity = draggedSlot.quantity;

                draggedSlot.SetItem(targetSlot.item, targetSlot.quantity);
                targetSlot.SetItem(tempItem, tempQuantity);

                // Reset position of dragged item
                draggedHandler.rectTransform.anchoredPosition = Vector2.zero;
                rectTransform.anchoredPosition = Vector2.zero; // Ensure the dropped item is centered in the slot

                // Set transform properties to ensure correct positioning
                draggedSlot.SetTransformProperties();
                targetSlot.SetTransformProperties();

                // Ensure the dragged item is interactable
                draggedHandler.canvasGroup.blocksRaycasts = true;
                canvasGroup.blocksRaycasts = true;

                // Reset the alpha for both the original and the target slots
                draggedHandler.canvasGroup.alpha = 1f;
                canvasGroup.alpha = 1f;
            }
        }
    }
}

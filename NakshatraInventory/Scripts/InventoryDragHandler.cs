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
    private Inventory inventory;
    private bool isChangingPage = false; // Prevent rapid page changes

    // Temporary drag item
    private GameObject dragItem;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Canvas reference is not set. Ensure the Canvas component is in a parent object.");
        }

        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogError("RectTransform reference is not set. Ensure the RectTransform component is attached to the GameObject.");
        }

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            Debug.LogError("CanvasGroup reference is not set. Ensure the CanvasGroup component is attached to the GameObject.");
        }

        // Find the Inventory component in the scene
        inventory = FindObjectOfType<Inventory>();
        if (inventory == null)
        {
            Debug.LogError("Inventory component not found in the scene. Ensure the Inventory component is attached to a GameObject.");
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (slot.item != null)
        {
            originalPosition = rectTransform.anchoredPosition;
            canvasGroup.alpha = 0.6f; // Make the item semi-transparent while dragging
            canvasGroup.blocksRaycasts = false; // Allow the item to be dragged

            // Create a temporary drag item
            dragItem = new GameObject("DragItem");
            dragItem.transform.SetParent(canvas.transform, true);
            dragItem.transform.SetAsLastSibling();

            // Copy the item's appearance
            Image dragImage = dragItem.AddComponent<Image>();
            dragImage.sprite = slot.itemIcon.sprite;
            dragImage.SetNativeSize();

            CanvasGroup tempCanvasGroup = dragItem.AddComponent<CanvasGroup>();
            tempCanvasGroup.blocksRaycasts = false;

            // Set the position
            RectTransform dragRectTransform = dragItem.GetComponent<RectTransform>();
            dragRectTransform.sizeDelta = rectTransform.sizeDelta;
            dragRectTransform.position = Input.mousePosition;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragItem != null)
        {
            RectTransform dragRectTransform = dragItem.GetComponent<RectTransform>();
            dragRectTransform.position = Input.mousePosition;

            // Check if near the "Next" or "Previous" buttons
            if (inventory != null && inventory.nextPageButton != null && inventory.previousPageButton != null)
            {
                RectTransform nextButtonRect = inventory.nextPageButton.GetComponent<RectTransform>();
                RectTransform prevButtonRect = inventory.previousPageButton.GetComponent<RectTransform>();

                if (!isChangingPage && RectTransformUtility.RectangleContainsScreenPoint(nextButtonRect, Input.mousePosition, canvas.worldCamera))
                {
                    // Dragging over the "Next" button
                    isChangingPage = true;
                    inventory.NextPage();
                    Invoke(nameof(ResetPageChange), 0.5f); // Prevent rapid page changes
                }
                else if (!isChangingPage && RectTransformUtility.RectangleContainsScreenPoint(prevButtonRect, Input.mousePosition, canvas.worldCamera))
                {
                    // Dragging over the "Previous" button
                    isChangingPage = true;
                    inventory.PreviousPage();
                    Invoke(nameof(ResetPageChange), 0.5f); // Prevent rapid page changes
                }
            }
        }
    }

    private void ResetPageChange()
    {
        isChangingPage = false;
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragItem != null)
        {
            Destroy(dragItem);
            dragItem = null;
        }

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
                var targetHandler = targetSlot.slotObject.transform.Find("DraggableItem")?.GetComponent<CanvasGroup>();
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

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public int rows = 4;  // Number of rows per page
    public int columns = 5;  // Fixed number of columns
    public int totalSlots = 20;  // Total number of slots
    public GameObject slotPrefab;
    public Transform inventoryGrid;
    public List<InventorySlot> inventorySlots = new List<InventorySlot>();
    private int currentPage = 0;
    private int pages;

    public Vector2 spacing = new Vector2(10, 10);  // Spacing between slots
    public int paddingLeft = 10;
    public int paddingRight = 10;
    public int paddingTop = 10;
    public int paddingBottom = 10;
    private RectOffset padding;

    public int Pages
    {
        get { return pages; }
    }

    void Start()
    {
        // Initialize padding
        padding = new RectOffset(paddingLeft, paddingRight, paddingTop, paddingBottom);

        SetupInventoryUI();
    }

    public void SetupInventoryUI()
    {
        // Clear existing slots
        foreach (Transform child in inventoryGrid)
        {
            Destroy(child.gameObject);
        }
        inventorySlots.Clear();

        // Set padding and spacing for the GridLayoutGroup
        GridLayoutGroup gridLayout = inventoryGrid.GetComponent<GridLayoutGroup>();
        gridLayout.padding = padding;
        gridLayout.spacing = spacing;
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = columns;

        float width = ((RectTransform)inventoryGrid).rect.width;
        float height = ((RectTransform)inventoryGrid).rect.height;

        // Calculate cell size considering spacing and padding
        float cellWidth = (width - gridLayout.padding.left - gridLayout.padding.right - (gridLayout.spacing.x * (columns - 1))) / columns;
        float cellHeight = (height - gridLayout.padding.top - gridLayout.padding.bottom - (gridLayout.spacing.y * (rows - 1))) / rows;
        gridLayout.cellSize = new Vector2(cellWidth, cellHeight);

        int slotsPerPage = rows * columns;
        pages = Mathf.CeilToInt((float)totalSlots / slotsPerPage);

        for (int i = 0; i < totalSlots; i++)
        {
            GameObject slotObject = Instantiate(slotPrefab, inventoryGrid);
            InventorySlotUI slotUI = slotObject.GetComponent<InventorySlotUI>();
            if (slotUI != null)
            {
                slotUI.slot.SetTransformProperties(); // Ensure transform properties are set
                inventorySlots.Add(slotUI.slot);

                // Set the InventorySlot reference in the DragHandler
                InventoryDragHandler dragHandler = slotObject.transform.Find("DraggableItem").GetComponent<InventoryDragHandler>();
                if (dragHandler != null)
                {
                    dragHandler.slot = slotUI.slot;
                }
            }
        }

        UpdatePage();
    }

    public void AddItem(InventoryItem item, int quantity = 1)
    {
        InventorySlot existingSlot = FindItemSlot(item);

        if (existingSlot != null && item.isStackable)
        {
            existingSlot.quantity += quantity;
            existingSlot.stackText.text = existingSlot.quantity.ToString();
        }
        else
        {
            InventorySlot newSlot = inventorySlots.Find(slot => slot.item == null);
            if (newSlot != null)
            {
                newSlot.SetItem(item, quantity);
                newSlot.SetTransformProperties();
            }
            else
            {
                Debug.LogWarning("Inventory is full!");
            }
        }
    }

    public void RemoveItem(InventoryItem item, int quantity = 1)
    {
        InventorySlot existingSlot = FindItemSlot(item);

        if (existingSlot != null)
        {
            existingSlot.quantity -= quantity;
            if (existingSlot.quantity <= 0)
            {
                existingSlot.SetItem(null, 0);
            }
            else
            {
                existingSlot.stackText.text = existingSlot.quantity.ToString();
            }
        }
        else
        {
            Debug.LogWarning("Item not found in inventory!");
        }
    }

    private InventorySlot FindItemSlot(InventoryItem item)
    {
        return inventorySlots.Find(slot => slot.item == item);
    }

    public void SetPage(int pageIndex)
    {
        currentPage = pageIndex;
        UpdatePage();
    }

    private void UpdatePage()
    {
        int slotsPerPage = rows * columns;
        int startSlot = currentPage * slotsPerPage;
        int endSlot = Mathf.Min(startSlot + slotsPerPage, totalSlots);

        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (i >= startSlot && i < endSlot)
            {
                inventorySlots[i].slotObject.SetActive(true);
            }
            else
            {
                inventorySlots[i].slotObject.SetActive(false);
            }
        }
    }
}

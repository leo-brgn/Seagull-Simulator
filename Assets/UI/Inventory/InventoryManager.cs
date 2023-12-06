using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public int maxStack = 5;
    public InventorySlot[] inventorySlots;

    public GameObject inventoryItemPrefab;

    private int selectedSlotIndex = -1;

    private void Start() {
        ChangeSelectedSlot(0);
    }

    private void Update() {
        if (Input.inputString != null) {
            bool isNumber = int.TryParse(Input.inputString, out int number);
            if (isNumber && number > 0 && number <= 9) {
                ChangeSelectedSlot(number - 1);
            }
        }
    }

    void ChangeSelectedSlot(int newValue) {
        if (selectedSlotIndex > -1) {
            inventorySlots[selectedSlotIndex].Deselect();
        }
        inventorySlots[newValue].Select();
        selectedSlotIndex = newValue;
    }

    public bool AddItem(Item item)
    {
        // Check if any slot has the same item and count < max(range)
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot inventorySlot = inventorySlots[i];
            InventoryItem inventoryItem = inventorySlot.GetComponentInChildren<InventoryItem>();
            if (inventoryItem != null)
            {
                if (inventoryItem.item == item && inventoryItem.count < maxStack && item.stackable)
                {
                    inventoryItem.count++;
                    inventoryItem.RefreshCount();
                    return true;
                }
            }
        }

        // find empty slot
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot inventorySlot = inventorySlots[i];
            InventoryItem inventoryItem = inventorySlot.GetComponentInChildren<InventoryItem>();
            if (inventoryItem == null)
            {
                SpawnNewItem(item, inventorySlot);
                return true;
            }
        }
        return false;

    }

    void SpawnNewItem(Item item, InventorySlot inventorySlot)
    {
        GameObject newItem = Instantiate(inventoryItemPrefab, inventorySlot.transform);
        InventoryItem inventoryItem = newItem.GetComponent<InventoryItem>();
        inventoryItem.InitializeItem(item);
    }

    public Item GetSelectedItem(bool use)
    {
        if (selectedSlotIndex > -1)
        {
            InventorySlot inventorySlot = inventorySlots[selectedSlotIndex];
            InventoryItem inventoryItem = inventorySlot.GetComponentInChildren<InventoryItem>();
            if (inventoryItem != null)
            {
                Item item = inventoryItem.item;
                if (use) {
                    inventoryItem.count--;
                    if (inventoryItem.count <= 0) {
                        Destroy(inventoryItem.gameObject);
                    } else {
                        inventoryItem.RefreshCount();
                    }
                }
                return item;
            }
        }
        return null;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public int maxStack = 5;
    public float throwForce = 20f;
    public GameObject _seagull;
    public SeagullInputs _input;

    public InventorySlot[] inventorySlots;

    public GameObject inventoryItemPrefab;

    private int selectedSlotIndex = -1;

    private void Start() {
        if (_seagull == null)
        {
            _seagull = GameObject.FindGameObjectWithTag("Player");
            if (_seagull == null)
            {
                Debug.LogError("Seagull not found");
            }
        }
        if (_input == null)
        {
            _input = _seagull.GetComponent<SeagullInputs>();
            if (_input == null)
            {
                Debug.LogError("SeagullInputs not found");
            }
        }
        ChangeSelectedSlot(0);
    }

    private void Update() {
        if (Input.inputString != null) {
            bool isNumber = int.TryParse(Input.inputString, out int number);
            if (isNumber && number > 0 && number <= 9) {
                ChangeSelectedSlot(number - 1);
            }
        } else if (_input.mouseWheel != 0) {
            int newSlotIndex = selectedSlotIndex + (int) (_input.mouseWheel * -1 / 2);
            if (newSlotIndex < 0) {
                newSlotIndex = 8;
            } else if (newSlotIndex > 8) {
                newSlotIndex = 0;
            }
            ChangeSelectedSlot(newSlotIndex);
        }
        // Add use
        if (_input.use) {
            _input.use = false;
            Item item = GetSelectedItem(true);
            if (item != null) {
                item.Use();
            }
        }
        // Add throw
        if (_input.throwObject) {
            _input.throwObject = false;
            Item item = GetSelectedItem(true);
            if (item != null) {
                Vector3 position = _seagull.transform.position;
                position += _seagull.transform.up;
                GameObject itemGo = Instantiate(item.prefab, position, Quaternion.identity);
                itemGo.GetComponent<Rigidbody>().AddForce(_seagull.transform.forward* throwForce, ForceMode.Impulse);
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
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null && 
                itemInSlot.item == item && 
                itemInSlot.count < maxStack &&
                itemInSlot.item.stackable)
            {
                itemInSlot.count++;
                itemInSlot.RefreshCount();
                return true;
            }
        }


        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot == null)
            {
                SpawnNewItem(item, slot);
                return true;
            }
        }

        return false;

    }

    void SpawnNewItem(Item item, InventorySlot slot)
    {
        GameObject newItemGo = Instantiate(inventoryItemPrefab, slot.transform);
        InventoryItem inventoryItem = newItemGo.GetComponent<InventoryItem>();
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

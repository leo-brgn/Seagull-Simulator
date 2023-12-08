using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public Item item;
    public InventoryManager _inventory;

    public void Start()
    {
        if (_inventory == null)
        {
            _inventory = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
            if (_inventory == null)
            {
                Debug.LogError("InventoryManager not found");
            }
        }
    }

    public void Interact()
    {
        bool success = _inventory.AddItem(item);
        if (success) {
            if (item.name == "Doritos") {
                QuestsManager questsManager = GameObject.Find("QuestsManager").GetComponent<QuestsManager>();
                questsManager.CompleteQuest("Steal doritos");
            }
            Destroy(gameObject);
        } else {
            // TODO: Show message to player
            Debug.Log("Inventory full");
        }
    }
}

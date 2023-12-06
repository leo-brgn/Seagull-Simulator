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
        _inventory.AddItem(item);
        Destroy(gameObject);
    }
}

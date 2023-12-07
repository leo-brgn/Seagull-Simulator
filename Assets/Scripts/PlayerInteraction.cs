using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    public SeagullInputs _input;
    public InventoryManager _inventory;
    public GameObject _helpText;
    public float interactionDistance = 2.0f;

    void Start() {
        if (_input == null)
        {
            _input = gameObject.GetComponent<SeagullInputs>();
            if (_input == null)
            {
                Debug.LogError("SeagullInputs not found");
            }
        }
        if (_helpText == null)
        {
            _helpText = GameObject.Find("HelpText");
            if (_helpText == null)
            {
                Debug.LogError("HelpText not found");
            }
        }
    }
    
    private void LateUpdate() {
        
        RaycastHit hit;
        Vector3 startPosition = transform.position + transform.up * 0.01f;
        Vector3 forward = transform.TransformDirection(Vector3.forward) * interactionDistance;
        Debug.DrawRay(startPosition, forward, Color.green); // Dessine un rayon vert pour le débogage


        if (Physics.Raycast(startPosition, transform.forward, out hit, interactionDistance))
        {
            if (hit.collider.CompareTag("Interactable"))
            {
                _helpText.GetComponent<TMP_Text>().text = "Press E to interact";

                if (Input.GetKeyDown(KeyCode.E)) // Touche d'interaction, par exemple E
                {
                    Interactable interactable = hit.collider.GetComponent<Interactable>();
                    if (interactable != null)
                    {
                        interactable.Interact();
                    }
                }
            }
        }
        else
        {
            _helpText.GetComponent<TMP_Text>().text = "";
        }
        
    }

   
}

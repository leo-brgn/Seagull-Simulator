using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainInventory : MonoBehaviour
{
    public GameObject _seagull;
    public SeagullInputs _input;
    private bool visible = false;
    void Awake()
    {
        gameObject.GetComponent<Image>().enabled = visible;
        // Disable all children
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            gameObject.transform.GetChild(i).gameObject.SetActive(visible);
        }
    }

    void Start()
    {
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
    }

    void Update()
    {
        if (_input.inventory)
        {
            _input.inventory = false;
            visible = !visible;
            gameObject.GetComponent<Image>().enabled = visible;
            // Change all children
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                gameObject.transform.GetChild(i).gameObject.SetActive(visible);
            }
        }
    }
}

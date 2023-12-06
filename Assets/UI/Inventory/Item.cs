using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Scriptable object/Item")]
public class Item : ScriptableObject {


    [Header("Only gameplay")]
    public GameObject prefab;
    public ItemType type;
    public ActionType actionType;
    public Vector2Int range = new Vector2Int(5, 4);


    [Header("Only UI")]
    public bool stackable = false;

    [Header("Both")]
    public Sprite sprite;

}

public enum ItemType
{
    Food,
    Weapon,
    Tool,
}

public enum ActionType
{
    Eat,
    Attack,
    Use,
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] string itemName;
    
    public string GetName() { return itemName; }

    public void PickUp()
    {
        FindAnyObjectByType<Player>().AddToInventory(itemName);
        Destroy(gameObject);
    }
}

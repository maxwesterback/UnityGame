using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName="New Item", menuName = "Inventory/Items")]
public class InventoryItem : ScriptableObject
{
    public string itemName;
    public string itemDescription;
    public Sprite itemImage;
    public int itemAmount;
    public bool usable;
    public bool unique;
    public UnityEvent thisEvent;

    public void Use()
    {
            thisEvent.Invoke();
    }

    public void IncreaseAmount(int amountToIncrease)
    {
        itemAmount += amountToIncrease;
    }
    public void DecreaseAmount(int amountToDecrese)
    {
        itemAmount-= amountToDecrese;
        if(itemAmount<0)
        {
            itemAmount = 0;
        }
    }
}
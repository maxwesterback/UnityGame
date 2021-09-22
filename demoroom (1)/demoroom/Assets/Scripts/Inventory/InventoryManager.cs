using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{

    /*[Header("Inventory Informmation")]
    public PlayerInventory playerinventory;
    [SerializeField] private GameObject blankInventorySlot;
    [SerializeField] private GameObject inventoryPanel;
    
    public InventoryItem currentItem;

    public void SetTextAndButton(string itemName,  string description, bool buttonActive)
    {
        nameText.text = itemName;
        descriptionText.text = description;
        if(buttonActive)
        {
            useButton.SetActive(true);
        }
        else
        {
            useButton.SetActive(false);
        }
    }

    void MakeInventorySlots()
    {
        if(playerinventory)
        {
            for (int i = 0; i < playerinventory.myInventory.Count; i++)
            {
                if (playerinventory.myInventory[i].itemAmount > 0 || playerinventory.myInventory[i].itemName == "Bread")
                {
                    GameObject temp =
                        Instantiate(blankInventorySlot, inventoryPanel.transform.position, Quaternion.identity);
                    temp.transform.SetParent(inventoryPanel.transform);
                    InventorySlot newSlot = temp.GetComponent<InventorySlot>();
                    if (newSlot)
                    {
                        newSlot.Setup(playerinventory.myInventory[i], this);
                    }
                }
            }
        }
    }*/
    // Start is called before the first frame update

    [SerializeField] private Text descriptionText;
    [SerializeField] private TextMeshProUGUI foodAmount;
    [SerializeField] private GameObject useButton;
    [SerializeField] private Text useItemName;

    [SerializeField] private Button swordSlot;
    [SerializeField] private Button shieldSlot;
    [SerializeField] private Button handsSlot;
    [SerializeField] private Button feetSlot;
    [SerializeField] private Button beltSlot;
    [SerializeField] private Button foodSlot;

    [SerializeField] private InventoryItem food;

    [SerializeField] private GameObject sword;
    [SerializeField] private GameObject shield;
    [SerializeField] private GameObject hands;
    [SerializeField] private GameObject feet;
    [SerializeField] private GameObject belt;

    [SerializeField] private Sprite swordInactive;
    [SerializeField] private Sprite swordActive;
    [SerializeField] private Sprite shieldInactive;
    [SerializeField] private Sprite shieldActive;
    [SerializeField] private Sprite handsInactive;
    [SerializeField] private Sprite handsActive;
    [SerializeField] private Sprite feetInactive;
    [SerializeField] private Sprite feetActive;
    [SerializeField] private Sprite beltInactive;
    [SerializeField] private Sprite beltActive;
    [SerializeField] private Sprite foodActive;
    [SerializeField] private Sprite foodInactive;

    [SerializeField] private IsometricPlayerMovement player;

    void Start()
    {
        //MakeInventorySlots();
        SetDesc("");
    }

    public void SetDesc(string item)
    {
        if (item == "Sword")
        {
            if (sword.activeSelf)
            { descriptionText.text = "My trusty sword, although a bit dull"; }
            else { descriptionText.text = "Can't really do much without my sword"; }
        }
        if (item == "Shield")
        {
            if (shield.activeSelf) { descriptionText.text = "I should be able to fend off some attacks with this"; }
            else { descriptionText.text = "This is where i would have my shield. IF I HAD IT"; }
        }
        if (item == "Belt")
        {
            if (belt.activeSelf) { descriptionText.text = "With this, I can move freely"; }
            else { descriptionText.text = "Can't move too much without my belt, my pants might fall"; }
        }
        if (item == "Hands")
        {
            if (hands.activeSelf) { descriptionText.text = "I can grip my sword better with these"; }
            else { descriptionText.text = "I can't use my sword efficiently without my gloves"; }
        }
        if (item == "Feet")
        {
            if (feet.activeSelf) { descriptionText.text = "No need to worry about sharp things on the floor with these on"; }
            else { descriptionText.text = "I should find my shoes, don't want to step into any sharp stones"; }
        }
        if (item == "Food")
        {
            if (food.itemAmount > 0) { descriptionText.text = "Some dried bread"; useButton.SetActive(true); useItemName.text = "Food"; }
            else { descriptionText.text = "There should be bread around somewhere, although dry"; useButton.SetActive(false); useItemName.text = ""; }
        }
        if (item == "")
        {
            descriptionText.text = "";

            useButton.SetActive(false);
        }

    }

    public void Opna()
    {
        foodAmount.text = ""+food.itemAmount;
        if (sword.activeSelf)
        {
            swordSlot.GetComponent<Image>().sprite = swordActive;
        }
        else
        {
            swordSlot.GetComponent<Image>().sprite = swordInactive;
        }
        if (shield.activeSelf)
        {
            shieldSlot.GetComponent<Image>().sprite = shieldActive;
        }
        else
        {
            shieldSlot.GetComponent<Image>().sprite = shieldInactive;
        }
        if (hands.activeSelf)
        {
            handsSlot.GetComponent<Image>().sprite = handsActive;
        }
        else
        {
            handsSlot.GetComponent<Image>().sprite = handsInactive;
        }
        if (feet.activeSelf)
        {
            feetSlot.GetComponent<Image>().sprite = feetActive;
        }
        else
        {
            feetSlot.GetComponent<Image>().sprite = feetInactive;
        }
        if (belt.activeSelf)
        {
            beltSlot.GetComponent<Image>().sprite = beltActive;
        }
        else
        {
            beltSlot.GetComponent<Image>().sprite = beltInactive;
        }
        if(food.itemAmount >0 )
        {
            foodSlot.GetComponent<Image>().sprite = foodActive;
        }
        else
        {
            foodSlot.GetComponent<Image>().sprite = foodInactive;
        }
    }

    public void Consume()
    {
        string item = useItemName.text;
        if(item == "Food")
        {
            if(food.itemAmount > 0)
            {
                if (player.currentHealth != player.maxHealth) {
                    Debug.Log("Ate food");
                    player.currentHealth += 10;
                    food.itemAmount -= 1;
                    foodAmount.text = "" + food.itemAmount; }
                else
                {
                    Debug.Log("I'm already full");
                }
            }
            else
            {
                Debug.Log("No food left");
            }
        }
    }

    /*void ClearInventorySlots()
    {
        for(int i = 0; i< inventoryPanel.transform.childCount; i ++)
        {
            Destroy(inventoryPanel.transform.GetChild(i).gameObject);
        }
    }

    public void UseButtonPressed()
    {
        if(currentItem)
        {
            currentItem.Use();
            //Clear all of the inventory slots
            ClearInventorySlots();
            //Refill all slots with new numbers
            MakeInventorySlots();
            if (currentItem.itemAmount == 0)
            {
                SetTextAndButton("", "", false);
            }
        }
    }*/
}


using System.Collections.Generic;
using UnityEngine;

public class PauseMenuInventoryManagement : MonoBehaviour
{
    //member variables
    //drag in all inventory slots
    [SerializeField] private PauseMenuInventoryManagementSlot[] inventoryManagementSlot = null;
    public GameObject inventoryManagementDraggedItemPrefab;
    [SerializeField] private Sprite transparent16x16 = null;

    [HideInInspector] public GameObject inventoryTextBoxGameobject;


    private void OnEnable()
    {
        //whenever items are switched in the pause menu
        EventHandler.InventoryUpdatedEvent += PopulatePlayerInventory;

        // populate player inventory
        //make sure inventory exsists
        if(InventoryManager.Instance != null)
        {
            PopulatePlayerInventory(InventoryLocation.player, InventoryManager.Instance.inventoryLists[(int)InventoryLocation.player]);
        }
    }

    private void OnDisable()
    {
        //unsubscribe on disable
        EventHandler.InventoryUpdatedEvent -= PopulatePlayerInventory;
        DestroyInventoryTextBoxGameobject();
    }

    public void DestroyInventoryTextBoxGameobject()
    {
        //destroy inventory textbox if created
        if(inventoryTextBoxGameobject != null)
        {
            //no random text boxes
            Destroy(inventoryTextBoxGameobject);
        }
    }

//as we exit then make sure any currently dragged items are destroyed
    public void DestroyCurrentlyDraggedItems()
    {
        //loop through all player inventory ideas
        for(int i = 0; i < InventoryManager.Instance.inventoryLists[(int)InventoryLocation.player].Count; i++)
        {
            if(inventoryManagementSlot[i].draggedItem != null)
            {
                Destroy(inventoryManagementSlot[i].draggedItem);
            }
        }
    }
    //populate all inventory slots
    private void PopulatePlayerInventory(InventoryLocation inventoryLocation, List<InventoryItem> playerInventoryList)
    {
        //check that the inventory location is the player inventory
        if(inventoryLocation == InventoryLocation.player)
        {
            InitialiseInventoryManagementSlots();

            //loop through all player inventory items
            for(int i = 0; i< InventoryManager.Instance.inventoryLists[(int)InventoryLocation.player].Count; i++)
            {
                //get inventory item details
                inventoryManagementSlot[i].itemDetails = InventoryManager.Instance.GetItemDetails(playerInventoryList[i].itemCode);
                inventoryManagementSlot[i].itemQuantity = playerInventoryList[i].itemQuantity;

                if(inventoryManagementSlot[i].itemDetails != null)
                {
                    //update inventory management slot with image and quanity
                    inventoryManagementSlot[i].inventoryManagementSlotImage.sprite = inventoryManagementSlot[i].itemDetails.itemSprite;
                    inventoryManagementSlot[i].textMeshProUGUI.text = inventoryManagementSlot[i].itemQuantity.ToString();
                }

            }
        }
    }

    private void InitialiseInventoryManagementSlots()
    {
        //clear inventory slots
        for(int i = 0; i <Settings.playerMaximumInventoryCapacity; i++)
        {
            //resets the images, clears the images that the player can occupy
            inventoryManagementSlot[i].greyedOutImageGO.SetActive(false);
            inventoryManagementSlot[i].itemDetails = null;
            inventoryManagementSlot[i].itemQuantity = 0;
            inventoryManagementSlot[i].inventoryManagementSlotImage.sprite = transparent16x16;
            inventoryManagementSlot[i].textMeshProUGUI.text = "";
        }

        //grey out unavailable slots
        for(int i = InventoryManager.Instance.inventoryListCapacityIntArray[(int)InventoryLocation.player]; i < Settings.playerMaximumInventoryCapacity; i++)
        {
            //set the players inactive slots to false
            inventoryManagementSlot[i].greyedOutImageGO.SetActive(true);
        }
    }
}

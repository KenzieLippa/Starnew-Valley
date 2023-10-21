using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInventoryBar : MonoBehaviour
{
    [SerializeField] private Sprite blank16x16sprite = null;
    [SerializeField] private UIInventorySlot[] inventorySlot = null;
    //drag in the the slots in the inspector
    public GameObject inventoryBarDraggedItem;
    [HideInInspector] public GameObject inventoryTextBoxGameobject;

    //need to subscribe to the inventory add event otherwise it wont matter



    //want position component from the inventory bar
    private RectTransform rectTransform;

    private bool _isInventoryBarPositionBottom = true;
    //retrieve and set value of the private bool
    public bool IsInventoryBarPositionBottom { get => _isInventoryBarPositionBottom; set => _isInventoryBarPositionBottom = value; }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    //unsubscribe
    private void OnDisable()
    {
        EventHandler.InventoryUpdatedEvent -= InventoryUpdated;
    }


    //susbscribe
    private void OnEnable()
    {
        EventHandler.InventoryUpdatedEvent += InventoryUpdated;
    }

    

    private void Update()
    {
        //based on player position
        SwitchInventoryBarPosition();
    }
    /// <summary>
	/// clear highlights from the inventory bar
	/// </summary>
	public void ClearHighlightOnInventorySlots()
    {
        //inventory slot array check to see its not 0
        if (inventorySlot.Length > 0)
        {
            //loop through inventory slots and clear highlight sprites
            //loop through the inventory
            for(int i = 0; i<inventorySlot.Length; i++)
            {
                if (inventorySlot[i].isSelected)
                {
                    //set to be not selected
                    inventorySlot[i].isSelected = false;
                    //set to be a new color that has no alpha, makes it transparent
                    inventorySlot[i].inventorySlotHighlight.color = new Color(0f, 0f, 0f, 0f);
                    //update inventory to show item as not selected
                    //clear the inventory item for the location
                    InventoryManager.Instance.ClearSelectedInventoryItem(InventoryLocation.player);
                    
                }
            }
        }
    }


    private void ClearInventorySlots()
    {
        //are there spots in the thingy
        if (inventorySlot.Length > 0)
        {
            for(int i = 0; i<inventorySlot.Length; i++)
            {
                //for each slot clear everything
                inventorySlot[i].inventorySlotImage.sprite = blank16x16sprite;
                inventorySlot[i].textMeshProUGUI.text = "";
                inventorySlot[i].itemDetails = null;
                inventorySlot[i].itemQuantity = 0;

                //call method and if slot isnt selected it will clear it
                SetHighlightedInventorySlots(i);
            }
        }
    }

    //parameters the same as with the event that was triggered
    private void InventoryUpdated(InventoryLocation inventoryLocation, List<InventoryItem> inventoryList)
    {
        //check if location is equal to the player
        if(inventoryLocation == InventoryLocation.player)
        {
            //clear the slots of info and then rebuild then with whats currently in there
            ClearInventorySlots();

            //do they have slots in them?
            //do they have more in their inventory themn 0
            if(inventorySlot.Length > 0 && inventoryList.Count > 0)
            {
                //loop through all and re-update the system
                //populTE the 12 slots
                for(int i =0; i<inventorySlot.Length; i++)
                {
                    //have we gone above the number of items the player holds?
                    //if not then enter in the items
                    if(i < inventoryList.Count)
                    {
                        int itemCode = inventoryList[i].itemCode;
                        ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(itemCode);


                        if(itemDetails != null)
                        {
                            //get item details and fill out information, extract item details and populate the slot
                            inventorySlot[i].inventorySlotImage.sprite = itemDetails.itemSprite;
                            //set up text and item details
                            inventorySlot[i].textMeshProUGUI.text = inventoryList[i].itemQuantity.ToString();
                            inventorySlot[i].itemDetails = itemDetails;
                            inventorySlot[i].itemQuantity = inventoryList[i].itemQuantity;
                            //as image updated then double check its been selected
                            SetHighlightedInventorySlots(i);
                        }

                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }

    /// <summary>
	/// Set the selected highlight if set on an inventory item position
	/// </summary>

    public void SetHighlightedInventorySlots()
    {
        if(inventorySlot.Length > 0)
        {
            //loop through inventory slots and clear highlight sprites
            for(int i = 0; i<inventorySlot.Length; i++)
            {
                SetHighlightedInventorySlots(i);
            }
        }
    }
    /// <summary>
	/// Set the selected highlight if set on an inventory item for the given slot position
	/// </summary>

    public void SetHighlightedInventorySlots(int itemPosition)
    {
        if(inventorySlot.Length>0 && inventorySlot[itemPosition].itemDetails != null)

        {
            //check if selected, then make the graphic visable
            if (inventorySlot[itemPosition].isSelected)
            {
                inventorySlot[itemPosition].inventorySlotHighlight.color = new Color(1f, 1f, 1f, 1f);

                //update item to show as selected
                InventoryManager.Instance.SetSelectedInventoryItem(InventoryLocation.player, inventorySlot[itemPosition].itemDetails.itemCode);
            }
        }

    }

    private void SwitchInventoryBarPosition()
    {
        //calls just created method to get the viewport position
        Vector3 playerViewportPosition = Player.Instance.GetPlayerViewportPosition();

        //see where they are on the screen, top or bottom
        //more than a third of the screen and the bar is on the top then move bar to the bottom
        //do this by adjusting values with the bar
        if(playerViewportPosition.y >0.3f&& IsInventoryBarPositionBottom == false)
        {
            rectTransform.pivot = new Vector2(0.5f, 0f);
            //currently on the transform according to the inspector
            rectTransform.anchorMin = new Vector2(0.5f, 0f);
            rectTransform.anchorMax = new Vector2(0.5f, 0f);
            rectTransform.anchoredPosition = new Vector2(0f, 2.5f);

            //once moved is true
            IsInventoryBarPositionBottom = true;

        }

        //the opposite of whats above basically
        else if (playerViewportPosition.y <= 0.3f && IsInventoryBarPositionBottom == true)
        {
            rectTransform.pivot = new Vector2(0.5f, 1f);
            rectTransform.anchorMin = new Vector2(0.5f, 1f);
            rectTransform.anchorMax = new Vector2(0.5f, 1f);
            rectTransform.anchoredPosition = new Vector2(0f, -2.5f);

            IsInventoryBarPositionBottom = false;
        }
    }

    public void DestroyCurrentlyDraggedItems()
    {
        //loops through slots, refrences dragged item field, if its not null then it will be deleted.
        for(int i = 0; i <inventorySlot.Length; i++)
        {
            if(inventorySlot[i].draggedItem != null)
            {
                Destroy(inventorySlot[i].draggedItem);
            }
        }
    }
    //clear selected item
    public void ClearCurrentlySelectedItems()
    {
        for(int i = 0; i < inventorySlot.Length; i++)
        {
            inventorySlot[i].ClearSelectedItem();
        }
    }
}

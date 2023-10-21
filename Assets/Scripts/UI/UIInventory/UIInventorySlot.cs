using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//trigger the ipointer when they enter and exit the game object
public class UIInventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    //need in order to call screenToWorldPoint which converts a screen point to a vector
    private Camera mainCamera;
    private Canvas parentCanvas;
    //when creating textbox pair it under here
    private Transform parentItem;
    private GridCursor gridCursor;
    private Cursor cursor;
    public GameObject draggedItem; //used to be private
    //set as the highlight graphic and is used
    public Image inventorySlotHighlight;
    public Image inventorySlotImage;
    public TextMeshProUGUI textMeshProUGUI;

    //item details and quantity
    [SerializeField] private UIInventoryBar inventoryBar = null;
    //refrence to prefab so you can refrence it easily
    [SerializeField] private GameObject inventoryTextBoxPrefab = null;

    [HideInInspector] public bool isSelected = false;
    //serializeFields appear in the inspector to be populated
    [HideInInspector] public ItemDetails itemDetails;
    //refrence to the item prefab
    [SerializeField] private GameObject itemPrefab = null;
    [HideInInspector] public int itemQuantity;
    [SerializeField] private int slotNumber = 0;

    private void Awake()
    {
        //retrieve value of canvas
        parentCanvas = GetComponentInParent<Canvas>();
    }
    //unsubscribe
    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= SceneLoaded;
        EventHandler.RemoveSelectedItemFromInventoryEvent -= RemoveSelectedItemFromInventory;
        EventHandler.DropSelectedItemEvent -= DropSelectedItemAtMousePosition;
    }


    //subscribe to the method that enables it
    private void OnEnable()
    {
        //event to listen to, subscribe to scene loaded
        EventHandler.AfterSceneLoadEvent += SceneLoaded;
        //subscribes and links to the position we already have
        EventHandler.RemoveSelectedItemFromInventoryEvent += RemoveSelectedItemFromInventory;
        EventHandler.DropSelectedItemEvent += DropSelectedItemAtMousePosition;
    }
    private void Start()
    {
        //refrence to main camera then set it
        mainCamera = Camera.main;
        //populate with the findGameObject tag
        //when game objects are parented then we will need to find this because we will be adding addatively because it might not always
        //be there and if it is then we should find it
        //next line no longer needed because now its set to look for it when a scene change occurs instead of on start
        //parentItem = GameObject.FindGameObjectWithTag(Tags.ItemsParentTransform).transform;

        gridCursor = FindObjectOfType<GridCursor>();
        //find object
        cursor = FindObjectOfType<Cursor>();

    }

    //disable the cursor, set type to none
    private void ClearCursors()
    {
        //disable cursor
        gridCursor.DisableCursor();

        cursor.DisableCursor();
        //set the item type to none
        gridCursor.SelectedItemType = ItemType.none;
        cursor.SelectedItemType = ItemType.none;
    }

    /// <summary>
	/// Sets the selected inventory slot
	/// </summary>
    //called when clicking on the inventory bar
    private void SetSelectedItem()
    {
        //clear currently highlighted item
        inventoryBar.ClearHighlightOnInventorySlots();

        //highlight item on inventory bar
        isSelected = true;

        inventoryBar.SetHighlightedInventorySlots();
        //set selected inventory item in inventory
        //calls this method from the inventory manager, location and inventory code

        //set radius, based on item use grid radius already established
        gridCursor.ItemUseGridRadius = itemDetails.itemUseGridRadius;

        cursor.ItemUseRadius = itemDetails.itemUseRadius;

        //if item requires a grid cursor then enable cursor
        if (itemDetails.itemUseGridRadius > 0)
        {
            gridCursor.EnableCursor();
        }
        else
        {
            gridCursor.DisableCursor();
        }
        //if item requires a cursor than enable a cursor
        if(itemDetails.itemUseRadius > 0f)
        {
            cursor.EnableCursor();

        }
        else
        {
            cursor.DisableCursor();
        }


        //set item type
        gridCursor.SelectedItemType = itemDetails.itemType;
        cursor.SelectedItemType = itemDetails.itemType;

        InventoryManager.Instance.SetSelectedInventoryItem(InventoryLocation.player, itemDetails.itemCode);


        //check if can be carried
        if(itemDetails.canBeCarried == true)
        {
            //pass in the show carried item method from the player class/ this instance of the player
            Player.Instance.ShowCarriedItem(itemDetails.itemCode);
        }
        else
        {
            Player.Instance.ClearCarriedItem();
        }
  

    }
    public void ClearSelectedItem()//used to be private as well
    {

        ClearCursors();
        //clear currently selected item
        inventoryBar.ClearHighlightOnInventorySlots();
        isSelected = false;

        //select nothing to be highlighted
        InventoryManager.Instance.ClearSelectedInventoryItem(InventoryLocation.player);

        //clear from players hand
        Player.Instance.ClearCarriedItem();
    }

    /// <summary>
	/// Drops the item if (if selected) at the current mouse position
	/// </summary>
	private void DropSelectedItemAtMousePosition()
    {
        if(itemDetails !=null && isSelected)
        {
            //calc where to place the item when it will be dropped based on the calculations
            //camera position needs to be negative because camera is at -10 and want to place it at the opposite of that
            //if can drop item here
            //world to cell method passes in a world position and returns a grid positions
            /* Vector3Int gridPosition = GridPropertiesManager.Instance.grid.WorldToCell(worldPosition);
             //use this to get the info for the cell and retrieve the property details if they exsist
             GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(gridPosition.x, gridPosition.y);

             if(gridPropertyDetails != null && gridPropertyDetails.canDropItem)
             {*/
            if (gridCursor.CursorPositionIsValid)
            { 

                Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));

                //create object prefab at mouse location
                //parent it and dont rotate it
                GameObject itemGameObject = Instantiate(itemPrefab, new Vector3(worldPosition.x, worldPosition.y-Settings.gridCellSize/2f,worldPosition.z), Quaternion.identity, parentItem);
                //get item component, store it here 
                Item item = itemGameObject.GetComponent<Item>();
                //set item code to be the item code of the item dragged
                item.ItemCode = itemDetails.itemCode;


                //remove the item from inventory
                InventoryManager.Instance.RemoveItem(InventoryLocation.player, item.ItemCode);

                //if there are no more of an item then clear the selected
                if(InventoryManager.Instance.FindItemInInventory(InventoryLocation.player, item.ItemCode) == -1)
                {
                    //check to see if there are any other items left for the location
                    ClearSelectedItem();
                }

            }

        }
    }

    private void RemoveSelectedItemFromInventory()
    {
        //check if current item details arent null and is selected
        if (itemDetails != null && isSelected)
        {
            //item code to current item code
            int itemCode = itemDetails.itemCode;
            //remove an item from the players inventory
            InventoryManager.Instance.RemoveItem(InventoryLocation.player, itemCode);

            //if no more of item then clear selected
            //if item in inventory and only one then clear it
            if(InventoryManager.Instance.FindItemInInventory(InventoryLocation.player, itemCode) == -1)
            {
                ClearSelectedItem();
            }
        }
    }
	

    //enable it to respond to drag events
    //works with other interfaces that allow drags to work

    public void OnBeginDrag(PointerEventData eventData)
    {
        //itemdetails is populated when things are added to the slot
        if(itemDetails != null)
        { 
            //disable player movement
            Player.Instance.DisablePlayerInputAndResetMovement();

            //instantiate the dragged item as a new game object
            draggedItem = Instantiate(inventoryBar.inventoryBarDraggedItem, inventoryBar.transform);

            //get image for dragged object
            Image draggedItemImage = draggedItem.GetComponentInChildren<Image>();
            draggedItemImage.sprite = inventorySlotImage.sprite;

            SetSelectedItem();

        }

    }

    public void OnDrag(PointerEventData eventData)
    {
        if(draggedItem != null)
        {
            //if it exsists, set its position to the mouse position
            draggedItem.transform.position = Input.mousePosition;
        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        //Destroy game object as is dragged out
        //dragged item appears with the mouse
        if (draggedItem != null)
        {
            //destroy the mouse dragged item
            Destroy(draggedItem);

            //if over inventory bar swap it in the inventory bar, from slot to slot
            if(eventData.pointerCurrentRaycast.gameObject !=null && eventData.pointerCurrentRaycast.gameObject.GetComponent<UIInventorySlot>()!= null)
            {
                //get the slot number and trigger a swap
                //first get the slot number and store it in a variable
                int toSlotNumber = eventData.pointerCurrentRaycast.gameObject.GetComponent<UIInventorySlot>().slotNumber;


                //trigger a method to swap the inventory items
                InventoryManager.Instance.SwapInventoryItems(InventoryLocation.player, slotNumber, toSlotNumber);

                //destroy textbox
                DestroyInventoryTextBox();

                //Clear Selected item
                ClearSelectedItem();
            }
            //else drop the item if it can be dropped
            else
            {
                //if over the scene and you want to drop it and it can be dropped
                if (itemDetails.canBeDropped)
                {
                    DropSelectedItemAtMousePosition();

                }
            }
            //enable player input
            Player.Instance.EnablePlayerInput();
        }

    }

    //method that corresponds to the iPointerClickHandler
    public void OnPointerClick(PointerEventData eventData)
    {
        //if left click
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            //if slot currently selected then deselect
            if(isSelected == true)
            {
                ClearSelectedItem();

            }
            else
            {
                //if holding something

                if (itemQuantity > 0)
                {
                    SetSelectedItem();
                }
            }

        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //populates textbox with item details
        if(itemQuantity != 0)
        {
            //as cursor enters/ hovers over a slot we test to see if there is actually an item there, then instantiate a textbox

            //quaternion.identity means to not rotate
            //instantiate the inventory text box
            inventoryBar.inventoryTextBoxGameobject = Instantiate(inventoryTextBoxPrefab, transform.position, Quaternion.identity);

            //change to be the parent transform cached earlier
            inventoryBar.inventoryTextBoxGameobject.transform.SetParent(parentCanvas.transform, false);

            UIInventoryTextBox inventoryTextBox = inventoryBar.inventoryTextBoxGameobject.GetComponent<UIInventoryTextBox>();

            //set item type description
            string itemTypeDescription = InventoryManager.Instance.GetItemTypeDescription(itemDetails.itemType);

            //populate textbox
            inventoryTextBox.SetTextboxText(itemDetails.itemDescription, itemTypeDescription, "", itemDetails.itemLongDescription, "", "");

            //where to put the text based on where tbe bar is
            //Set text position according to inventory bar position
            if (inventoryBar.IsInventoryBarPositionBottom)
            {
                inventoryBar.inventoryTextBoxGameobject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0f);
                inventoryBar.inventoryTextBoxGameobject.transform.position = new Vector3(transform.position.x, transform.position.y + 50f, transform.position.z);

            }
            else
            {
                inventoryBar.inventoryTextBoxGameobject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1f);
                inventoryBar.inventoryTextBoxGameobject.transform.position = new Vector3(transform.position.x, transform.position.y - 50f, transform.position.z);
            }
        }
    }

    //destroy when the mouse exits
    public void OnPointerExit(PointerEventData eventData)
    {
        DestroyInventoryTextBox();
    }

    //checks that the gameobject exsists and then destroys it
    public void DestroyInventoryTextBox()
    {
        if(inventoryBar.inventoryTextBoxGameobject != null)
        {
            Destroy(inventoryBar.inventoryTextBoxGameobject);
        }
    }
    public void SceneLoaded()
    {
        //once loaded look for tag
        parentItem = GameObject.FindGameObjectWithTag(Tags.ItemsParentTransform).transform;
    }
}

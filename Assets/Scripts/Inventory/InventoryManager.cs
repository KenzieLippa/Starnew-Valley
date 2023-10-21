
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : SingletonMonobehaviour<InventoryManager>, ISaveable
    //creates singleton of the game object
{
    //reference the inventory bar so we can use it for any deslections that might be nessiscary
    private UIInventoryBar inventoryBar;
    //make a dictionary to hold the objects/ items
    //makes it easier to access item details through item code
    private Dictionary<int, ItemDetails> itemDetailsDictionary;
    //the first one is the data type, the second is where its coming from or will come from, use <> for the input of info

    private int[] selectedInventoryItem; //index of the array is the inventory list, value is the item code
    //for each inventory location we can store an item code

    //define an array of inventory lists
    //array of item type listed before it
    //number of different lists, two locations
    //new list for player and for chest or any other location

    //index is the inventory list that we specify capacity for
    public List<InventoryItem>[] inventoryLists;

    [HideInInspector] public int[] inventoryListCapacityIntArray;

    

    [SerializeField] private SO_ItemList itemList = null;
    // the SO_ItemList is a script name and because of how unity works it is a class

        //private list that appears in modifier
        //properties
    private string _iSaveableUniqueID;
    public string ISaveableUniqueID{get { return _iSaveableUniqueID;} set{_iSaveableUniqueID = value;}}
    private GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave{get {return _gameObjectSave;} set{_gameObjectSave = value;}}


    protected override void Awake()
    {

        //base class is the singleton monobehaviour class, run all that function still and then after that you can add things
        base.Awake();

        CreateInventoryLists();

        CreateItemDetailsDictionary();
        //create the dictionary in the awake method instead of the start method to ensure it is created before it is accessed

        //defined type of list, now need to create them and trigger that here

        //initialize selected inventory item array
        //length determined by the quantity we have
        selectedInventoryItem = new int[(int)InventoryLocation.count];

        for(int i = 0; i<selectedInventoryItem.Length; i++)
        {
            selectedInventoryItem[i] = -1;
            //-1 indicates we dont have an item selected
        }
        //get unique id for game object and create save data
        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;
//creating new game object
        GameObjectSave = new GameObjectSave();

    }
    private void OnDisable()
    {
        ISaveableDeregister();
    }

//on enable register on disable deregister

    private void OnEnable()
    {
        ISaveableRegister();
    }
    private void Start()
    {
        inventoryBar = FindObjectOfType<UIInventoryBar>();
    }
    private void CreateInventoryLists()
    {
        //inventory list is the lists and then the number of elements corresponds to the number of possible
        inventoryLists = new List<InventoryItem>[(int)InventoryLocation.count];
        //loop through the array and for every part of that array it creates a new list of those inventory items

        for(int i = 0; i < (int)InventoryLocation.count; i++)
        {
            //player will be inventoryList[0] and chest is inventoryList[1] is chest
            inventoryLists[i] = new List<InventoryItem>();

        }
        //initialize array
        //determines and specifies how much each list can hold
        //array of two integers
        inventoryListCapacityIntArray = new int[(int)InventoryLocation.count];
        //access player capacity/ set it at capacity
        //set the capacity to what the settings value was set at

        inventoryListCapacityIntArray[(int)InventoryLocation.player] = Settings.playerInitialInventoryCapacity;

    }
    /// <summary>
    /// Populates the item details dictionary using the scriptable objects you add
    /// </summary>
    private void CreateItemDetailsDictionary()
    {
        itemDetailsDictionary = new Dictionary<int, ItemDetails>();
        //want to loop through scriptable item details and populate them
        foreach (ItemDetails itemDetails in itemList.itemDetails)
        {
            //ItemDetails is the script/class name, itemDetails is the name of the variable you've just made and itemList has a function called itemDetails
            itemDetailsDictionary.Add(itemDetails.itemCode, itemDetails);
            //populate based on values retrieved

        }


    }

    ///<summary>
    ///add an item to the inventory list for the inventory location
    ///</summary>
    //takes in an enum and an item which is a class type
    public void AddItem(InventoryLocation inventoryLocation, Item item)
    {
        int itemCode = item.ItemCode;
        //set to be what the players inventory already is
        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation];


        //check to see if already contains item in question
        int itemPosition = FindItemInInventory(inventoryLocation, itemCode);
        if (itemPosition != -1)
        {
            //stack items
            AddItemAtPosition(inventoryList, itemCode, itemPosition);

        }
        else
        {
            //add item to be stacked
            AddItemAtPosition(inventoryList, itemCode);
        }
        //notify subscribers
        EventHandler.CallInventoryUpdatedEvent(inventoryLocation, inventoryLists[(int)inventoryLocation]);
    }
    //method overload to have one method with different numbers of parameters

    ///<summary>
	///Add item to the end of the inventory
	/// </summary>
	// adds to the end of the list
	private void AddItemAtPosition(List<InventoryItem> inventoryList, int itemCode)
    {
        //create a new item for the list
        InventoryItem inventoryItem = new InventoryItem();
        //allocate item struct and quantity then add it to the list

        inventoryItem.itemCode = itemCode;
        inventoryItem.itemQuantity = 1;
        inventoryList.Add(inventoryItem);

        //DebugPrintInventoryList(inventoryList);
    }


    ///<summary>
	///Add a game object to the list and destroy the one in world
	/// </summary>
	public void AddItem(InventoryLocation inventoryLocation, Item item, GameObject gameObjectToDelete)
    {
        //call the original add item method with the two parameters your supposed to have
        AddItem(inventoryLocation, item);
        //just allows you to destroy the game object
        Destroy(gameObjectToDelete); 
    }

    ///<summaru>
    /// Add an item of type item code to the inventory list for the inventory location
    ///</summary>
    public void AddItem(InventoryLocation inventoryLocation, int itemCode)
    {
        //reurns the list in for the given inventory location
        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation];

        //check if inventory already contains the item
        int itemPosition = FindItemInInventory(inventoryLocation, itemCode);
        //if item already exsists then add one
        if(itemPosition != -1)
        {
            AddItemAtPosition(inventoryList, itemCode, itemPosition);
        }
        //if doesnt exsist then it adds the item into the inventory seperately
        else
        {
            AddItemAtPosition(inventoryList, itemCode);
        }

        //send event that inventory has been updated
        //triggers subs to tell them an item has been added
        EventHandler.CallInventoryUpdatedEvent(inventoryLocation, inventoryLists[(int)inventoryLocation]);
    }


    ///<summary>
	///Adds item at a position in the inventory
	/// </summary>

    private void AddItemAtPosition(List<InventoryItem> inventoryList, int itemCode, int position)
    {
        //new item
        InventoryItem inventoryItem = new InventoryItem();
        //position is determined by the find inventory item method
        //access at a position index
        int quantity = inventoryList[position].itemQuantity + 1;
        //get quantity and add one to it
        inventoryItem.itemQuantity = quantity;
        //update the value to be where it would normally be
        inventoryItem.itemCode = itemCode;
        //reset the given position
        inventoryList[position] = inventoryItem;

        //Debug.ClearDeveloperConsole();
        //DebugPrintInventoryList(inventoryList);
    }

    //will need to also have a third method that is able to destroy the game object once its picked up


    ///<summary>
	///Swap item from frontItem index to the new index to the toItem the player has moused over in the inventoryLocation inventory list
	/// </summary>

    //takes in a location and a from item and to item
    public void SwapInventoryItems(InventoryLocation inventoryLocation, int fromItem, int toItem)
    {
        //if front item index and toItem index are within the list, not the same, are greater then or equal to 0
        //compare against the list count to see if they are in the bounds
        if(fromItem< inventoryLists[(int)inventoryLocation].Count && toItem < inventoryLists[(int)inventoryLocation].Count
            && fromItem != toItem && fromItem>=0 && toItem >= 0)
        {
            //create a from item and allocate it to the from item index that was passed through
            InventoryItem fromInventoryItem = inventoryLists[(int)inventoryLocation][fromItem];
            //use the toitem index to locate this one like the others
            InventoryItem toInventoryItem = inventoryLists[(int)inventoryLocation][toItem];

            //swap the two
            inventoryLists[(int)inventoryLocation][toItem]= fromInventoryItem;
            inventoryLists[(int)inventoryLocation][fromItem] = toInventoryItem;

            //send event that inventory has been updated
            //refresh the ui to show that the items were swapped

            EventHandler.CallInventoryUpdatedEvent(inventoryLocation, inventoryLists[(int)inventoryLocation]);

        }
    }


    ///<summary>
	/// Clear the selected inventory item for inventoryLocation
	/// </summary>

    //takes a location and then in the index given then clear the selection
    public void ClearSelectedInventoryItem(InventoryLocation inventoryLocation)
    {
        selectedInventoryItem[(int)inventoryLocation] = -1;
    }
    
    
    ///<summary>
	///Finds if an item is already in the inventory and returns its posiiton
	/// </summary>

    //takes in item code and inventory list
    public int FindItemInInventory(InventoryLocation inventoryLocation, int itemCode)
    {
        //allocate to the inventory list for the location passed in
        //index is the inventorys location
        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation];
        //loop through the inventory list using the count property to get the total number of items     
        for(int i = 0; i <inventoryList.Count; i++)
        {
            if(inventoryList[i].itemCode == itemCode)
            {
                return i;
            }
        }
        return -1;
    }
     
   
    /// <summary>
	/// Returns the itemDetails from the SO_Item list unless there is none and then it will return nothing
	/// </summary>
    public ItemDetails GetItemDetails(int itemCode)
    {
        ItemDetails itemDetails;
        //try to get a value from whats been passed
        if(itemDetailsDictionary.TryGetValue (itemCode, out itemDetails))
            //can use powerful debugging tools
        {
            //what you want and where it goes
            return itemDetails;

        }
        else
        {
            return null;
        }

         
    }
    ///<summary>
	///Returns the item details (from the SO_itemlist) for the current selected item in the inventorylocation or null if an item isnt selected
	/// </summary>

    public ItemDetails GetSelectedInventoryItemDetails(InventoryLocation inventoryLocation)
    {
        int itemCode = GetSelectedInventoryItem(inventoryLocation);

        if(itemCode == -1)
        {
            return null;
        }
        else
        {
            //returns the item details based on the code
            return GetItemDetails(itemCode);
        }
    }

    ///<summary>
	///Get the selected item  for inventory location - return item code or -1 if nothing is selected
	/// </summary>

    //pass in an inventory location and return the selected item
    private int GetSelectedInventoryItem(InventoryLocation inventoryLocation)
    {
        return selectedInventoryItem[(int)inventoryLocation];
    }

    ///<summary>
	///Gets the item type description for an item type - returns the item description as a string for the item type
	/// </summary>

    //returns a string
    public string GetItemTypeDescription(ItemType itemType)
    {
        string itemTypeDescription;
        switch (itemType)
        {
            //execute this switch based on the item type
            case ItemType.Breaking_tool:
                itemTypeDescription = Settings.BreakingTool;
                break;

            case ItemType.Chopping_tool:
                itemTypeDescription = Settings.ChoppingTool;
                break;

            case ItemType.Hoeing_tool:
                itemTypeDescription = Settings.HoeingTool;
                break;

            case ItemType.Reaping_tool:
                itemTypeDescription = Settings.ReapingTool;
                break;

            case ItemType.Watering_tool:
                itemTypeDescription = Settings.WateringTool;
                break;

            case ItemType.Collecting_tool:
                itemTypeDescription = Settings.CollectingTool;
                break;

            //if not a tool then its set to the item type and set as a string
            default:
                itemTypeDescription = itemType.ToString();
                break;

        }
        return itemTypeDescription;
    }

    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Add(this);
    }
    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
    }
    public GameObjectSave ISaveableSave()
    {
        //create new scene save
        SceneSave sceneSave = new SceneSave();

        //remove any existing scene save for persistant scene for this gameobject
        //inventory manager is on the persistent scene so measure/use data against this
        GameObjectSave.sceneData.Remove(Settings.PersistentScene);

        //add inventory lists array to persistent scene save
        sceneSave.listInvItemArray = inventoryLists;

        //add inventory list capacity array to persistent scene
        //hold number which is the max for the list
        sceneSave.intArrayDictionary = new Dictionary<string, int[]>();
        sceneSave.intArrayDictionary.Add("inventoryListCapacityArray", inventoryListCapacityIntArray);

        //add scene save for game object
        //add scene save to scene data
        GameObjectSave.sceneData.Add(Settings.PersistentScene, sceneSave);
        //runs isaveable save and lets it get accumulated
        return GameObjectSave;

    }
    public void ISaveableLoad(GameSave gameSave)
    {
        //does isaveable save in reverse
        //loops through all objects held and for each then it triggers the load method on the object
        //allows each game object to unpack its load data
        //retrieve the game object save information
        if(gameSave.gameObjectData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            //allocated to member variable
            GameObjectSave = gameObjectSave;

            //need to find inventory lists- start by trying to locate scence save for game object
            //try get persistent scene out of it
            if(gameObjectSave.sceneData.TryGetValue(Settings.PersistentScene, out SceneSave sceneSave))
            {
                //list inv items array exsists for persistent scene
                if(sceneSave.listInvItemArray != null)
                {
                    //retrive the value and add it back in, player and chest inventory list
                    inventoryLists = sceneSave.listInvItemArray;

                    //send events that inventory has been updated
                    for(int i = 0; i <(int)InventoryLocation.count; i++)
                    {
                        //call for each one that exists
                        EventHandler.CallInventoryUpdatedEvent((InventoryLocation)i, inventoryLists[i]);
                    }

                    //clear all items player was carying
                    Player.Instance.ClearCarriedItem();

                    //clear any highlights on inventory bar
                    inventoryBar.ClearHighlightOnInventorySlots();
                }
                //int array dictionary exists for scene
                //retrieve if found
                if(sceneSave.intArrayDictionary != null && sceneSave.intArrayDictionary.TryGetValue("inventoryListCapacityArray", out int[]
                inventoryCapacityArray))
                {
                    //set the new value the old one
                    inventoryListCapacityIntArray = inventoryCapacityArray;
                }
            }
        }
    }
    //no need for the store and restore things but need the headers
    public void ISaveableStoreScene(string sceneName)
    {
        //nothing in here bc its in the persistant scene
    }
    public void ISaveableRestoreScene(string sceneName)
    {
        //nothing required here since is the persistant scene
    }



    ///<summary>
	///Removes item from inventory and creates a new one where the object was dropped
	/// </summary>
	///

    //passes in location and item code

    public void RemoveItem(InventoryLocation inventoryLocation, int itemCode)
    {
        //0 index is whats returned
        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation];

        //check if inventory already contains the the item
        int itemPosition = FindItemInInventory(inventoryLocation, itemCode);
        //if does exsist then remove it
        if(itemPosition != -1)
        {
            RemoveItemAtPosition(inventoryList, itemCode, itemPosition);
        }

        EventHandler.CallInventoryUpdatedEvent(inventoryLocation, inventoryLists[(int)inventoryLocation]);
    }
    //list, item code and position
    private void RemoveItemAtPosition(List<InventoryItem> inventoryList, int itemCode, int position)
    {
        //create new empty inventory item
        InventoryItem inventoryItem = new InventoryItem();
        //reduce value by 1
        int quantity = inventoryList[position].itemQuantity - 1;
        //if quantity is not 0 then adjust the inventory item where it is
        if(quantity > 0)
        {
            inventoryItem.itemQuantity = quantity;
            inventoryItem.itemCode = itemCode;
            inventoryList[position] = inventoryItem;
        }
        //if there are no more items left then remove it from the list
        else
        {
            inventoryList.RemoveAt(position);
        }
    }

    ///<summary>
	///set the selected inventory item at the inventory location to itemcode
	/// </summary>

    //accepts two parameters, the enum and the item code
    public void SetSelectedInventoryItem(InventoryLocation inventoryLocation, int itemCode)
    {
        //sets the index as the inventory location and the value as the item code
        selectedInventoryItem[(int)inventoryLocation] = itemCode;
    }
        //private void DebugPrintInventoryList(List<InventoryItem> inventoryList)
        //{

           // foreach(InventoryItem inventoryItem in inventoryList)
           // {
               // Debug.Log("Item Description:" + InventoryManager.Instance.GetItemDetails(inventoryItem.itemCode).itemDescription + "    Item Quantity: " + inventoryItem.itemQuantity);
            //}
            //Debug.Log("***********************************************************");
       // }

 
}

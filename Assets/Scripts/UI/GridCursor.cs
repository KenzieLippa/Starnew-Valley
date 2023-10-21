using System.Collections.Generic; 
using UnityEngine;
using UnityEngine.UI;

public class GridCursor : MonoBehaviour
{
    //use to store the canvas that the cursor sits on
    private Canvas canvas;
    //grid for tile maps
    private Grid grid;
    //refrence to the main camera
    private Camera mainCamera;

    //populate in the inspector
    //image component for the cursor
    [SerializeField] private Image cursorImage = null;
    //rect transform of the cursor
    [SerializeField] private RectTransform cursorRectTransform = null;

    //green and red cursor sprite
    [SerializeField] private Sprite greenCursorSprite = null;
    [SerializeField] private Sprite redCursorSprite = null;

//populated in inspector, retrieve the crop to see if the tool selected is valid
    [SerializeField] private SO_CropDetailsList so_CropDetailsList = null;

    //properties
    //depending on whether its valid or not will be true or false
    private bool _cursorPositionIsValid = false;

    public bool CursorPositionIsValid { get => _cursorPositionIsValid; set => _cursorPositionIsValid = value; }

    //grid radius for selected item and its distance from the player
    private int _itemUseGridRadius = 0;

    public int ItemUseGridRadius { get => _itemUseGridRadius; set => _itemUseGridRadius = value; }

    //type of item that it is
    private ItemType _selectedItemType;

    public ItemType SelectedItemType { get => _selectedItemType; set => _selectedItemType = value; }

    private bool _cursorIsEnabled = false;

    public bool CursorIsEnabled { get => _cursorIsEnabled; set => _cursorIsEnabled = value; }


    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= SceneLoaded;
    }

    //subscribe to the scene loaded method
    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += SceneLoaded;
    }

    //start is called before the first frame updated
    private void Start()
    {
        mainCamera = Camera.main;

        //cache the value of the canvas
        canvas = GetComponentInParent<Canvas>();
    }

    //update is called once per frame
    private void Update()
    {
        if (CursorIsEnabled)
        {
            DisplayCursor();
        }
    }

    private Vector3Int DisplayCursor()
    {
        //check to see if the grid is null
        if(grid != null)
        {
            //get grid position for cursor
            Vector3Int gridPosition = GetGridPositionForCursor();

            //get grid position for player
            Vector3Int playerGridPosition = GetGridPositionForPlayer();

            //set cursor sprite
            SetCursorValidity(gridPosition, playerGridPosition);

            //get rect transform position for cursor
            cursorRectTransform.position = GetRectTransformPositionForCursor(gridPosition);

            return gridPosition;
        }
        else
        {
            return Vector3Int.zero;
        }
    }

    private void SceneLoaded()
    {
        grid = GameObject.FindObjectOfType<Grid>();
    }


    private void SetCursorValidity(Vector3Int cursorGridPosition, Vector3Int playerGridPosition)
    {
        SetCursorToValid();

        //do tests to see and if it fails make it invalid
        //check item use radius is valid

        //difference between placement and player is greater than possible radius
        if(Mathf.Abs(cursorGridPosition.x- playerGridPosition.x) > ItemUseGridRadius
            || Mathf.Abs(cursorGridPosition.y - playerGridPosition.y)> ItemUseGridRadius)
        {
            SetCursorToInvalid();
            return;
        }
        //get selected item details
        //check to see if there are item details to get
        ItemDetails itemDetails = InventoryManager.Instance.GetSelectedInventoryItemDetails(InventoryLocation.player);

        if(itemDetails == null)
        {
            SetCursorToInvalid();
            return;
        }
        GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(cursorGridPosition.x, cursorGridPosition.y);

        if(gridPropertyDetails!= null)
        {
            //Determine cursor validity based on inventory item selected and grid property details
            switch (itemDetails.itemType)
            {
                //test the item type from itemdetilas
                case ItemType.Seed:
                    if (!IsCursorValidForSeed(gridPropertyDetails))
                    {
                        SetCursorToInvalid();
                        return;
                    }
                    break;

                case ItemType.Commodity:
                    if (!IsCursorValidForCommodity(gridPropertyDetails))
                    {
                        SetCursorToInvalid();
                        return;
                    }
                    break;

                    //check if valid
                case ItemType.Watering_tool:
                case ItemType.Hoeing_tool:
                case ItemType.Breaking_tool:
                case ItemType.Chopping_tool:
                case ItemType.Reaping_tool:
                case ItemType.Collecting_tool:

                    if(!IsCursorValidForTool(gridPropertyDetails, itemDetails))
                    {
                        SetCursorToInvalid();
                        return;
                    }
                    break;

                case ItemType.none:
                    break;

                case ItemType.count:
                    break;

                default:
                    break;
            }
        }
        else
        {
            SetCursorToInvalid();
            return;
        }
    }

    /// <summary>
	/// Set the cursor to be invalid
	/// </summary>

    private void SetCursorToInvalid()
    {
        cursorImage.sprite = redCursorSprite;
        CursorPositionIsValid = false;
    }


    /// <summary>
	/// Set the cursor to be valid
	/// </summary>

    private void SetCursorToValid()
    {
        cursorImage.sprite = greenCursorSprite;
        CursorPositionIsValid = true;
    }

    /// <summary>
	/// Test cursor validity for a commodity for the target gridPropertyDetails. return true if valid, false if invalid
	/// </summary>

    private bool IsCursorValidForCommodity(GridPropertyDetails gridPropertyDetails)
    {
        return gridPropertyDetails.canDropItem;
    }

    /// <summary>
	/// Set cursor validity for a seed for the target gridPropertyDetails. return true if valid false if invalid
	/// </summary>

    private bool IsCursorValidForSeed(GridPropertyDetails gridPropertyDetails)
    {
        return gridPropertyDetails.canDropItem;    
    }

    /// <summary>
	/// Sets the cursor as either valid or invalid for the tool for the target gridPropertyDetails. rert
	/// </summary>
    private bool IsCursorValidForTool(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails)
    {
        //switch on tool
        switch (itemDetails.itemType)
        {
            case ItemType.Hoeing_tool:
                //has the ground been dug before and is it possible to dig it
                if(gridPropertyDetails.isDiggable == true && gridPropertyDetails.daysSinceDug == -1)
                {
                    #region Need to get any items at location so we can check if reapable
                    //get world position for cursor
                    //adjust by half to make sure its in the middle
                    Vector3 cursorWorldPosition = new Vector3(GetWorldPositionForCursor().x + 0.5f, GetWorldPositionForCursor().y, 0f);

                    //get list of items at cursor location
                    //if there are items then get the componets
                    List<Item> itemList = new List<Item>();

                    HelperMethods.GetComponentsAtBoxLocation<Item>(out itemList, cursorWorldPosition, Settings.cursorSize, 0f);
                    #endregion

                    //Loop through items found to see if there are any reapable types- we are not letting the player dig where there is reapable scenery
                    bool foundReapable = false;

                    foreach(Item item in itemList)
                    {
                        //use item details
                        if(InventoryManager.Instance.GetItemDetails(item.ItemCode).itemType == ItemType.Reapable_scenary)
                        {
                            foundReapable = true;
                            break;
                        }
                    }
                    if (foundReapable)
                    {
                        //dont dig the scenary
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    //if it isnt diggable or has been dug
                    return false;
                }
            case ItemType.Watering_tool:
                //if ground dug and not watered then return true, if not then its false
                if(gridPropertyDetails.daysSinceDug>-1 && gridPropertyDetails.daysSinceWatered == -1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            //check if axe
            case ItemType.Chopping_tool:
            //check if the basket
            case ItemType.Collecting_tool:
            case ItemType.Breaking_tool:
                //check if item can be harvested with item selected, check if itme is fully grown
                //check if cursor valid
                //if seed is planted then it will be an item code
                //check if seed planted
                if(gridPropertyDetails.seedItemCode != -1)
                {
                    //get crop details for seed
                    //use this method with the seed item tool
                    CropDetails cropDetails = so_CropDetailsList.GetCropDetails(gridPropertyDetails.seedItemCode);

                    //if crop details found
                    if(cropDetails != null)
                    {
                        //check if crop is fully grown
                        //length of crop details array and then the position of the last element in the array
                        if(gridPropertyDetails.growthDays >= cropDetails.growthDays[cropDetails.growthDays.Length -1])
                        {
                            //check if crop can be harvested with the tool selected
                            if(cropDetails.CanUseToolToHarvestCrop(itemDetails.itemCode))
                            {
                                //if tool can be used/if cursor is valid
                                return true;
                            }
                            else
                            {
                                //if its not return false
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                }

                return false;

            default:
                return false;
        }
    }

    //sets the image to transparent and sets enabled to false
    public void DisableCursor()
    {
        cursorImage.color = Color.clear;
        CursorIsEnabled = false;
    }

    //full alpha and sets cursor enabled to true
    public void EnableCursor()
    {
        cursorImage.color = new Color(1f, 1f, 1f, 1f);
        CursorIsEnabled = true;
    }

    public Vector3Int GetGridPositionForCursor()
    {
        //get the vector of the cursor with a screen point to a camera point
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));
        //z is how far the objects are in front of the camera, camera is at -10 so objects are (-)-10 in front = 10
        //convert to a cell
        return grid.WorldToCell(worldPosition);
    }

    public Vector3Int GetGridPositionForPlayer()
    {
        //use premade world to cell to get the grid position
        return grid.WorldToCell(Player.Instance.transform.position);
    }

    //pixel position for cursor
    public Vector2 GetRectTransformPositionForCursor(Vector3Int gridPosition)
    {
        //convert to screen point using the world position
        //convert to pixel based for the canvas layer
        Vector3 gridWorldPosition = grid.CellToWorld(gridPosition);
        Vector2 gridScreenPosition = mainCamera.WorldToScreenPoint(gridWorldPosition);
        //pixel values that relate to the current posiiton
        return RectTransformUtility.PixelAdjustPoint(gridScreenPosition, cursorRectTransform, canvas);

    }
    public Vector3 GetWorldPositionForCursor()
    {
        //returns a vector 3 int from the method and use the cell to world method to convert to a world position.
        //return that position
        //grid cell measured from the bototm left
        return grid.CellToWorld(GetGridPositionForCursor());
    }

}

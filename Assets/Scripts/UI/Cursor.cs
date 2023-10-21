
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cursor : MonoBehaviour
{
    //sadly they dont autocomplete
    private Canvas canvas;
    private Camera mainCamera;
    //populate in the inspector
    //the image of the cursor
    //populate them all with the sprites
    [SerializeField] private Image cursorImage = null;
    [SerializeField] private RectTransform cursorRectTransform = null;
    [SerializeField] private Sprite greenCursorSprite = null;
    [SerializeField] private Sprite transparentCursorSprite = null;
    //instance of the cursor
    [SerializeField] private GridCursor gridCursor = null;

    private bool _cursorIsEnabled = false;

    public bool CursorIsEnabled { get => _cursorIsEnabled; set => _cursorIsEnabled = value; }

    private bool _cursorPositionIsValid = false;

    public bool CursorPositionIsValid { get => _cursorPositionIsValid; set => _cursorPositionIsValid = value; }

//when the item is selected it is held here
    private ItemType _selectedItemType;

    public ItemType SelectedItemType { get => _selectedItemType; set => _selectedItemType = value; }

//items use radius will be held here
//non grid use radius
    private float _itemUseRadius = 0f;

    public float ItemUseRadius { get => _itemUseRadius; set => _itemUseRadius = value; }

    //start is called before first frame update

    //populate canvas and camera
    //because camera is in the UI then you look for the component in parent

    private void Start()
    {
        mainCamera = Camera.main;
        canvas = GetComponentInParent<Canvas>();

    }

    //update is called once per frame
    private void Update()
    {
        //check if enabled
        if(CursorIsEnabled)
        {
            //if it is then display it
            DisplayCursor();

        }
    }
    private void DisplayCursor(){
        //get position for cursor
        //gets mouse screen posiiton
        //then converts that to a world position on the camera and returns it
        Vector3 cursorWorldPosition = GetWorldPositionForCursor();

        //set cursor sprite
        //pass in whats just been calculated
        
        SetCursorValidity(cursorWorldPosition, Player.Instance.GetPlayerCentrePosition());

        //get rect transform position for cursor
        cursorRectTransform.position = GetRectTransformPositionForCursor();

    }

    private void SetCursorValidity(Vector3 cursorPosition, Vector3 playerPosition){
        //sets to be valid
        SetCursorToValid();

        //check use radius corners
        //if tests fail then sets to invalid
        //see whether cursor falls within the corner use areas
        //if it does then set to invalid
        if(
            cursorPosition.x>(playerPosition.x + ItemUseRadius/2f) && cursorPosition.y >(playerPosition.y+ ItemUseRadius/2f)
            ||
            cursorPosition.x< (playerPosition.x - ItemUseRadius/2f) && cursorPosition.y >(playerPosition.y + ItemUseRadius/2f)
            ||
            cursorPosition.x<(playerPosition.x - ItemUseRadius/2f) && cursorPosition.y <(playerPosition.y - ItemUseRadius/2f)
            ||
            cursorPosition.x>(playerPosition.x + ItemUseRadius/2f) && cursorPosition.y <(playerPosition.y - ItemUseRadius/2f)
        ){
            SetCursorToInvalid();
            return;
        }
        //see if falls outside the areas
        //check to see if radius is valid
        if(Mathf.Abs(cursorPosition.x - playerPosition.x)> ItemUseRadius || Mathf.Abs(cursorPosition.y - playerPosition.y)>ItemUseRadius){
            SetCursorToInvalid();
            return;
        }

        //get selected item details
        //look to see what is selected, is cursor valid for that item or not?
        ItemDetails itemDetails = InventoryManager.Instance.GetSelectedInventoryItemDetails(InventoryLocation.player);

        //if no details return then set to invalid and return out
        if(itemDetails == null){
            SetCursorToInvalid();
            return;
        }
        //determine cursor validity based on inventory item selected and what object the cursor is over
        //if the tool type is a:
        switch(itemDetails.itemType){
            case ItemType.Watering_tool:
            case ItemType.Breaking_tool:
            case ItemType.Chopping_tool:
            case ItemType.Hoeing_tool:
            case ItemType.Reaping_tool:
            case ItemType.Collecting_tool:
                //pass through these and check them
                if(!SetCursorValidityTool(cursorPosition, playerPosition, itemDetails)){
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

//probably more in here somewhere

    ///<summary>
	/// sets the cursor to be valid
	/// </summary>
	//set to valid
	private void SetCursorToValid()
    {
        cursorImage.sprite = greenCursorSprite;
        CursorPositionIsValid = true;

        gridCursor.DisableCursor();
    }

    ///<summary>
    /// sets the cursor to be invalid
    ///</summary>
    private void SetCursorToInvalid()
    {
        //transparent image
        cursorImage.sprite = transparentCursorSprite;
        CursorPositionIsValid = false;

        //enable grid cursor
        gridCursor.EnableCursor();
    }


    ///<summary>
    ///Sets the cursor to be valid or invalid for the tool for the target. Returns true if valid or false if invalid
    ///</summary>

    private bool SetCursorValidityTool(Vector3 cursorPosition, Vector3 playerPosition, ItemDetails itemDetails){
        //switch on tool
        //interested in a reaping tool, otherwise ignore it
        switch(itemDetails.itemType){
            case ItemType.Reaping_tool:
                return SetCursorValidityReapingTool(cursorPosition, playerPosition, itemDetails);

            default:
                return false;
         }


    }
    private bool SetCursorValidityReapingTool(Vector3 cursorPosition, Vector3 playerPosition, ItemDetails equippedItemDetails){
        //create a new item list
        List<Item> itemList = new List<Item>();
        //look to see whats at the cursor location
        //component type
        if(HelperMethods.GetComponentsAtCursorLocation<Item>(out itemList, cursorPosition)){
            if(itemList.Count != 0){
                //return any item in the list, then check them to see if they are reapable scenery
                foreach (Item item in itemList)
                {
                    if(InventoryManager.Instance.GetItemDetails(item.ItemCode).itemType == ItemType.Reapable_scenary)
                    {
                        //test to see if its of item type reapable scenary, then set it to true
                        return true;
                    }
                }
            }
        }
        return false;
        
    }

    
    public void DisableCursor()
    {
        //sets color to transparent
        cursorImage.color = new Color(1f, 1f, 1f, 0f);
        //disable
        CursorIsEnabled = false;

    }

    public void EnableCursor()
    {
        //visible
        cursorImage.color = new Color(1f, 1f, 1f, 1f);
        //disable
        CursorIsEnabled = true;

    }

    public Vector3 GetWorldPositionForCursor()
    {
        //talked about earlier
        Vector3 screenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);

        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(screenPosition);

        return worldPosition;
    }

    //gets the screen position and then transforms the screen position

    public Vector2 GetRectTransformPositionForCursor()
    {
        Vector2 screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        //returns a pixel location
        return RectTransformUtility.PixelAdjustPoint(screenPosition, cursorRectTransform, canvas); 
    }
    



}

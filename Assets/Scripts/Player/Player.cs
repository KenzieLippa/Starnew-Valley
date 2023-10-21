using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Player : SingletonMonobehaviour<Player>, ISaveable
{
   //public GameObject reapingPrefab;
    private WaitForSeconds afterLiftToolAnimationPause;
    private WaitForSeconds afterUseToolAnimationPause;
    private WaitForSeconds afterPickAnimationPause;
    private AnimationOverrides animationOverrides;
    private GridCursor gridCursor;
    private Cursor cursor;

    //add movement parameters
    private float xInput;
    private float yInput;
    private bool isCarrying = false;
    private bool isIdle;
    private bool isLiftingToolDown;
    private bool isLiftingToolLeft;
    private bool isLiftingToolRight;
    private bool isLiftingToolUp;
    private bool isRunning;
    private bool isUsingToolDown;
    private bool isUsingToolLeft;
    private bool isUsingToolRight;
    private bool isUsingToolUp;
    private bool isSwingingToolDown;
    private bool isSwingingToolLeft;
    private bool isSwingingToolRight;
    private bool isSwingingToolUp;
    private bool isWalking;
    private bool isPickingUp;
    private bool isPickingDown;
    private bool isPickingLeft;
    private bool isPickingRight;
    private WaitForSeconds liftToolAnimationPause;
    private WaitForSeconds pickAnimationPause;

    private Camera mainCamera;
    private bool playerToolUseDisabled = false;

    private ToolEffect toolEffect = ToolEffect.none;


    private Rigidbody2D rigidBody2D;
    //corresponding to the two in settings
    private WaitForSeconds useToolAnimationPause;

//#pragma warning disable 414
    private Direction playerDirection;
    //used in future for save functionality
//#pragma warning restore 414

    //build up to swap out in terms of animations
    //attributes passed into the animation thing
    private List<CharacterAttribute> characterAttributeCustomisationList;
    //update the sprite renderer
    //display the item held above head
    [SerializeField] private SpriteRenderer equippedItemSpriteRenderer = null;

    //player attributes that can be swaped
    //for the arms
    private CharacterAttribute armsCharacterAttribute;

    private CharacterAttribute toolCharacterAttribute;

    private float movementSpeed;

    private bool _playerInputIsDisabled = false;

    public bool PlayerInputIsDisabled { get => _playerInputIsDisabled; set => _playerInputIsDisabled = value;}

//guid for the game object
    private string _iSaveableUniqueID;
    public string ISaveableUniqueID{get{return _iSaveableUniqueID;} set{_iSaveableUniqueID = value;}}
//save data build up for every object using interface

    private GameObjectSave _gameObjectSave;

    public GameObjectSave GameObjectSave{ get{return _gameObjectSave;} set{_gameObjectSave = value;}}

    protected override void Awake()
    {
        base.Awake();
        rigidBody2D = GetComponent<Rigidbody2D>();

        //populate the animation overrides field that has a refrence to the component
        //the component holds the class
        animationOverrides = GetComponentInChildren<AnimationOverrides>();

        //initialize the swapable character attributes
        //all of these are enums
        //these are default, later when you want to do carry partvarienttype will change
        armsCharacterAttribute = new CharacterAttribute(CharacterPartAnimator.arms, PartVariantColor.none, PartVariantType.none);

        //switch to the scythe need a character attribute for that
        toolCharacterAttribute = new CharacterAttribute(CharacterPartAnimator.tool, PartVariantColor.none, PartVariantType.hoe);

        //initialize the list of character attributes
        characterAttributeCustomisationList = new List<CharacterAttribute>();

        //get unique id for game object and create save data object
        //look for component and get id
        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;

        GameObjectSave = new GameObjectSave();

        //reference to main camera
        mainCamera =  Camera.main;
        
    }

    //unsubscribe to these events
    private void onDisable()
    {
        ISaveableDeregister();
        EventHandler.BeforeSceneUnloadFadeOutEvent -= DisablePlayerInputAndResetMovement;
        EventHandler.AfterSceneLoadFadeInEvent -= EnablePlayerInput;
    }

    private void OnEnable()
    {
        //register in list
        ISaveableRegister();
        //subscribe to these new events and add them to the event handler
        EventHandler.BeforeSceneUnloadFadeOutEvent += DisablePlayerInputAndResetMovement;
        EventHandler.AfterSceneLoadFadeInEvent += EnablePlayerInput;
    }

    private void Start()
    {
        gridCursor = FindObjectOfType<GridCursor>();
        //populate with new wait for seconds values
        //locate cursor
        cursor = FindObjectOfType<Cursor>();

        
        useToolAnimationPause = new WaitForSeconds(Settings.useToolAnimationPause);
        liftToolAnimationPause = new WaitForSeconds(Settings.liftToolAnimationPause);
        pickAnimationPause = new WaitForSeconds(Settings.pickAnimationPause);
        afterUseToolAnimationPause = new WaitForSeconds(Settings.afterUseToolAnimationPause);
        afterLiftToolAnimationPause = new WaitForSeconds(Settings.afterLiftToolAnimationPause);
        afterPickAnimationPause = new WaitForSeconds(Settings.afterPickAnimationPause);
    }

    private void Update()
    {
        #region Player Input
        //visuallly seperate out code
        //reset animation triggers

        if (!PlayerInputIsDisabled)
        {
            ResetAnimationTriggers();

            //capture input
            PlayerMovementInput();

            //shift for walking

            PlayerWalkInput();

            //check every frame
            PlayerTestInput();

            PlayerClickInput();

            //send to listeners
            //code on controllers and sends to different objects
            EventHandler.CallMovementEvent(xInput, yInput, isWalking, isRunning, isIdle, isCarrying, toolEffect,
                isUsingToolRight, isUsingToolLeft, isUsingToolUp, isUsingToolDown,
                isLiftingToolRight, isLiftingToolLeft, isLiftingToolUp, isLiftingToolDown,
                isPickingRight, isPickingLeft, isPickingUp, isPickingDown,
                isSwingingToolRight, isSwingingToolLeft, isSwingingToolUp, isSwingingToolDown,
                false, false, false, false);
            //player input should be in here because its counted every frame
            //triggers player movement in the fixed update


        }

        #endregion Player Input

    }
    private void FixedUpdate()
    {
        PlayerMovement();


    }

    private void PlayerMovement()
    {
        Vector2 move = new Vector2(xInput * movementSpeed * Time.deltaTime, yInput * movementSpeed * Time.deltaTime);
        // time.deltaTime is for the distance between updates which makes it indipendent of the ping
        //one for x and one for y

        rigidBody2D.MovePosition(rigidBody2D.position + move);


    }


    private void ResetAnimationTriggers()
    {
        isPickingRight = false;
        isPickingLeft = false;
        isPickingUp = false;
        isPickingDown = false;
        isUsingToolRight = false;
        isUsingToolLeft = false;
        isUsingToolUp = false;
        isUsingToolDown = false;
        isLiftingToolRight = false;
        isLiftingToolLeft = false;
        isLiftingToolUp = false;
        isLiftingToolDown = false;
        isSwingingToolRight = false;
        isSwingingToolLeft = false;
        isSwingingToolUp = false;
        isSwingingToolDown = false;
        toolEffect = ToolEffect.none;
    }


    private void PlayerMovementInput()
    {
        yInput = Input.GetAxisRaw("Vertical");
        
        xInput = Input.GetAxisRaw("Horizontal");


        if(yInput !=0 && xInput != 0)
        {
            xInput = xInput * 0.71f;
            yInput = yInput * 0.71f;
            //for the diagnol

        }

        if(xInput != 0 || yInput != 0)
        {
            isRunning = true;
            isWalking = false;
            isIdle = false;
            movementSpeed = Settings.runningSpeed;


            //capture for save game

            if(xInput< 0)
            {
                playerDirection = Direction.left;
            }
            else if (xInput > 0)
            {
                playerDirection = Direction.right;

            }
            else if (yInput < 0)
            {
                playerDirection = Direction.down;
            }
            else
            {
                playerDirection = Direction.up;
            }

        }
        else if(xInput == 0 && yInput == 0)
        {
            isRunning = false;
            isWalking = false;
            isIdle = true;
        }

    }

    private void PlayerWalkInput()
    {
        if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            isRunning = false;
            isWalking = true;
            isIdle = false;
            movementSpeed = Settings.walkingSpeed;
            
            
        }
        else
        {
            isRunning = true;
            isWalking = false;
            isIdle = false;
            movementSpeed = Settings.runningSpeed;
            //everything should work and yet for some god damn reason the auto fill is broken
            

        }
    }

    private void PlayerClickInput()
    {
        //use to disable if the animation is playing for the tool use
        if (!playerToolUseDisabled)
        {
            //left mouse button click
            if (Input.GetMouseButton(0))
            {
                //if cursor or grid cursor is enabled
                if (gridCursor.CursorIsEnabled || cursor.CursorIsEnabled)
                {
                //for getting the players position so you can tell which direction the person is facing when the tool is used.

                //get cursor grid position
                Vector3Int cursorGridPosition = gridCursor.GetGridPositionForCursor();

                //get the player grid position
                Vector3Int playerGridPosition = gridCursor.GetGridPositionForPlayer();


                    ProcessPlayerClickInput(cursorGridPosition, playerGridPosition);
                }
            }
        }
    }

    private void ProcessPlayerClickInput(Vector3Int cursorGridPosition, Vector3Int playerGridPosition)
    {
        ResetMovement();

        Vector3Int playerDirection = GetPlayerClickDirection(cursorGridPosition, playerGridPosition);

        //get grid property details at cursor position (the grid cursor validation routine ensures that grid property details are not null
        GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(cursorGridPosition.x, cursorGridPosition.y);

        //get selected item details
        ItemDetails itemDetails = InventoryManager.Instance.GetSelectedInventoryItemDetails(InventoryLocation.player);

        //if exsist or not, then process them
        if(itemDetails != null)
        {
            switch (itemDetails.itemType)
            {
                case ItemType.Seed:
                    if (Input.GetMouseButtonDown(0))
                    {
                        ProcessPlayerClickInputSeed(gridPropertyDetails, itemDetails);
                    }
                    break;

                case ItemType.Commodity:
                    if (Input.GetMouseButtonDown(0))
                    {
                        ProcessPlayerClickInputCommodity(itemDetails);
                    }
                    break;
                case ItemType.Watering_tool:
                case ItemType.Breaking_tool:
                case ItemType.Chopping_tool:
                case ItemType.Hoeing_tool:
                case ItemType.Reaping_tool:
                case ItemType.Collecting_tool:
                    ProcessPlayerClickInputTool(gridPropertyDetails, itemDetails, playerDirection);
                    break;

                case ItemType.none:
                    break;

                case ItemType.count:
                    break;

                default:
                    break;
            }
        }
    }

    private Vector3Int GetPlayerClickDirection(Vector3Int cursorGridPosition, Vector3Int playerGridPosition)
    {
        //return vector3Int of type direction

        //must be clicking right
        if(cursorGridPosition.x > playerGridPosition.x)
        {
            return Vector3Int.right;
        }
        //must be left
        else if (cursorGridPosition.x < playerGridPosition.x)
        {
            return Vector3Int.left;
        }
        //must be up
        else if (cursorGridPosition.y > playerGridPosition.y)
        {
            return Vector3Int.up;
        }
        //default, has to be down
        else
        {
            return Vector3Int.down;
        }
    }

    private Vector3Int GetPlayerDirection(Vector3 cursorPosition, Vector3 playerPosition){
        //pass in player and cursor position
        //use the same tests as for the collision tests
        //check for boxes
        if(
            //if in the right position
            cursorPosition.x > playerPosition.x
            &&
            cursorPosition.y < (playerPosition.y + cursor.ItemUseRadius / 2f)
            &&
            cursorPosition.y >(playerPosition.y - cursor.ItemUseRadius / 2f)
        )
        {
            return Vector3Int.right;

        }
        else if(
            //check player bounds
            cursorPosition.x < playerPosition.x
            &&
            cursorPosition.y < (playerPosition.y + cursor.ItemUseRadius / 2f)
            &&
            cursorPosition.y > (playerPosition.y - cursor.ItemUseRadius / 2f)
        )
        {
            return Vector3Int.left;
        }
        else if(cursorPosition.y > playerPosition.y){
            return Vector3Int.up;
        }
        else{
            return Vector3Int.down;
        }
    }


    private void ProcessPlayerClickInputSeed(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails)
    {
        //make sure grounds not already planted and is dug
        if(itemDetails.canBeDropped && gridCursor.CursorPositionIsValid && gridPropertyDetails.daysSinceDug >-1 && gridPropertyDetails.seedItemCode == -1)
        {
            //if thats true then you can plant
            PlantSeedAtCursor(gridPropertyDetails, itemDetails);
        }
        //testing to see if can be dropped
        else if(itemDetails.canBeDropped && gridCursor.CursorPositionIsValid)
        {
            EventHandler.CallDropSelectedItemEvent();
        }
    }

    private void PlantSeedAtCursor(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails)
    {
        //process if we have crop details for the seed
        if(GridPropertiesManager.Instance.GetCropDetails(itemDetails.itemCode) != null)
        {
            //update grid properties with seed details
            //set the seed item code to = the new thing passed in
            gridPropertyDetails.seedItemCode = itemDetails.itemCode;
            gridPropertyDetails.growthDays = 0;

            //display planted crop at grid property details
            //details called for
            GridPropertiesManager.Instance.DisplayPlantedCrop(gridPropertyDetails);

            //remove item from inventory
            EventHandler.CallRemoveSelectedItemFromInventoryEvent();
        }
    }

    private void ProcessPlayerClickInputCommodity(ItemDetails itemDetails)
    {
        //if valid and can be dropped then call the event
        if(itemDetails.canBeDropped && gridCursor.CursorPositionIsValid)
        {
            EventHandler.CallDropSelectedItemEvent();
        }
    }

    private void ProcessPlayerClickInputTool(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails, Vector3Int playerDirection)
    {
        //switch on tool
        switch (itemDetails.itemType)
        {
            //check for the hoeing tool to start
            case ItemType.Hoeing_tool:
                //check to see if the position is valid
                //can the ground be dug, has it been dug
                if (gridCursor.CursorPositionIsValid)
                {
                    HoeGroundAtCursor(gridPropertyDetails, playerDirection);
                }
                break;
            case ItemType.Watering_tool:
                if (gridCursor.CursorPositionIsValid)
                {
                    WaterGroundAtCursor(gridPropertyDetails, playerDirection);
                }
                break;

            case ItemType.Chopping_tool:
            if(gridCursor.CursorPositionIsValid)
            {
                 //if cursor is valid
                 ChopInPlayerDirection(gridPropertyDetails, itemDetails, playerDirection);
            }
            break;
           
               

            case ItemType.Collecting_tool:
                if(gridCursor.CursorPositionIsValid)
                {
                    //if valid run
                    CollectInPlayerDirection(gridPropertyDetails, itemDetails, playerDirection);
                }
                break;

            case ItemType.Breaking_tool:
                if(gridCursor.CursorPositionIsValid)
                {
                    BreakInPlayerDirection(gridPropertyDetails, itemDetails, playerDirection);
                }
                break;
            case ItemType.Reaping_tool:
                if(cursor.CursorPositionIsValid){
                    //now have recieved the player direction
                    playerDirection = GetPlayerDirection(cursor.GetWorldPositionForCursor(), GetPlayerCentrePosition());
                    ReapInPlayerDirectionAtCursor(itemDetails, playerDirection);
                }
                break;

            default:
                break;
        }
    }
    private void HoeGroundAtCursor(GridPropertyDetails gridPropertyDetails, Vector3Int playerDirection)
    {
        //trigger animation
        StartCoroutine(HoeGroundAtCursorRoutine(playerDirection, gridPropertyDetails));
    }

    private IEnumerator HoeGroundAtCursorRoutine(Vector3Int playerDirection, GridPropertyDetails gridPropertyDetails)
    {
        //disable all input for player and tool use
        //already got the input for the animation, dont need more
        PlayerInputIsDisabled = true;
        playerToolUseDisabled = true;

        //set tool animation to hoe in override animation
        //same method as changing arms but changes the tool
        toolCharacterAttribute.partVariantType = PartVariantType.hoe;
        //clear list
        characterAttributeCustomisationList.Clear();
        //add new attribute to list
        characterAttributeCustomisationList.Add(toolCharacterAttribute);
        //call parameters from the override methods about the thing we want to change, which is the tool variant
        animationOverrides.ApplyCharacterCustomisationParameters(characterAttributeCustomisationList);

        //check direction
        if(playerDirection == Vector3Int.right)
        {
            isUsingToolRight = true;
        }
        else if(playerDirection == Vector3Int.left)
        {
            isUsingToolLeft = true;
        }
        else if(playerDirection == Vector3Int.up)
        {
            isUsingToolUp = true;
        }
        else if(playerDirection == Vector3Int.down)
        {
            isUsingToolDown = true;
        }

        //only return after this length of time
        yield return useToolAnimationPause;

        //set grid property details for dug ground
        if(gridPropertyDetails.daysSinceDug == -1)
        {
            gridPropertyDetails.daysSinceDug = 0;
            //can also have events that pick up the changes
        }
        //set the grid property details to dug ground
        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);

        //display ground tiles
        GridPropertiesManager.Instance.DisplayDugGround(gridPropertyDetails);

        //after animation pause
        //make sure theres a small buffer 
        yield return afterUseToolAnimationPause;

        PlayerInputIsDisabled = false;
        playerToolUseDisabled = false;
    }
    private void WaterGroundAtCursor(GridPropertyDetails gridPropertyDetails, Vector3Int playerDirection)
    {
        StartCoroutine(WaterGroundAtCursorRoutine(playerDirection, gridPropertyDetails));
    }
    private IEnumerator WaterGroundAtCursorRoutine(Vector3Int playerDirection, GridPropertyDetails gridPropertyDetails)
    {
        //disable player input and tool use
        PlayerInputIsDisabled = true;
        playerToolUseDisabled = true;

        //set tool animation to watering can in override animation
        toolCharacterAttribute.partVariantType = PartVariantType.wateringCan;
        characterAttributeCustomisationList.Clear();
        characterAttributeCustomisationList.Add(toolCharacterAttribute);
        //pass in a character attribute customisation list to switch out the part to be the watering can
        animationOverrides.ApplyCharacterCustomisationParameters(characterAttributeCustomisationList);

        //TODO: if theres water in the watering can
        //animation controller for the tool effect on the player
        toolEffect = ToolEffect.watering;

        //check direction and then pick the animation
        if(playerDirection == Vector3Int.right)
        {
            isLiftingToolRight = true;
        }
        else if(playerDirection == Vector3Int.left)
        {
            isLiftingToolLeft = true;
        }
        else if(playerDirection == Vector3Int.up)
        {
            isLiftingToolUp = true;
        }
        else if (playerDirection == Vector3Int.down)
        {
            isLiftingToolDown = true;
        }

        yield return liftToolAnimationPause;

        // set grid property details for watered ground
        if(gridPropertyDetails.daysSinceWatered == -1)
        {
            //indicates that the grounds been watered
            gridPropertyDetails.daysSinceWatered = 0;
        }

        //set the grid property to watered
        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);

        //display watered ground tiles
        GridPropertiesManager.Instance.DisplayWateredGround(gridPropertyDetails);

        //after animation pause
        yield return afterLiftToolAnimationPause;

        PlayerInputIsDisabled = false;
        playerToolUseDisabled = false;

    }
    //react to the player input, starts the coroutine that we will set up, gridproperty details, item details and playerdirection
    private void ChopInPlayerDirection(GridPropertyDetails gridPropertyDetails, ItemDetails equippedItemDetails, Vector3Int playerDirection)
    {
        //trigger animation
        StartCoroutine(ChopInPlayerDirectionRoutine(gridPropertyDetails, equippedItemDetails, playerDirection));
    }

    private IEnumerator ChopInPlayerDirectionRoutine(GridPropertyDetails gridPropertyDetails, ItemDetails equippedItemDetails, Vector3Int playerDirection)
    {
        PlayerInputIsDisabled = true;
        playerToolUseDisabled = true;

        //set tool animation to axe in override animation
        toolCharacterAttribute.partVariantType = PartVariantType.axe;
        characterAttributeCustomisationList.Clear();
        characterAttributeCustomisationList.Add(toolCharacterAttribute);
        animationOverrides.ApplyCharacterCustomisationParameters(characterAttributeCustomisationList);

        ProcessCropWithEquippedItemInPlayerDirection(playerDirection, equippedItemDetails, gridPropertyDetails);

        yield return useToolAnimationPause;

        //after animation pause
        yield return afterUseToolAnimationPause;

        PlayerInputIsDisabled = false;
        playerToolUseDisabled = false;
    }
    private void CollectInPlayerDirection(GridPropertyDetails gridPropertyDetails, ItemDetails equippedItemDetails, Vector3Int playerDirection)
    {
        StartCoroutine(CollectInPlayerDirectionRoutine(gridPropertyDetails, equippedItemDetails, playerDirection));
    }
    private IEnumerator CollectInPlayerDirectionRoutine(GridPropertyDetails gridPropertyDetails, ItemDetails equippedItemDetails, Vector3Int playerDirection)
    {
        //disable input and tool use
        PlayerInputIsDisabled = true;
        playerToolUseDisabled = true;
        //process method
        ProcessCropWithEquippedItemInPlayerDirection(playerDirection, equippedItemDetails, gridPropertyDetails);

        yield return pickAnimationPause;
        //yeild for the pause
        yield return afterPickAnimationPause;
        //reset player movement
        PlayerInputIsDisabled = false;
        playerToolUseDisabled = false;
    }

    private void BreakInPlayerDirection(GridPropertyDetails gridPropertyDetails, ItemDetails equippedItemDetails, Vector3Int playerDirection)
    {
        StartCoroutine(BreakInPlayerDirectionRoutine(gridPropertyDetails, equippedItemDetails, playerDirection));
    }

    private IEnumerator BreakInPlayerDirectionRoutine(GridPropertyDetails gridPropertyDetails, ItemDetails equippedItemDetails, Vector3Int playerDirection)
    {
        //diable input and tool use
        PlayerInputIsDisabled = true;
        playerToolUseDisabled = true;

        //set tool animation to pickaxe in override animation
        //set the override
        toolCharacterAttribute.partVariantType = PartVariantType.pickaxe;
        characterAttributeCustomisationList.Clear();
        characterAttributeCustomisationList.Add(toolCharacterAttribute);
        //apply
        animationOverrides.ApplyCharacterCustomisationParameters(characterAttributeCustomisationList);

        ProcessCropWithEquippedItemInPlayerDirection(playerDirection, equippedItemDetails, gridPropertyDetails);
//add pause
        yield return useToolAnimationPause;

        //after animation pause
        yield return afterUseToolAnimationPause;

        PlayerInputIsDisabled = false;
        playerToolUseDisabled = false;
    }

    private void ReapInPlayerDirectionAtCursor(ItemDetails itemDetails, Vector3Int playerDirection){
        StartCoroutine(ReapInPlayerDirectionAtCursorRoutine(itemDetails, playerDirection));
    }

    private IEnumerator ReapInPlayerDirectionAtCursorRoutine(ItemDetails itemDetails, Vector3Int playerDirection){
        //disable player tool use and player 
        PlayerInputIsDisabled = true;
        playerToolUseDisabled = true;

        //set tool animation to scythe in override animation
        //sets up in character animation with the overides
        toolCharacterAttribute.partVariantType = PartVariantType.scythe;
        characterAttributeCustomisationList.Clear();
        characterAttributeCustomisationList.Add(toolCharacterAttribute);
        //pass list to be applied
        animationOverrides.ApplyCharacterCustomisationParameters(characterAttributeCustomisationList);

        //reap in player direction
        UseToolInPlayerDirection(itemDetails, playerDirection);

        yield return useToolAnimationPause;

        PlayerInputIsDisabled = false;
        playerToolUseDisabled = false;
    }

    private void UseToolInPlayerDirection(ItemDetails equippedItemDetails, Vector3Int playerDirection)
    {
        //check for left mouse button click
        if(Input.GetMouseButton(0)){
            switch(equippedItemDetails.itemType){
                //testing for the reaping tool
                case ItemType.Reaping_tool:
                //based on the player direction we set the animation direction
                    if(playerDirection == Vector3Int.right){
                        isSwingingToolRight = true;
                    }
                    else if(playerDirection == Vector3Int.left){
                        isSwingingToolLeft = true;
                    }
                    else if(playerDirection == Vector3Int.up){
                        isSwingingToolUp = true;
                    }
                    else if(playerDirection == Vector3Int.down){
                        isSwingingToolDown = true;
                    }
                    break;

            }
        }
        //define center point of the square used for collision testing
        //take players current position and then add an amount based on the player direction 
        Vector2 point = new Vector2(GetPlayerCentrePosition().x + (playerDirection.x *(equippedItemDetails.itemUseRadius / 2f)), GetPlayerCentrePosition().y +
        playerDirection.y * (equippedItemDetails.itemUseRadius / 2f));

        //Define size of the square for collision testing 
        Vector2 size = new Vector2(equippedItemDetails.itemUseRadius, equippedItemDetails.itemUseRadius);

        //get item components with 2D collider located in the 2d square at the centre point defined (2d colliders tested limited to maxCollidersToTestPerReapSwing)
        //return item components of type itme with point and size
        //returns an array of items
        Item[] itemArray = HelperMethods.GetComponentsAtBoxLocationNonAlloc<Item>(Settings.maxCollidersToTestPerReapSwing, point, size, 0f);

        int reapableItemCount = 0;
        //loop through all items retrived
        for(int i = itemArray.Length -1; i>=0; i--){
            if(itemArray[i] != null){
                //destroy item game object if reapable
                //if its reapable then you delete it
                //later we will add a custom effect
                if(InventoryManager.Instance.GetItemDetails(itemArray[i].ItemCode).itemType == ItemType.Reapable_scenary){
                    //effect position
                    Vector3 effectPosition = new Vector3(itemArray[i].transform.position.x, itemArray[i].transform.position.y + Settings.gridCellSize /2f, 
                    itemArray[i].transform.position.z);

                    //trigger reaping effect
                    //event position and which type of event it is.
                    //trigger the reaping effect
                    //call the handler for the effect position
                    EventHandler.CallHarvestActionEffectEvent(effectPosition, HarvestActionEffect.reaping);

                //destroy item targeted
                    Destroy(itemArray[i].gameObject);

                    //incrament reapable count
                    reapableItemCount++;
                    //if this is bigger than what we want to destroy then we break out of the for loop
                    if(reapableItemCount >= Settings.maxTargetComponentsToDestroyPerReapSwing){
                        break;
                    }
                }
            }
        }
    }

///<summary>
/// Method process crop with equipped item in player direction
///</summary>

//takes in three parameters
    private void  ProcessCropWithEquippedItemInPlayerDirection(Vector3Int playerDirection, ItemDetails equippedItemDetails, 
    GridPropertyDetails gridPropertyDetails)
    {//switch on the equipped items for the tools
        switch(equippedItemDetails.itemType)
        {
            //if item type is chopping tool
            //based on player direction then set the player use tool
            case ItemType.Chopping_tool:
            case ItemType.Breaking_tool:
           // Debug.Log("I made it here");
                if(playerDirection == Vector3Int.right)
                {
                    isUsingToolRight = true;

                }
                else if(playerDirection == Vector3Int.left)
                {
                    isUsingToolLeft = true;
                }
                else if(playerDirection == Vector3Int.up)
                {
                    isUsingToolUp = true;
                }
                else if(playerDirection == Vector3Int.down)
                {
            
                    isUsingToolDown = true;
                
                }
                break;

            //add more in the future, look at the player direction and then set the animation parameter 
            case ItemType.Collecting_tool:
                if(playerDirection == Vector3Int.right)
                {
                    isPickingRight = true;
                }
                else if(playerDirection == Vector3Int.left)
                {
                    isPickingLeft = true;
                }
                else if(playerDirection == Vector3Int.up)
                {
                    isPickingUp = true;
                }
                else if(playerDirection == Vector3Int.down)
                {
                    isPickingDown = true;
                }
                break;
            case ItemType.none:
            break;
        }
        //get crop at cursor location
        //grid location achieved
        Crop crop = GridPropertiesManager.Instance.GetCropObjectAtGridLocation(gridPropertyDetails);
        //Debug.Log("And now i have made it here");
//testing to see if part of type crop, null otherwise
        //Execute process tool action for crop
       // Debug.Log(crop);
        if (crop != null)
        {
            //Debug.Log("Now i am here");
            switch(equippedItemDetails.itemType)
            {
                
               case ItemType.Chopping_tool:
               case ItemType.Breaking_tool:
               //Debug.Log("AND ALSO HERE");
                    crop.ProcessToolAction(equippedItemDetails, isUsingToolRight, isUsingToolLeft, isUsingToolDown, isUsingToolUp);
                    break;
                //only have collection thusfar
                //harvest itself
                case ItemType.Collecting_tool:
                    crop.ProcessToolAction(equippedItemDetails, isPickingRight, isPickingLeft, isPickingDown, isPickingUp);
                    break;
            }
        }
    }


    //TODO: Remove
    /// <summary>
	/// Temp routine for test input
	/// </summary>

    private void PlayerTestInput()
    {
        //Trigger time advance
        if (Input.GetKey(KeyCode.T))
        {
            TimeManager.Instance.TestAdvanceGameMinute();
        }


        //Trigger day advance
        if (Input.GetKeyDown(KeyCode.G))
        {
            TimeManager.Instance.TestAdvanceGameDay();
        }


        /*//trigger scene load/unload
        if (Input.GetKeyDown(KeyCode.L))
        {
            SceneControllerManager.Instance.FadeAndLoadScene(SceneName.Scene1_Farm.ToString(), transform.position);
        }*/
//if been pressed then pass in all the stuff

       /* if(Input.GetMouseButtonDown(1)){
            //find the coords based on the mouse position
            GameObject tree = PoolManager.Instance.ReuseObject(reapingPrefab, mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, 
            //neg z value because the cursor is that distance away from the camera
            Input.mousePosition.y, -mainCamera.transform.position.z)), Quaternion.identity);
            //once trees been retrieved then you put it back
            tree.SetActive(true);
        }*/
    }

    private void ResetMovement()
    {
        //reseting the player input

        xInput = 0f;
        yInput = 0f;
        isRunning = false;
        isWalking = false;
        isIdle = true;
    }

    public void DisablePlayerInputAndResetMovement()
    {
        //disables the input
        DisablePlayerInput();
        //reset movement method
        ResetMovement();
        //call movement with updated parameters

        EventHandler.CallMovementEvent(xInput, yInput, isWalking, isRunning, isIdle, isCarrying, toolEffect,
                isUsingToolRight, isUsingToolLeft, isUsingToolUp, isUsingToolDown,
                isLiftingToolRight, isLiftingToolLeft, isLiftingToolUp, isLiftingToolDown,
                isPickingRight, isPickingLeft, isPickingUp, isPickingDown,
                isSwingingToolRight, isSwingingToolLeft, isSwingingToolUp, isSwingingToolDown,
                false, false, false, false);

    }



    //disable player input
    public void DisablePlayerInput()
    {
        PlayerInputIsDisabled = true;
    }


    //set player input to be false
    public void EnablePlayerInput()
    {
        PlayerInputIsDisabled = false;
    }

    public void ClearCarriedItem()
    {
        //turn off the equipped sprite renderer
        equippedItemSpriteRenderer.sprite = null;
        //disapear the sprite
        equippedItemSpriteRenderer.color = new Color(1f, 1f, 1f, 0f);


        //apply base character arm customisation
        //switch back
        armsCharacterAttribute.partVariantType = PartVariantType.none;
        //clear the list
        characterAttributeCustomisationList.Clear();
        //add the new one
        characterAttributeCustomisationList.Add(armsCharacterAttribute);
        //set back to the basics 
        animationOverrides.ApplyCharacterCustomisationParameters(characterAttributeCustomisationList);

        isCarrying = false;
    }

    //show the carried item
    //accepts an item code
    //swaps out all arms animation with these animations
    public void ShowCarriedItem(int itemCode)
    {
        //use inventory manager for the item details
        ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(itemCode);
        if(itemDetails != null)
        {
            //extract info
            equippedItemSpriteRenderer.sprite = itemDetails.itemSprite;
            //visable color, default sets it not visable
            equippedItemSpriteRenderer.color = new Color(1f, 1f, 1f, 1f);

            //apply 'carry' character arm customisation
            armsCharacterAttribute.partVariantType = PartVariantType.carry;
            //clear customization list
            characterAttributeCustomisationList.Clear();
            //add what we've just created
            characterAttributeCustomisationList.Add(armsCharacterAttribute);
            //call the overide with this method passing through the customization list
            animationOverrides.ApplyCharacterCustomisationParameters(characterAttributeCustomisationList);

            //set flag to be true
            isCarrying = true;
        }

    }


    //find out where the player is in the viewport
    public Vector3 GetPlayerViewportPosition()
    {
        //vector3 position for the player is (0,0) for the bottom left and (1,1) for the top right
        //uses this worldtoviewportpoint and it returns a vector three for the players position
        return mainCamera.WorldToViewportPoint(transform.position);

    }

    public Vector3 GetPlayerCentrePosition()
    {
        //return the natural center for the player
        //takes the transfrom and adds the player offset from the settings file
        //returns an adjusted value
        return new Vector3(transform.position.x, transform.position.y+ Settings.playerCentreYOffset, transform.position.z );
    }

    public void ISaveableRegister()
    {
        //adds this game object to the list in the save load manager
        SaveLoadManager.Instance.iSaveableObjectList.Add(this);
    }
    public void ISaveableDeregister()
    {
        //removes from the list
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
    }
    //save load manager calls as the scene is saved
    public GameObjectSave ISaveableSave()
    {
        //delete save scene for game object if it already exists
        GameObjectSave.sceneData.Remove(Settings.PersistentScene);

        //create sceneSave for game object
        SceneSave sceneSave = new SceneSave();

        //create vector3 dictionary
        //make a new dictionary to store these values
        sceneSave.vector3Dictionary = new Dictionary<string, Vector3Serializable>();

        //create string dictionary
        sceneSave.stringDictionary = new Dictionary<string, string>();

        //add player position to vector3 dictionary
        //pass in current transform position
        Vector3Serializable vector3Serializable = new Vector3Serializable(transform.position.x, transform.position.y, transform.position.z);
        //put in the vector 3 dictionary so it can be retrieved
        sceneSave.vector3Dictionary.Add("playerPosition", vector3Serializable);

        //add current scene name to string dictionary
        //.name converts to string
        sceneSave.stringDictionary.Add("currentScene", SceneManager.GetActiveScene().name);

        //add player direction to string dictionary
        sceneSave.stringDictionary.Add("playerDirection", playerDirection.ToString());

        //add scene save data for player game object
        GameObjectSave.sceneData.Add(Settings.PersistentScene, sceneSave);

        return GameObjectSave;
    }
    public void ISaveableLoad(GameSave gameSave)
    {
        //try and retrieve the game save data for the player object
        if(gameSave.gameObjectData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            //get save data from dictionary for scene
            //look at the game object save and use settings to try and find the value, then pass it out into the scene save
            if(gameObjectSave.sceneData.TryGetValue(Settings.PersistentScene, out SceneSave sceneSave))
            {
                //get player position
                //do we have a dictionary
                if(sceneSave.vector3Dictionary != null && sceneSave.vector3Dictionary.TryGetValue("playerPosition", out Vector3Serializable playerPosition))
                {
                    //player position
                    transform.position = new Vector3(playerPosition.x, playerPosition.y, playerPosition.z);
                }

                //get string dictionary
                if(sceneSave.stringDictionary != null)
                {
                    //have a string dictionary
                    //get player scene
                    //output return into the current scene object
                    if(sceneSave.stringDictionary.TryGetValue("currentScene", out string currentScene))
                    {
                        //fade and load the scene with the scene name and the player position
                        SceneControllerManager.Instance.FadeAndLoadScene(currentScene, transform.position);
                    }

                    //get player direction
                    if(sceneSave.stringDictionary.TryGetValue("playerDirection", out string playerDir))
                    {
                        //pass in string value and output value as an enum direction
                        bool playerDirFound = Enum.TryParse<Direction>(playerDir, true, out Direction direction);
                        if(playerDirFound)
                        {
                            //set to the direction found
                            playerDirection = direction;
                            SetPlayerDirection(playerDirection);
                        }
                        
                    }
                }
            }
        }
       
    }
    //dont need anything in here because player is in the persistant scene which doesnt have any particular scene plans
    public void ISaveableStoreScene(string sceneName)
    {
        //nothing is required here 
    }
    public void ISaveableRestoreScene(string sceneName)
    {
        //nothing is in here either, same as above
    }
    
     private void SetPlayerDirection(Direction direction)
        {
           switch(playerDirection)
           {
               //depending on direction then generate a movement event
               //last four parameters are the idlel directions
               case Direction.up:
               //set idle up trigger
                    EventHandler.CallMovementEvent(0f,0f, false, false, false, false, ToolEffect.none, false, false, false, false, false, false, false, false,
                    false, false, false, false, false, false, false, false, true, false, false, false);
                    break;
                case Direction.down:
                //set idle down trigger
                    EventHandler.CallMovementEvent(0f,0f, false, false, false, false, ToolEffect.none, false, false, false, false, false, false, false, false,
                    false, false, false, false, false, false, false, false, false, true, false, false);
                    break;
                case Direction.left:
                    EventHandler.CallMovementEvent(0f,0f, false, false, false, false, ToolEffect.none, false, false, false, false, false, false, false, false,
                    false, false, false, false, false, false, false, false, false, false, true, false);
                    break;
                case Direction.right:
                    EventHandler.CallMovementEvent(0f,0f, false, false, false, false, ToolEffect.none, false, false, false, false, false, false, false, false,
                    false, false, false, false, false, false, false, false, false, false, false, true);
                    break;
                default:
                //set idle down trigger
                    EventHandler.CallMovementEvent(0f,0f,false, false, false, false, ToolEffect.none, false, false, false, false, false, false, false, false,
                    false, false, false, false, false, false, false, false, false, true, false, false);
                    break;
           }
        }
}

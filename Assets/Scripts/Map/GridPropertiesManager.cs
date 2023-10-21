//will read through all the SO and using the gridproperties details will populate stuff

using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(GenerateGUID))]


//type singleton monobehavior, attached to a game object in the scene
public class GridPropertiesManager : SingletonMonobehaviour<GridPropertiesManager>, ISaveable
{
//any crops transform here
    private Transform cropParentTransform;
    private Tilemap groundDecoration1;
    private Tilemap groundDecoration2;
    private bool isFirstTimeSceneLoaded = true;

    //member variable of type grid
    private Grid grid;

    //store the property details in here, coords is the key
    private Dictionary<string, GridPropertyDetails> gridPropertyDictionary;

    //array of the SO assests of type so_gridProperties, populated from the grids
    [SerializeField] private SO_CropDetailsList so_CropDetailsList = null;
    [SerializeField] private SO_GridProperties[] so_gridPropertiesArray = null;
    [SerializeField] private Tile[] dugGround = null;
    [SerializeField] private Tile[] wateredGround = null;
    
    private string _iSaveableUniqueID;

    public string ISaveableUniqueID { get { return _iSaveableUniqueID; } set { _iSaveableUniqueID = value; } }

    private GameObjectSave _gameObjectSave;

    public GameObjectSave GameObjectSave { get { return _gameObjectSave; }set { _gameObjectSave = value; } }


    protected override void Awake()
    {
        base.Awake();

        //populate from the GUID
        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;

        //new instance
        GameObjectSave = new GameObjectSave();
    }

    private void OnEnable()
    {
        //registers this object with th isaveable object list in the isave manager
        ISaveableRegister();

        //populate the grid member variable after the scene is loaded
        EventHandler.AfterSceneLoadEvent += AfterSceneLoaded;

        //reset the water tile every day
        EventHandler.AdvanceGameDayEvent += AdvanceDay;
    }

    private void OnDisable()
    {
        //call this method which corresponds to the register, rather than adding it removes it from the list like before
        ISaveableDeregister();

        //deregister from the event
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoaded;
        EventHandler.AdvanceGameDayEvent -= AdvanceDay;
    }

    private void Start()
    {
        //bulk of setup is in here
        //loops through all the SO with the property values from before
        //create a loop from the properties and populate the dictionary
        //list of grid properties to iterate through, pull out the property
        //populate the grid property details
        InitialiseGridProperties();
    }

    private void ClearDisplayGroundDecorations()
    {
        //remove ground decorations
        //clear all tiles

        //dug ground
        groundDecoration1.ClearAllTiles();
        //watered ground
        groundDecoration2.ClearAllTiles();
    }

    private void ClearDisplayAllPlantedCrops()
    {
        //destroy all crops in the scene
        //local array of crops
        //returns a crop array
        Crop[] cropArray;
        cropArray = FindObjectsOfType<Crop>();

        foreach(Crop crop in cropArray)
        {
            //for each crop we destroy them all
            Destroy(crop.gameObject);
        }
    }

    private void ClearDisplayGridPropertyDetails()
    {
        //when scene is restored or advanced
        //will add more but for now its just this
        ClearDisplayGroundDecorations();
        ClearDisplayAllPlantedCrops();
    }
    public void DisplayDugGround(GridPropertyDetails gridPropertyDetails)
    {
        //dug
        if (gridPropertyDetails.daysSinceDug > -1)
        {
            ConnectDugGround(gridPropertyDetails);
        }
    }

    public void DisplayWateredGround(GridPropertyDetails gridPropertyDetails)
    {
        //watered
        //see if watered
        if(gridPropertyDetails.daysSinceWatered > -1)
        {
            ConnectWateredGround(gridPropertyDetails);
        }
    }

    //takes in the grid square that the user is digging
    private void ConnectDugGround(GridPropertyDetails gridPropertyDetails)
    {
        //select tile based on the surounding dug tiles
        //take x and y and select which tile should be placed based on wich surounding tiles are dug
        //this is is passed into dugTile
        Tile dugTile0 = SetDugTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY);
        //selects the tile and then places it on the GD1 tile map
        groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0), dugTile0);

        //set 4 tiles if dug surounding current tile - up, down, left, right, now that this centeral tile has been dug

        //check surrounding tiles to see if they need to be updated
        GridPropertyDetails adjacentGridPropertyDetails;

        //grid square above where we just dug
        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);
        //is it dug
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceDug > -1)
        {
            Tile dugTile1 = SetDugTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);
            groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1, 0), dugTile1);
        }

        //below the tile we just dug
        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1);
        if(adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceDug > -1)
        {
            Tile dugTile2 = SetDugTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1);
            groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1, 0), dugTile2);
        }

        //left tile
        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY);
        if(adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceDug > -1)
        {
            Tile dugTile3 = SetDugTile(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY);
            groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY, 0), dugTile3);
        }

        //right tile
        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
        if(adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceDug > -1)
        {
            Tile dugTile4 = SetDugTile(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
            groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX +1, gridPropertyDetails.gridY, 0), dugTile4);
        }
    }

    private void ConnectWateredGround(GridPropertyDetails gridPropertyDetails)
    {
        //select tile based on surounding tiles
        Tile wateredTile0 = SetWateredTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY);
        groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0), wateredTile0);

        //set 4 tiles if watered surrounding current tile - up, down, left, right now that the center tile is watered
        GridPropertyDetails adjacentGridPropertyDetails;

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);
        if(adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceWatered > -1)
        {
            Tile wateredTile1 = SetWateredTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);
            groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1, 0), wateredTile1);
        }

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1);

        if(adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceWatered > -1)
        {
            Tile wateredTile2 = SetWateredTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1);
            groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1, 0), wateredTile2);
        }

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY);
        if(adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceWatered > -1)
        {
            Tile wateredTile3 = SetWateredTile(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY);
            groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY, 0), wateredTile3);
        }

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
        if(adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceWatered > -1)
        {
            Tile wateredTile4 = SetWateredTile(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
            groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY, 0), wateredTile4);
        }
    }



    private Tile SetDugTile(int xGrid, int yGrid)
    {
        //get whether surrounding tiles (up down left and right) are dug or not

        bool upDug = IsGridSquareDug(xGrid, yGrid + 1);
        bool downDug = IsGridSquareDug(xGrid, yGrid - 1);
        bool leftDug = IsGridSquareDug(xGrid - 1, yGrid);
        bool rightDug = IsGridSquareDug(xGrid + 1, yGrid);

        //16 combos to see which tile to pick
        #region Set appropriate tile based on whether surounding tiles are dug or not
        if(!upDug && !downDug && !rightDug && !leftDug)
        {
            //will populate in tn inspector
            return dugGround[0];
        }
        else if(!upDug &&downDug && rightDug && !leftDug)
        {
            return dugGround[1];
        }
        else if(!upDug && downDug && rightDug && leftDug)
        {
            return dugGround[2];
        }
        else if(!upDug && downDug && !rightDug && leftDug)
        {
            return dugGround[3];
        }
        else if(!upDug && downDug && !rightDug && !leftDug)
        {
            return dugGround[4];
        }
        else if(upDug && downDug && rightDug && !leftDug)
        {
            return dugGround[5];
        }
        else if(upDug && downDug && rightDug && leftDug)
        {
            return dugGround[6];

        }
        else if (upDug && downDug && !rightDug && leftDug)
        {
            return dugGround[7];
        }
        else if(upDug && downDug && !rightDug && !leftDug)
        {
            return dugGround[8];
        }
        else if (upDug && !downDug&& rightDug && !leftDug)
        {
            return dugGround[9];
        }
        else if (upDug && !downDug && rightDug && leftDug)
        {
            return dugGround[10];
        }
        else if(upDug && !downDug && !rightDug&& leftDug)
        {
            return dugGround[11];
        }
        else if(upDug && !downDug&& !rightDug && !leftDug)
        {
            return dugGround[12];
        }
        else if (!upDug && !downDug && rightDug && !leftDug)
        {
            return dugGround[13];
        }
        else if (!upDug && !downDug && rightDug && leftDug)
        {
            return dugGround[14];
        }
        else if (!upDug && !downDug && !rightDug && leftDug)
        {
            return dugGround[15];
        }
        return null;

        #endregion Set appropriate tiles based on whether surrounding tiles were dug or not
    }
    private bool IsGridSquareDug(int xGrid, int yGrid)
    {
        //is it dug or not
        GridPropertyDetails gridPropertyDetails = GetGridPropertyDetails(xGrid, yGrid);
        if(gridPropertyDetails == null)
        {
            return false;
        }
        else if(gridPropertyDetails.daysSinceDug > -1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private Tile SetWateredTile(int xGrid, int yGrid)
    {
        //get whether surrounding tiles are watered or not (up,down, right, left)
        //check to see status of the 4 tiles closest 
        bool upWatered = IsGridSquareWatered(xGrid, yGrid + 1);
        bool downWatered = IsGridSquareWatered(xGrid, yGrid - 1);
        bool leftWatered = IsGridSquareWatered(xGrid - 1, yGrid);
        bool rightWatered = IsGridSquareWatered(xGrid + 1, yGrid);

#region Set appropriate tile based on whether surounding tiles are watered or not
        if(!upWatered && !downWatered && !rightWatered && !leftWatered)
        {
            return wateredGround[0];
        }
        else if(!upWatered && downWatered && rightWatered && !leftWatered)
        {
            return wateredGround[1];
        }
        else if(!upWatered && downWatered && rightWatered && leftWatered)
        {
            return wateredGround[2];
        }
        else if(!upWatered && downWatered && !rightWatered && leftWatered)
        {
            return wateredGround[3];
        }
        else if(!upWatered && downWatered && !rightWatered && !leftWatered)
        {
            return wateredGround[4];
        }
        else if(upWatered && downWatered && rightWatered && !leftWatered)
        {
            return wateredGround[5];
        }
        else if(upWatered && downWatered && rightWatered && leftWatered)
        {
            return wateredGround[6];
        }
        else if(upWatered && downWatered && !rightWatered && leftWatered)
        {
            return wateredGround[7];
        }
        else if(upWatered && downWatered && !rightWatered && !leftWatered)
        {
            return wateredGround[8];
        }
        else if(upWatered && !downWatered && rightWatered && !leftWatered)
        {
            return wateredGround[9];
        }
        else if(upWatered && !downWatered && rightWatered && leftWatered)
        {
            return wateredGround[10];
        }
        else if(upWatered && !downWatered && !rightWatered && leftWatered)
        {
            return wateredGround[11];
        }
        else if(upWatered && !downWatered && !rightWatered && !leftWatered)
        {
            return wateredGround[12];
        }
        else if(!upWatered && !downWatered && rightWatered && !leftWatered)
        {
            return wateredGround[13];
        }
        else if(!upWatered && !downWatered && rightWatered && leftWatered)
        {
            return wateredGround[14];
        }
        else if(!upWatered && !downWatered && !rightWatered && leftWatered)
        {
            return wateredGround[15];
        }
        return null;

#endregion set appropriate tile ased on whether surrounding tiles are wanted or not
    }

    private bool IsGridSquareWatered(int xGrid, int yGrid)
    {
        GridPropertyDetails gridPropertyDetails = GetGridPropertyDetails(xGrid, yGrid);
        //if not watered
        if(gridPropertyDetails == null)
        {
            return false;
        }
        //if watered
        else if (gridPropertyDetails.daysSinceWatered > -1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void DisplayGridPropertyDetails()
    {
        //loop through all grid property details
        //go through all of them and return as a key value pair
        foreach(KeyValuePair<string, GridPropertyDetails> item in gridPropertyDictionary)
        {
            GridPropertyDetails gridPropertyDetails = item.Value;
            DisplayDugGround(gridPropertyDetails);
            DisplayWateredGround(gridPropertyDetails);
            //display planted crops
            DisplayPlantedCrop(gridPropertyDetails);
        }
    }



    /// <summary>
	/// This initialises the grid property dictionary with the values from the SO property assets and stores the values in each scene in
	/// GameObjectSave sceneData
	/// </summary>
    private void InitialiseGridProperties()
    {
        //Loop through all gridproperties in the array
        foreach (SO_GridProperties so_GridProperties in so_gridPropertiesArray)
        {
            //create dictionary of grid property details
            Dictionary<string, GridPropertyDetails> gridPropertyDictionary = new Dictionary<string, GridPropertyDetails>();

            //populate grid property dictionary - iterate through all the grid properties in the so gridproperties list
            foreach (GridProperty gridProperty in so_GridProperties.gridPropertyList)
            {
                GridPropertyDetails gridPropertyDetails;

                //pulling out the grid property and populate the details
                //passes in the coords to look in the dictionary
                gridPropertyDetails = GetGridPropertyDetails(gridProperty.gridCoordinate.x, gridProperty.gridCoordinate.y, gridPropertyDictionary);

                //have grid properties or not
                if (gridPropertyDetails == null)
                {
                    gridPropertyDetails = new GridPropertyDetails();
                }

                //switch with the property
                switch (gridProperty.gridBoolProperty)
                {
                    case GridBoolProperty.diggable:
                        //set to bool value from the SO which tells you whether or not u can do stuff there
                        gridPropertyDetails.isDiggable = gridProperty.gridBoolValue;
                        break;

                    case GridBoolProperty.canDropItem:
                        gridPropertyDetails.canDropItem = gridProperty.gridBoolValue;
                        break;

                    case GridBoolProperty.canPlaceFurniture:
                        gridPropertyDetails.canPlaceFurniture = gridProperty.gridBoolValue;
                        break;

                    case GridBoolProperty.isPath:
                        gridPropertyDetails.isPath = gridProperty.gridBoolValue;
                        break;

                    case GridBoolProperty.isNPCObstacle:
                        gridPropertyDetails.isNPCObstacle = gridProperty.gridBoolValue;
                        break;

                    default:
                        break;
                }

                SetGridPropertyDetails(gridProperty.gridCoordinate.x, gridProperty.gridCoordinate.y, gridPropertyDetails, gridPropertyDictionary);
            }

            //Create scene save for this game object
            //save it for the scene 
            SceneSave sceneSave = new SceneSave();

            //add grid property dictionary to scene save data
            //pull out the field just created and set it to grid property dictionary
            sceneSave.gridPropertyDetailsDictionary = gridPropertyDictionary;

            //if starting scene set the gridPropertyDictionary member variable to the current iteration
            //current scene = starting scene then set it to the currently populated dictionary        
            if (so_GridProperties.sceneName.ToString() == SceneControllerManager.Instance.startingSceneName.ToString())
            {
                this.gridPropertyDictionary = gridPropertyDictionary;
            }
//create new bool dictionary and add a new boolean 
//set value to true, this boolean is saved and in the save scene methods you can now control whether this is loaded or not for the first scene
            sceneSave.boolDictionary = new Dictionary<string, bool>();
            sceneSave.boolDictionary.Add("isFirstTimeSceneLoaded", true);

            //add scene save to game object scene data
            GameObjectSave.sceneData.Add(so_GridProperties.sceneName.ToString(), sceneSave);
        }
    }
    private void AfterSceneLoaded()
    {
        //finds the tag
        if(GameObject.FindGameObjectWithTag(Tags.CropsParentTransform)!= null)
        {
            //sets to the game object we just found
            cropParentTransform = GameObject.FindGameObjectWithTag(Tags.CropsParentTransform).transform;
        }
        else
        {
            cropParentTransform = null;
        }
        //get grid
        grid = GameObject.FindObjectOfType<Grid>();

        //get tilemaps
        groundDecoration1 = GameObject.FindGameObjectWithTag(Tags.GroundDecoration1).GetComponent<Tilemap>();
        groundDecoration2 = GameObject.FindGameObjectWithTag(Tags.GroundDecoration2).GetComponent<Tilemap>();

    }
    ///<summary>
	///Returns the gridPropertiesdetail at the grid location for the supplied dictionary, or null if no properties exist at that location
	/// </summary>

    //constructs a key from the x and y, 
    public GridPropertyDetails GetGridPropertyDetails(int gridX, int gridY, Dictionary<string, GridPropertyDetails> gridPropertyDictionary)
    {
        //construct key from coordinate
        string key = "x" + gridX + "y" + gridY;

        GridPropertyDetails gridPropertyDetails;

        //check if grid property details exist for coordinate and retrieve
        //puts the value in gridproperty details
        if(!gridPropertyDictionary.TryGetValue(key, out gridPropertyDetails))
        {
            //if not found return null
            return null;
        }
        else
        {
            return gridPropertyDetails;
        }
    }

    ///<summary>
    /// Returns the crop object at the gridX, gridY position or null if none was found
    ///</summary>

//takes in gridproperties and returns crop
    public Crop GetCropObjectAtGridLocation(GridPropertyDetails gridPropertyDetails)
    {
        //Debug.Log("I have been assigned");
        //get the position/the logical center coords into position
        //returns the center of the grid cell in world space
        Vector3 worldPosition = grid.GetCellCenterWorld(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0));
        //returns any coliders overlapping each other
        Collider2D[] collider2DArray = Physics2D.OverlapPointAll(worldPosition);

        //loop through colliders to get crop game object
        Crop crop = null;
//see if there is a crop for any collider
        for(int i = 0; i <collider2DArray.Length; i++)
        {
            //looking to find the game object connected to the collider
            //try to locate crop components on game objects
            //locate any crop components
            crop = collider2DArray[i].gameObject.GetComponentInParent<Crop>();
            //check component being returned if any, find if the grid position is equal to the grid properties
            //DUMB ASS!!!
            if(crop != null && crop.cropGridPosition == new Vector2Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY))
                break;
            //just in case, check the children, full check on all objects
            crop = collider2DArray[i].gameObject.GetComponentInChildren<Crop>();
            //crop position not null and crop position is = to the vector
            if (crop != null && crop.cropGridPosition == new Vector2Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY))
                break;
        }
        return crop;
    }

    ///<summary>
    /// returns the crop details for the provided seed item code
    ///</summary>

    public CropDetails GetCropDetails(int seedItemCode)
    {
        //takes in a seed item code and returns the variable 
        return so_CropDetailsList.GetCropDetails(seedItemCode);
    }

    ///<summary>
	///GEt the grid property details for the title at (gridX, gridY) if no grid property exsists null is returned, and is returned
	///and can assume that all grid property detail values are null or false
	/// </summary>

    public GridPropertyDetails GetGridPropertyDetails(int gridX, int gridY)
    {
        //current dictionary, passes this into the other overloaded method
        return GetGridPropertyDetails(gridX, gridY, gridPropertyDictionary);
    }

    ///<summary>
    /// for scene name this method returns a vector2Int with the grid dimensions for the scene , or vector2Int.zero if scene not found
    ///</summary>

//true if scene found and false if not
    public bool GetGridDimensions(SceneName sceneName, out Vector2Int gridDimensions, out Vector2Int gridOrigin)
    {
        //set to zero
        gridDimensions = Vector2Int.zero;
        gridOrigin = Vector2Int.zero;

        //loop through scenes
        //gives scene name, dimmensions, and origin points
        foreach(SO_GridProperties so_GridProperties in so_gridPropertiesArray)
        {
            //make sure its the one we want
            if(so_GridProperties.sceneName == sceneName)
            {
                //populate stuff
                gridDimensions.x = so_GridProperties.gridWidth;
                gridDimensions.y = so_GridProperties.gridHeight;

                gridOrigin.x = so_GridProperties.originX;
                gridOrigin.y = so_GridProperties.originY;

                return true;
            }
        }
        return false;
    }

    public void DisplayPlantedCrop(GridPropertyDetails gridPropertyDetails)
    {
        //see if the seed item code is there
        if(gridPropertyDetails.seedItemCode > -1)
        {
            //get crop details
            //returns the item code and details
            CropDetails cropDetails = so_CropDetailsList.GetCropDetails(gridPropertyDetails.seedItemCode);
//null check just to be safe to make sure theres no error.
            if(cropDetails != null)
            {
                //temp variable
                //prefab to use
                GameObject cropPrefab;

                //instantiate crop prefab at grid locations
                //equates to the number of growth stages the thing has
                int growthStages = cropDetails.growthDays.Length;

                int currentGrowthStage = 0;
                //days counter is the total days
                //int daysCounter = cropDetails.totalGrowthDays;
                //what stage are we currently in based on when the crop was planted
                //loops backwards to see if the current number is greater than or equal to the days counter
                //go backwards to check to see if its bigger, if its not then next stage and when it is then you go to that growth stage
                for(int i = growthStages -1; i >= 0; i--){
                    //if this is true then this is the last growth stage
                    //check growth days for the growth stage instead
                    if(gridPropertyDetails.growthDays >= cropDetails.growthDays[i])
                    {
                        //i is the last growth day, then break out
                        currentGrowthStage = i;
                        break;
                    }
                    //deduct values from the days counter until we get there
                   // daysCounter = daysCounter - cropDetails.growthDays[i];
                }
                //select the appropriate value in the prefab array
                cropPrefab = cropDetails.growthPrefab[currentGrowthStage];

                //gets the sprite
                Sprite growthSprite = cropDetails.growthSprite[currentGrowthStage];
                //gets the location
                Vector3 worldPosition = groundDecoration2.CellToWorld(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0));
                //adjusts the world position in the grid square
                worldPosition = new Vector3(worldPosition.x + Settings.gridCellSize / 2, worldPosition.y, worldPosition.z);
                //save here the cropinstance
                GameObject cropInstance = Instantiate(cropPrefab, worldPosition, Quaternion.identity);
                //get the sprite renderer and set it up
                cropInstance.GetComponentInChildren<SpriteRenderer>().sprite = growthSprite;
                //set the transform to what the parent is
                cropInstance.transform.SetParent(cropParentTransform);
                //set the crop grid position
                cropInstance.GetComponent<Crop>().cropGridPosition = new Vector2Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY);
            }
        }
    }

    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
    }

    public void ISaveableLoad(GameSave gameSave)
    {
        //locate the gameobject save object for this unique id
        //look in the file 
        if(gameSave.gameObjectData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            //if found then save
            GameObjectSave = gameObjectSave;

            //restore data for current scene
            ISaveableRestoreScene(SceneManager.GetActiveScene().name);
            //will trigger a method a bunch of times from the save load manager as it loops through everything registered
        }
    }


    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Add(this);
    }

    public void ISaveableRestoreScene(string sceneName)
    {
        //get scenesave for scene- it exsists because we created it in initialize
        //try to get a value out and then see if youve got something
        if(GameObjectSave.sceneData.TryGetValue(sceneName, out SceneSave sceneSave))
        {
            //get grid property details dictionary - it exsists since we created it in initialise
            if(sceneSave.gridPropertyDetailsDictionary != null)
            {
                gridPropertyDictionary = sceneSave.gridPropertyDetailsDictionary;
            }

            //get dictionary of bools - it exsists since we created it and initialized it
            //if it doesnt equal null and does exist
            if(sceneSave.boolDictionary != null && sceneSave.boolDictionary.TryGetValue("isFirstTimeSceneLoaded", out bool storedIsFirstTimeSceneLoaded))
            {
                isFirstTimeSceneLoaded = storedIsFirstTimeSceneLoaded;
            }

            //Instantiate any crop prefabs initially present in the scene
            //create all the crops from the crop prefab
            if(isFirstTimeSceneLoaded)
            {
                EventHandler.CallInstantiateCropPrefabsEvent();
            }


            //If grid properties exsist
            if (gridPropertyDictionary.Count > 0)
            {
                //grid property details found for the current scene destroy existing gorund decoration
                ClearDisplayGridPropertyDetails();

                //Instantiate grid property details for the current scene
                DisplayGridPropertyDetails();
            }

            //update first time scene loaded bool
            if(isFirstTimeSceneLoaded == true)
            {
                isFirstTimeSceneLoaded = false;
            }
        }
    }
    //save the game for each registered object with the save load manager
    //store the current scene data keyed on the scene data
    
    public GameObjectSave ISaveableSave()
    {
        //store current scene data
        ISaveableStoreScene(SceneManager.GetActiveScene().name);

        return GameObjectSave;
    }

    //saves the grid property dictionary for the current scene
    //in the restore scene method you can load it back in again
    public void ISaveableStoreScene(string sceneName)
    {
        //removes sceneSave for scene
        GameObjectSave.sceneData.Remove(sceneName);

        //create scene save for scene
        SceneSave sceneSave = new SceneSave();

        //create & add dict grid property details dictionary
        sceneSave.gridPropertyDetailsDictionary = gridPropertyDictionary;

        //create & add dict grid property details dictionary
        sceneSave.boolDictionary = new Dictionary<string, bool>();
        //set and log
        sceneSave.boolDictionary.Add("isFirstTimeSceneLoaded", isFirstTimeSceneLoaded);

        //Add scene save to game object scene data
        //scene name is the key and this is a dictionary
        GameObjectSave.sceneData.Add(sceneName, sceneSave);
    }
    ///<summary>
	/// set the grid property details to grid property details for the title at (gridX and gridY) for current scene
	/// </summary>

    public void SetGridPropertyDetails(int gridX, int gridY, GridPropertyDetails gridPropertyDetails)
    {
        //current dictionary instead of adding to the other dictionary
        SetGridPropertyDetails(gridX, gridY, gridPropertyDetails, gridPropertyDictionary);
    }

    ///<summary>
	/// Set the grid property details to gridPropertyDetails for the titel at gridX and gridY for the gridProperties dictionary
	/// </summary>

    public void SetGridPropertyDetails(int gridX, int gridY, GridPropertyDetails gridPropertyDetails, Dictionary<string, GridPropertyDetails> gridPropertyDictionary)
    {
        //construct key from coordinate
        string key = "x" + gridX + "y" + gridY;

        gridPropertyDetails.gridX = gridX;
        gridPropertyDetails.gridY = gridY;

        //set value in the dictionary
        gridPropertyDictionary[key] = gridPropertyDetails;
    }

    //clear from ALL scenes, not just the current one
    private void AdvanceDay(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        //clear display all grid property details
        ClearDisplayGridPropertyDetails();


        //loop through all scenes- by looping through all gridproperties in the array
        //one fo the properties stored in the array is a scene name
        foreach(SO_GridProperties so_GridProperties in so_gridPropertiesArray) 
        {
            //get grid property details dictionary for scene
            //try to get scene data, put it into the scene save and put it in the field
            if(GameObjectSave.sceneData.TryGetValue(so_GridProperties.sceneName.ToString(), out SceneSave sceneSave))
            {
                //do we have a grid properties dictionary?
                if(sceneSave.gridPropertyDetailsDictionary != null)
                {
                    //loops through all grid properties backwards
                    //cant modify in a foreach statement
                    for(int i = sceneSave.gridPropertyDetailsDictionary.Count -1; i >=0; i--)
                    {
                        //uses extention for dictionary class
                        //allows to retrieve an element in the dictionary at a position
                        //return key value pair
                        KeyValuePair<string, GridPropertyDetails> item = sceneSave.gridPropertyDetailsDictionary.ElementAt(i);

                        //change values to reflect the new value put in
                        GridPropertyDetails gridPropertyDetails = item.Value;

                        #region Update all grid properties to reflect the advance in the day
                        //if a crop is planted
                        if(gridPropertyDetails.growthDays > -1)
                        {
                            //if greater than -1 then increase by 1
                            gridPropertyDetails.growthDays += 1;
                        }

                        //if ground is watered, then clear water
                        if (gridPropertyDetails.daysSinceWatered > -1)
                        {
                            gridPropertyDetails.daysSinceWatered = -1;
                        }
                        //set grid property details
                        SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails, sceneSave.gridPropertyDetailsDictionary);

                        #endregion update all grid properties to reflect the advance in day
                    }
                }
            }
        }

        //Display grid property details to reflect the changed values
        DisplayGridPropertyDetails();
    }



}

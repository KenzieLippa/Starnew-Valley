 
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



//require this component with a class that we created before
//this creates the specific guid 
[RequireComponent(typeof(GenerateGUID))]

//lives in the persistent scene, inherits from singleton monobehavior
//impliments the isaveable interface
public class SceneItemsManager : SingletonMonobehaviour<SceneItemsManager>, ISaveable
{
    //specified field to hold parent in the scene
    private Transform parentItem;

    //populate in inspector with the item prefab
    [SerializeField] private GameObject itemPrefab = null;

    private string _iSaveableUniqueID;

    public string ISaveableUniqueID { get { return _iSaveableUniqueID; } set { _iSaveableUniqueID = value; } }

    //stores the actual scene data 
    private GameObjectSave _gameObjectSave;

    public GameObjectSave GameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }

    //find item with the tag
    private void AfterSceneLoad()
    {
        //put them under this tag
        parentItem = GameObject.FindGameObjectWithTag(Tags.ItemsParentTransform).transform;
    }

    protected override void Awake()
    {
        //base awake from the class it derrives from 
        base.Awake();

        //populate with the GUID value from the GUID component
        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;

        //create a new game object save
        GameObjectSave = new GameObjectSave();

    }

    ///<summary>
	/// Destroy items currently in the scene
	/// </summary>

    private void DestroySceneItems()
    {
        //get all items in the scene
        Item[] itemsInScene = GameObject.FindObjectsOfType<Item>();

        //Loop  through all scene items and destroy them
        for(int i = itemsInScene.Length-1; i>-1; i--)
        {
            Destroy(itemsInScene[i].gameObject);
        }
    }

    public void InstantiateSceneItem(int itemCode, Vector3 itemPosition)
    {
        GameObject itemGameObject = Instantiate(itemPrefab, itemPosition, Quaternion.identity, parentItem);
        Item item = itemGameObject.GetComponent<Item>();
        item.Init(itemCode);
    }

    private void InstantiateSceneItems(List<SceneItem> sceneItemList)
    {
        GameObject itemGameObject;
        //loop through them, returns a scene item data type 
        foreach(SceneItem sceneItem in sceneItemList)
        {
            //make sure to get its location
            //parent it to the items game object, rotation to 0
            itemGameObject = Instantiate(itemPrefab, new Vector3(sceneItem.position.x, sceneItem.position.y, sceneItem.position.z), Quaternion.identity, parentItem);
            Item item = itemGameObject.GetComponent<Item>();
            item.ItemCode = sceneItem.itemCode;
            item.name = sceneItem.itemName;
        }
    }
    private void OnDisable()
    {
        ISaveableDeregister();
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoad;
    }


    private void OnEnable()
    {
        //call this method which is a mandatory method from the interface
        ISaveableRegister();
        EventHandler.AfterSceneLoadEvent += AfterSceneLoad;
    }

    public void ISaveableDeregister()
    {
        //opposite of the register
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
    }

    public void ISaveableLoad(GameSave gameSave)
    {
        //will use this method to loop through and then pass in data and look in the dictionary for the item
        if(gameSave.gameObjectData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            //allocate in the game save value
            GameObjectSave = gameObjectSave;

            //restore data for current scene
            ISaveableRestoreScene(SceneManager.GetActiveScene().name);
        }
    }


    public void ISaveableRestoreScene(string sceneName)
    {
        //looks to see if there is a value for the scene
        //tries to see if the dictionary spot exsists and then if theres an item there
        if(GameObjectSave.sceneData.TryGetValue(sceneName, out SceneSave sceneSave))
        {
            //then checks to see if theres a list scene item dictionary
            if(sceneSave.listSceneItemDictionary != null)
            {
                //scene list items found - destroy exsisting items in scene
                DestroySceneItems();

                //now instantiate the list of scene items
                //for all the ones we've just found
                InstantiateSceneItems(sceneSave.listSceneItemDictionary);
            }
        }
    }

    public void ISaveableRegister()
    {
        //add the item to the list
        SaveLoadManager.Instance.iSaveableObjectList.Add(this);
    }

    public GameObjectSave ISaveableSave()
    {
        //store current scene data
        ISaveableStoreScene(SceneManager.GetActiveScene().name);

//add to its own game save data
        return GameObjectSave;
    
    }

    public void ISaveableStoreScene(string sceneName)
    {
        //remove old scene save for a game object if it exsists
        //dictionary keyed by scene name
        //has scene save data types
        //do we have an entry because if we do we should kill it
        GameObjectSave.sceneData.Remove(sceneName);

        //get all items in the scene
        //create a new list of scene items
        List<SceneItem> sceneItemList = new List<SceneItem>();
        //find objects of types item and add it to the array of items
        Item[] itemsInScene = FindObjectsOfType<Item>();


        //loop through all scene items discovered
        foreach(Item item in itemsInScene)
        {
            //create a new scene item for each item found
            SceneItem sceneItem = new SceneItem();
            sceneItem.itemCode = item.ItemCode;
            sceneItem.position = new Vector3Serializable(item.transform.position.x, item.transform.position.y, item.transform.position.z);
            sceneItem.itemName = item.name;

            //add scene item to list after getting all the data
            sceneItemList.Add(sceneItem);
        }

        //Create list scene items in scene save and added to it
        SceneSave sceneSave = new SceneSave();
        //what item is in dictionary is dictated by the key
        //sceneSave.listSceneItemDictionary = new Dictionary<string, List<SceneItem>>();
        //changed from being a dictionary
        sceneSave.listSceneItemDictionary = sceneItemList;

        //add scene save to game object
        GameObjectSave.sceneData.Add(sceneName, sceneSave);
    }
}

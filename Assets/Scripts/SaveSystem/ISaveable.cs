
//contracts, if implimented then needs to use these properties

public interface ISaveable 
{
    //property, cant specify fields, can only specify properties
    string ISaveableUniqueID { get; set; }

    //stores the save data for the game object
    GameObjectSave GameObjectSave { get; set; }

    //registers object with the save load manager
    void ISaveableRegister();

    //deregisters from the save load manager
    void ISaveableDeregister();

//save variables saved by game and keyed by the GUID
    GameObjectSave ISaveableSave();

//game save data object and then see which part applys to the game object
    void ISaveableLoad(GameSave gameSave);

    //need to impliment to store scene data in a game object
    void ISaveableStoreScene(string sceneName);

    void ISaveableRestoreScene(string sceneName);

}

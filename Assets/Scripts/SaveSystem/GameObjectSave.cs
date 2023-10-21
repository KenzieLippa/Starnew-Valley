
using System.Collections.Generic;

[System.Serializable]

public class GameObjectSave 
{
    //string key = scene name
    public Dictionary<string, SceneSave> sceneData;

    //every game object you need to save the scene for you have in the field
    //for each scene you have an entry and a save entry

    public GameObjectSave()
    {
        //creates a new dictionary for the data field
        sceneData = new Dictionary<string, SceneSave>();

    }
    public GameObjectSave(Dictionary<string, SceneSave> sceneData)
    {
        //can pass in the dictionary and then it goes into the save data field
        this.sceneData = sceneData;
    }
}

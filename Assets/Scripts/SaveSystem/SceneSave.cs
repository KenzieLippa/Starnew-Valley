
using System.Collections.Generic;

[System.Serializable]

public class SceneSave 
{
    //store a number of ints in an int dictionary
    public Dictionary<string, int> intDictionary;
    //store booleans 
    public Dictionary<string, bool> boolDictionary; //string key is an identifier name we chose for list
    //string key is an identifier name we chose for the list
    //string as a key and list as a value
    //accumulate all scene item data types and add them to the list
    //create a dictionary item for the list
    //use a key to access and un access it

    //not really nessiscary for a dictionary because theres only one list
   // public Dictionary<string, List<SceneItem>> listSceneItemDictionary;
   //identifier to find
   public Dictionary<string, string> stringDictionary;
   //value is vector 3 string is the key
   public Dictionary<string, Vector3Serializable> vector3Dictionary;
   //save inventory capacity arrays

   public Dictionary<string, int[]> intArrayDictionary;

    public List<SceneItem> listSceneItemDictionary;

    public Dictionary<string, GridPropertyDetails> gridPropertyDetailsDictionary;
    //store inventory items
    public List<InventoryItem>[] listInvItemArray;
}

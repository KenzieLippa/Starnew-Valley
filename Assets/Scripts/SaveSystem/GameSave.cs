
using System.Collections.Generic;

//serialized into a file as part of functionality
[System.Serializable]
public class GameSave 
{
   //string key - GUID gameobject ID
   //member variable as a dictionary, string key and then game object save
   public Dictionary<string, GameObjectSave> gameObjectData;

//constructor method
   public GameSave()
   {
       //new instance in the game object data file
       gameObjectData = new Dictionary<string, GameObjectSave>();
   }
}

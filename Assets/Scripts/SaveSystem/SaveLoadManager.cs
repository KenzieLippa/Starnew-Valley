
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadManager : SingletonMonobehaviour<SaveLoadManager>
{
    //type of ISavable list which uses the interface, allows for a variety of objects in the list
    //all have to use the ISaveable interface
    //holding a list of the isaveable object types
    public GameSave gameSave;
    public List<ISaveable> iSaveableObjectList;

    protected override void Awake()
    {
        //call base class awake 
        base.Awake();

        //new list
        iSaveableObjectList = new List<ISaveable>();
    }

    //called by gui
    //creates a new class
    public void LoadDataFromFile()
    {
        //new class of binary formatter to serialize the data
        BinaryFormatter bf = new BinaryFormatter();
        //check to see if theres already a file saved
        if(File.Exists(Application.persistentDataPath + "/WildHopeCreek.dat"))
        {
            //create new game save
            gameSave = new GameSave();
            //open the file stream where the file system is
            //open file for reading
            //then use the binary formatter and deserialize it for reading
            FileStream file = File.Open(Application.persistentDataPath + "/WildHopeCreek.dat", FileMode.Open);
            //put it in the gameSave object
            gameSave = (GameSave)bf.Deserialize(file);

            //loop through all Isaveable objects and apply save data
            //game objects have to register themselves and add themselves to the ISaveable object list

            for(int i=  iSaveableObjectList.Count -1; i > -1; i--)
            {
                //contains key for the isaveable objects
                //pull out unique id
                if(gameSave.gameObjectData.ContainsKey(iSaveableObjectList[i].ISaveableUniqueID))
                {
                    //execute the load method in the grid properties manager
                    //based on game object property it saves the information and then starts to be restored
                    iSaveableObjectList[i].ISaveableLoad(gameSave);
                }
                //else if isaveable object uique id is not in the game object data then destroy object
                else
                {
                    Component component = (Component)iSaveableObjectList[i];
                    Destroy(component.gameObject);
                }
            }
            //close file
            file.Close();
        }
        //diable the pause menu and disable to return back to the game after loaded
        UIManager.Instance.DisablePauseMenu();
    }

    public void SaveDataToFile()
    {
        //creates new game save
        gameSave = new GameSave();

        //loop through all the ISaveable objects and generate save data
        //trigger the isaveable save method to return a game object of type save
        foreach(ISaveable iSaveableObject in iSaveableObjectList)
        {
            gameSave.gameObjectData.Add(iSaveableObject.ISaveableUniqueID, iSaveableObject.ISaveableSave());
        }
        //open a new file stream
        BinaryFormatter bf = new BinaryFormatter();
        //create a new file or re-write it if it already exsists

        FileStream file = File.Open(Application.persistentDataPath + "/WildHopeCreek.dat", FileMode.Create);
//serialize it in a binary file
        bf.Serialize(file, gameSave);
        //close file
        file.Close();
//disable pause menu
        UIManager.Instance.DisablePauseMenu();
    }

    public void StoreCurrentSceneData()
    {
        //loop through all ISaveable objects and trigger store scene data for each
        //executes the method on the object as specified
        //all game objects added get iterated through and have this method executed
        foreach(ISaveable iSaveableObject in iSaveableObjectList)
        {
            //add itself to the list during the awake method
            iSaveableObject.ISaveableStoreScene(SceneManager.GetActiveScene().name);
        }
    }


    public void RestoreCurrentSceneData()
    {
        //loop through all ISaveable objects and trigger restore scene data for each
        foreach(ISaveable iSaveableObject in iSaveableObjectList)
        {
            //execute the restore scene method using the current scene
            iSaveableObject.ISaveableRestoreScene(SceneManager.GetActiveScene().name);
        }
    }

}
  


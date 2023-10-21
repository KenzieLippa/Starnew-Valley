using System;
using System.Collections.Generic;
using UnityEngine;

//require these two components before operating th script
[RequireComponent(typeof(NPCMovement))]
[RequireComponent(typeof(GenerateGUID))]
//inheret isaveable and all its properties
public class NPC : MonoBehaviour, ISaveable
{
//use to manage save game
//property for id and game object
    private string _ISaveableUniqueID;
    public string ISaveableUniqueID {get{return _ISaveableUniqueID;} set{_ISaveableUniqueID = value;}}
    private GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave {get{return _gameObjectSave;} set{_gameObjectSave = value;}}
//cache npc movement
    private NPCMovement npcMovement;

    private void OnEnable()
    {
        ISaveableRegister();
    }
    private void OnDisable()
    {
        ISaveableDeregister();
    }
    private void Awake()
    {
        //get GUID value and save in id
        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        //create game object save
        GameObjectSave = new GameObjectSave();
    }

    private void Start()
    {
        //get npc movement component
        //get movement and cache here
        npcMovement = GetComponent<NPCMovement>();
    }

    public void ISaveableDeregister()
    {
        //remove refrence from th list
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
    }
    public void ISaveableLoad(GameSave gameSave)
    {
        //get game object save
        //do we have th game data and use th id to retrieve it if we do
        if(gameSave.gameObjectData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            //populate with retrieved value
            GameObjectSave = gameObjectSave;

            //get scene save
            //return whats saved for npc in persistant scene
            if(GameObjectSave.sceneData.TryGetValue(Settings.PersistentScene, out SceneSave sceneSave))
            {
                //if dictionaries are not null
                if(sceneSave.vector3Dictionary != null && sceneSave.stringDictionary != null)
                {
                    //target grid position
                    //try to get value from dictionary
                    //if value exists put in th saved target position
                    if(sceneSave.vector3Dictionary.TryGetValue("npcTargetGridPosition", out Vector3Serializable savedNPCTargetGridPosition))
                    {
                        //update npc target position with tht value and current one
                        npcMovement.npcTargetGridPosition = new Vector3Int((int)savedNPCTargetGridPosition.x, (int)savedNPCTargetGridPosition.y, (int)savedNPCTargetGridPosition.z);
                        npcMovement.npcCurrentGridPosition = npcMovement.npcTargetGridPosition;
                    }

                    //target world position
                    //retrieve th value then update th npc target world position
                    if(sceneSave.vector3Dictionary.TryGetValue("npcTargetWorldPosition", out Vector3Serializable savedNPCTargetWorldPosition))
                    {
                        npcMovement.npcTargetWorldPosition = new Vector3(savedNPCTargetWorldPosition.x, savedNPCTargetWorldPosition.y, savedNPCTargetWorldPosition.z);
                        transform.position = npcMovement.npcTargetWorldPosition;
                    }

                    //target Scene
                    //retrieve th scene from th string dictionary
                    if(sceneSave.stringDictionary.TryGetValue("npcTargetScene", out string savedTargetScene))
                    {
                        //see if an enum tht matches th string
                        if(Enum.TryParse<SceneName>(savedTargetScene, out SceneName sceneName))
                        {
                            //update this
                            npcMovement.npcTargetScene = sceneName;
                            npcMovement.npcCurrentScene = npcMovement.npcTargetScene;
                        }
                    }

                    //clear any current NPC movement
                    npcMovement.CancelNPCMovement();
                }
            }
        }
    }

    public void ISaveableRegister()
    {
        //add to the list
        SaveLoadManager.Instance.iSaveableObjectList.Add(this);
    }

    public void ISaveableRestoreScene(string sceneName)
    {
        //nothing required here because on persistant scene
    }

    public GameObjectSave ISaveableSave()
    {
        //remove curent scene save
        //remove it passing in th persistant scene
        GameObjectSave.sceneData.Remove(Settings.PersistentScene);

        //create scene save
        SceneSave sceneSave = new SceneSave();

        //create vector3 serializable dictionary
        //new instance for position
        sceneSave.vector3Dictionary = new Dictionary<string, Vector3Serializable>();

        //create string dictionary
        //target scene
        sceneSave.stringDictionary = new Dictionary<string, string>();

        //store target grid position, target world position, and target scene
        //key and then new vector3 with xyz values
        sceneSave.vector3Dictionary.Add("npcTargetGridPosition", new Vector3Serializable(npcMovement.npcTargetGridPosition.x, 
        npcMovement.npcTargetGridPosition.y, npcMovement.npcTargetGridPosition.z));
        //need world position too
        sceneSave.vector3Dictionary.Add("npcTargetWorldPosition", new Vector3Serializable(npcMovement.npcTargetWorldPosition.x,
        npcMovement.npcTargetWorldPosition.y, npcMovement.npcTargetWorldPosition.z));
        //add into th string dictionary
        sceneSave.stringDictionary.Add("npcTargetScene", npcMovement.npcTargetScene.ToString());

        //add scene save to game object
        GameObjectSave.sceneData.Add(Settings.PersistentScene, sceneSave);
        //required to be returned
        return GameObjectSave;
    }

    public void ISaveableStoreScene(string sceneName)
    {
        //nothing required here since on th persistant scene
    }

}

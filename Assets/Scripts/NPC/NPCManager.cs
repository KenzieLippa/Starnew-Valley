using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//make sure astar is attached to component
[RequireComponent(typeof(AStar))]

public class NPCManager : SingletonMonobehaviour<NPCManager>
{
    //refrence the scriptable objects
    //populate in inspector with scene route list
    [SerializeField] private SO_SceneRouteList so_SceneRouteList = null;
    //set up dictionary to search for routes to move npc on
    private Dictionary<string, SceneRoute> sceneRouteDictionary;

    [HideInInspector]
    //array of npcs and holds details of all the npcs
    public NPC[] npcArray;

    private AStar aStar;
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();

        //create scene route dictionary
        //key is type string and type sceneRoute
        sceneRouteDictionary = new Dictionary<string, SceneRoute>();

        if(so_SceneRouteList.sceneRouteList.Count > 0)
        {
            //loop through all scene routes in scriptable objects
            foreach(SceneRoute so_sceneRoute in so_SceneRouteList.sceneRouteList)
            {
                //check for duplicates routes in the dictionary
                //key is the from scene and the to string next to each other
                if(sceneRouteDictionary.ContainsKey(so_sceneRoute.fromSceneName.ToString() + so_sceneRoute.toSceneName.ToString()))
                {
                    Debug.Log("** Duplicate scene route key found** check for duplicate routes in the scriptable objects scene route list");
                    continue;
                    
                }
                //add route to dictionary
                //actual value is what has been retrieved
                sceneRouteDictionary.Add(so_sceneRoute.fromSceneName.ToString() + so_sceneRoute.toSceneName.ToString(), so_sceneRoute);
            }
        }

        aStar = GetComponent<AStar>();

        //Get NPC Gameobjects in scene
        npcArray = FindObjectsOfType<NPC>();

    }
    private void OnEnable()
    {
        //link after scene load event 
        EventHandler.AfterSceneLoadEvent +=  AfterSceneLoad;
    }
    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoad;
    }
    private void AfterSceneLoad()
    {
        //call this method once loaded for the npcs active status
        SetNPCsActiveStatus();
    }
   private void SetNPCsActiveStatus()
   {
       foreach(NPC npc in npcArray)
       {
           //retrieve the movement component 
           NPCMovement npcMovement = npc.GetComponent<NPCMovement>();
           //compare the current scene of the npc to the players current scene
           if(npcMovement.npcCurrentScene.ToString() == SceneManager.GetActiveScene().name)
           {
               //if supposed to be scene set active
               npcMovement.SetNPCActiveInScene();
           }
           else
           {
               //if not supposed to be there than set inactive
               npcMovement.SetNPCInactiveInScene();
           }
       }
   }
   public SceneRoute GetSceneRoute(string fromSceneName, string toSceneName)
   {
       //local scene route variable
       SceneRoute sceneRoute;

       //get scene route from dictionarys
       //try get is used to look for a scene route that has those two scene names concocted
       if(sceneRouteDictionary.TryGetValue(fromSceneName + toSceneName, out sceneRoute))
       {
           //if there is then you can return it
           return sceneRoute;
       }
       else
       {
           //if not return nothing
           return null;
       }
   }

    //npc movment step stack and grid position we want to move to
   public bool BuildPath(SceneName sceneName, Vector2Int startGridPosition, Vector2Int endGridPosition, Stack<NPCMovementStep> npcMovementStepStack)
   {
       //calls the build path method and passes in all the info
       //update the movement step stack 
       if(aStar.BuildPath(sceneName, startGridPosition, endGridPosition, npcMovementStepStack))
       {
           return true;
       }
       else
       {
           return false;
       }
   }
}

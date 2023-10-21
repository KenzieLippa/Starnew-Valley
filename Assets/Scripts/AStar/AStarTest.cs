/*using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

//need astar component in order to work
[RequireComponent(typeof(AStar))]

public class AStarTest : MonoBehaviour
{
    //refrence the astar script
    //populate in inspector
    private AStar aStar;
    [SerializeField] private Vector2Int startPosition;
    [SerializeField] private Vector2Int finishPosition;
    //temp tile map to display tile
    [SerializeField] private Tilemap tileMapToDisplayPathOn = null;
    //use the semi transparent tile
    [SerializeField] private TileBase tileToUseToDisplayPath = null;
    [SerializeField] private bool displayStartAndFinish = false;
    //do we want to display the path
    [SerializeField] private bool displayPath = false;

//for population
    private Stack<NPCMovementStep> npcMovementSteps;

    private void Awake()
    {
        //call when building the path
        aStar = GetComponent<AStar>();
        npcMovementSteps = new Stack<NPCMovementStep>();

    }

    
  

    // Update is called once per frame
    void Update()
    {
        //start position set and a finish position set and check that the tile map is set within th inspector

        if(startPosition != null && finishPosition != null && tileMapToDisplayPathOn != null && tileToUseToDisplayPath != null)
        {
            //display start and finish tiles
            if(displayStartAndFinish)
            {
                //display start tile
                //takes in a vector 3 int and the start position and displays the tile
                tileMapToDisplayPathOn.SetTile(new Vector3Int(startPosition.x, startPosition.y, 0), tileToUseToDisplayPath);

                //display finish tile
                tileMapToDisplayPathOn.SetTile(new Vector3Int(finishPosition.x, finishPosition.y, 0), tileToUseToDisplayPath);
            }
            else
            //clear start and finish 
            {
                //clear start tile
                tileMapToDisplayPathOn.SetTile(new Vector3Int(startPosition.x, startPosition.y, 0), null);

                //clear finish tile
                tileMapToDisplayPathOn.SetTile(new Vector3Int(finishPosition.x, finishPosition.y, 0), null);
            }

            //display path
            //calls astar algorythm
            if(displayPath)
            {
                //get current scene name
                //convert scene name into enum
                Enum.TryParse<SceneName>(SceneManager.GetActiveScene().name, out SceneName sceneName);

                //build path
                aStar.BuildPath(sceneName, startPosition, finishPosition, npcMovementSteps);

                //display path on tilemap
                //loop through movement steps
                foreach (NPCMovementStep npcMovementStep in npcMovementSteps)
                {
                    //set the tile based on the held position
                    tileMapToDisplayPathOn.SetTile(new Vector3Int(npcMovementStep.gridCoordinate.x, npcMovementStep.gridCoordinate.y, 0), tileToUseToDisplayPath);
                }
            }
            else
            {
                //clear path
                if(npcMovementSteps.Count > 0)
                {
                    //clear path on tilemap
                    foreach(NPCMovementStep npcMovementStep in npcMovementSteps)
                    {
                        tileMapToDisplayPathOn.SetTile(new Vector3Int(npcMovementStep.gridCoordinate.x, npcMovementStep.gridCoordinate.y, 0), null);
                    }
                    //clezr movement step
                    npcMovementSteps.Clear();
                }
            }
        }
    }
}
*/
using UnityEngine;

public class AStarTest : MonoBehaviour 
{
    // private AStar aStar

//npc path component for npc want to move
    [SerializeField] private NPCPath npcPath = null;
    //tick to trigger moving of npc
    [SerializeField] private bool moveNPC = false;
    [SerializeField] private SceneName sceneName = SceneName.Scene1_Farm;
    //where we want the npc to go
    [SerializeField] private Vector2Int finishPosition;
    //specify this clip
    [SerializeField] private AnimationClip idleDownAnimationClip = null;
    [SerializeField] private AnimationClip eventAnimationClip = null;

    private NPCMovement npcMovement;

    private void Start()
    {
        //cache and use game object to get component
        npcMovement = npcPath.GetComponent<NPCMovement>();
        //set facing direction
        npcMovement.npcFacingDirectionAtDestination = Direction.down;
        npcMovement.npcTargetAnimationClip = idleDownAnimationClip;
    }

    private void Update()
    {
        //wait till triggered
        if(moveNPC)
        {
            moveNPC = false;

//set a fake schedule event
            NPCScheduleEvent npcScheduleEvent = new NPCScheduleEvent(0, 0, 0, 0, Weather.none, Season.none, sceneName, new GridCoordinate(finishPosition.x, 
            finishPosition.y), eventAnimationClip);

//building path
            npcPath.BuildPath(npcScheduleEvent);
        }
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NPCMovement))]
public class NPCPath : MonoBehaviour
{
    //central to path building in astar and then back in again to the npc path
    public Stack<NPCMovementStep> npcMovementStepStack;

    private NPCMovement npcMovement;

    private void Awake()
    {
        npcMovement = GetComponent<NPCMovement>();
        //create a new stack
        //first in last out
        //tells where an NPC is supposed to be
        npcMovementStepStack = new Stack<NPCMovementStep>();
    }

    public void ClearPath()
    {
        //uses the clear method to empty the stack
        npcMovementStepStack.Clear();
    }

//when trigger the path to be built we trigger from the build path event 
    public void BuildPath(NPCScheduleEvent npcScheduleEvent)
    {
        ClearPath();

        //if schedule event is for the same scene as the current NPC scene
        if(npcScheduleEvent.toSceneName == npcMovement.npcCurrentScene)
        {
            //get current and target grid positions 
            Vector2Int npcCurrentGridPosition = (Vector2Int)npcMovement.npcCurrentGridPosition;

            Vector2Int npcTargetGridPosition = (Vector2Int)npcScheduleEvent.toGridCoordinate;

            //build path and add movement steps to movement step stack
            NPCManager.Instance.BuildPath(npcScheduleEvent.toSceneName, npcCurrentGridPosition, npcTargetGridPosition, npcMovementStepStack);

            //if stack count >1, update times and then pop off 1st item which is starting position
         
        }

        //else if the schedule event is for a location in a different scene
        else if(npcScheduleEvent.toSceneName != npcMovement.npcCurrentScene)
        {
            SceneRoute sceneRoute;

            //get scene route matching schedule
            //try to retrieve a scene route from current scene to target scene
            sceneRoute = NPCManager.Instance.GetSceneRoute(npcMovement.npcCurrentScene.ToString(), npcScheduleEvent.toSceneName.ToString());

            //has a valid scene route been found
            //if route exsists 
            if(sceneRoute != null)
            {
                //loop through scene paths in reverse order
                //start at destination and work back to the start
                for(int i = sceneRoute.scenePathList.Count -1; i >= 0; i--)
                {
                    int toGridX, toGridY, fromGridX, fromGridY;
                    //current scene path
                    ScenePath scenePath = sceneRoute.scenePathList[i];

                    //check if final destination
                    if(scenePath.toGridCell.x >= Settings.maxGridWidth || scenePath.toGridCell.y >= Settings.maxGridHeight)
                    {
                        //if so, use final destination grid cell
                        toGridX = npcScheduleEvent.toGridCoordinate.x;
                        toGridY = npcScheduleEvent.toGridCoordinate.y;
                    }
                    else
                    {
                        //else use scene path to position
                        toGridX = scenePath.toGridCell.x;
                        toGridY = scenePath.toGridCell.y;
                    }
                    //check if this is the starting position
                    if(scenePath.fromGridCell.x >= Settings.maxGridWidth || scenePath.fromGridCell.y >= Settings.maxGridHeight)
                    {
                        //if so use npc position
                        fromGridX = npcMovement.npcCurrentGridPosition.x;
                        fromGridY = npcMovement.npcCurrentGridPosition.y;
                    }
                    else
                    {
                        // else use scene path from position
                        fromGridX = scenePath.fromGridCell.x;
                        fromGridY = scenePath.fromGridCell.y;
                    }
                    //depending on where we are here are the two positions in the scene that the npc is in
                    Vector2Int fromGridPosition = new Vector2Int(fromGridX, fromGridY);

                    Vector2Int toGridPosition = new Vector2Int(toGridX, toGridY);

                    //build path and add movement steps to movement step stack
                    //keep going till no more paths
                    NPCManager.Instance.BuildPath(scenePath.sceneName, fromGridPosition, toGridPosition, npcMovementStepStack);
                }
            }
        }
        if(npcMovementStepStack.Count > 1)
        {
            UpdateTimesOnPath();
            npcMovementStepStack.Pop();//discard starting step

            //set schedule event details in NPC movement
            npcMovement.SetScheduleEventDetails(npcScheduleEvent);
        }
    }

    ///<summary>
    /// update the path movement steps with expected gametime
    ///</summary>

    public void UpdateTimesOnPath()
    {
        //get current game time
        TimeSpan currentGameTime = TimeManager.Instance.GetGameTime();

        NPCMovementStep previousNPCMovementStep = null;

//loop through all movement steps in the stack 
        foreach(NPCMovementStep npcMovementStep in npcMovementStepStack)
        {
            if(previousNPCMovementStep == null)
                previousNPCMovementStep = npcMovementStep;

            npcMovementStep.hour = currentGameTime.Hours;
            npcMovementStep.minute = currentGameTime.Minutes;
            npcMovementStep.second = currentGameTime.Seconds;

            TimeSpan movementTimeStep;

//test to see if they are moving diagnoly
            //if diagnol
            if(MovementIsDiagonal(npcMovementStep, previousNPCMovementStep))
            {
                //use distance and speed to work out the speed of when theyll get there
                movementTimeStep = new TimeSpan(0, 0, (int)(Settings.gridCellDiagonalSize / Settings.secondsPerGameSecond / npcMovement.npcNormalSpeed));
            }
            else
            {
                movementTimeStep = new TimeSpan(0, 0, (int)(Settings.gridCellSize / Settings.secondsPerGameSecond / npcMovement.npcNormalSpeed));
            }

//add movement time to the current game time retrieved
            currentGameTime = currentGameTime.Add(movementTimeStep);

            //set previous to be current
            previousNPCMovementStep = npcMovementStep;

        }
    }

    ///<summary>
    /// returns true if the previous movement step is diagonal to movement step, else returns false
    ///</summary>
    //see if its diagnol
    private bool MovementIsDiagonal(NPCMovementStep npcMovementStep, NPCMovementStep previousNPCMovementStep)
    {
        if((npcMovementStep.gridCoordinate.x != previousNPCMovementStep.gridCoordinate.x) && (npcMovementStep.gridCoordinate.y != previousNPCMovementStep.gridCoordinate.y))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}

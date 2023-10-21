using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour
{
    //break up fields and space them out so we can see whats happening
   [Header("Tiles & Tilemap References")]
   [Header("Options")]
   //should movement penalties be observed or not?
   [SerializeField] private bool observeMovementPenalties = true;

   //encourage npcs to walk along paths
   //if there are priority paths then you can put a value on default spaces so other spaces have a penalty so the algorythm favors the paths
   [Range(0, 20)]
   [SerializeField] private int pathMovementPenalty = 0;
   [Range(0, 20)]
   [SerializeField] private int defaultMovementPenalty = 0;

//nodes from gridnodes class
   private GridNodes gridNodes;
   //two nodes to find
   private Node startNode;
   private Node targetNode;
   private int gridWidth;
   private int gridHeight;
   private int originX;
   private int originY;

//open list and closed list
//hashed set locates contents with a hash, allows to use contained methods much quicker
//cant sort a hash set that easily
   private List<Node> openNodeList;
   private HashSet<Node> closedNodeList;

//when a path is found
   private bool pathFound = false;

   ///<summary>
   ///builds a path for the given scene name, from the start grid position to the end grid position, and adds movement steps to the passed in npcmovementstack .
   ///also returns true if path found or false if no path found
   ///</summary>

//builds a path for the given scene name from position 1 to position 2
//adds a step to the movement step stack
//npc manager will build a path using the scene we want the path built for
//stack of movements, add and pull off the stack
   public bool BuildPath(SceneName sceneName, Vector2Int startGridPosition, Vector2Int endGridPosition, Stack<NPCMovementStep> npcMovementStepStack)
   {
       //make sure path found is false
       pathFound = false;
       //utilizes the grid properties tile maps
       if(PopulateGridNodesFromGridPropertiesDictionary(sceneName, startGridPosition, endGridPosition))
       {
           //true or false if path found
           if(FindShortestPath())
           {
               UpdatePathOnNPCMovementStepStack(sceneName, npcMovementStepStack);

               return true;
           }
       }
       return false;
   }

//pass in scene name and step stack
   private void UpdatePathOnNPCMovementStepStack(SceneName sceneName, Stack<NPCMovementStep> npcMovementStepStack)
   {
       Node nextNode = targetNode;

//work back from the target to the start
       while(nextNode != null)
       {
           NPCMovementStep npcMovementStep = new NPCMovementStep();

           npcMovementStep.sceneName = sceneName;
           npcMovementStep.gridCoordinate = new Vector2Int(nextNode.gridPosition.x + originX, nextNode.gridPosition.y + originY);

//push it onto the stack
//last thing you push on is the first thing you pull off
           npcMovementStepStack.Push(npcMovementStep);

           nextNode = nextNode.parentNode;
       }
   }

   ///<summary>
   /// returns true if the path has been found
   ///</summary>
   private bool FindShortestPath()
   {
       //add start node to open list
       openNodeList.Add(startNode);

       //loop through open node list until empty
       while(openNodeList.Count > 0)
       {
           //sort list 
           //can just use because we already have the icomparible
           //lowest fcost will be at the top
           openNodeList.Sort();

           // current node = the node in the open list with the lowest fCost
           Node currentNode = openNodeList[0];
           openNodeList.RemoveAt(0);

           //add current node to the closed list
           closedNodeList.Add(currentNode);

           //if current node = target node
           // then finish
           if(currentNode == targetNode)
           {
               pathFound = true;
               //stop the while loop (shwartz wld be so mad)
               break;
           }
           
           //evaluate fcost for each neighbor of the current node
           EvaluateCurrentNodeNeighbours(currentNode);
       }

       if(pathFound)
       {
           return true;
       }
       else
       {
           return false;
       }
   }
   //pass in the current node 
   private void EvaluateCurrentNodeNeighbours(Node currentNode)
   {
       //set to grid position to the current node
       Vector2Int currentNodeGridPosition = currentNode.gridPosition;

       Node validNeighbourNode;

       //loop through all directions
       //has 8 possible neighbor nodes in all directions
       for(int i = -1; i<= 1; i++)
       {
           for(int j = -1; j <= 1; j++)
           {
               if(i == 0 && j == 0)
               {
                   //current node continue
                   continue;
               }
               //get valid node neighbor
               validNeighbourNode = GetValidNodeNeighbour(currentNodeGridPosition.x + i, currentNodeGridPosition.y +j);
//if valid
               if(validNeighbourNode != null)
               {
                   //calculate new gcost for neighbor
                   int newCostToNeighbour;

//set or unset to see if we observe movement penalties
                   if(observeMovementPenalties)
                   {
                       //current gcost is updated with the new distance and move penalty
                       newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, validNeighbourNode) + validNeighbourNode.movementPenalty;
                   }
                   else
                   {
                       newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, validNeighbourNode);
                   }
                   //see if valid node is in the open list
                   bool isValidNeighbourNodeInOpenList = openNodeList.Contains(validNeighbourNode);

//update to be the new cost of the neighbor
                   if(newCostToNeighbour < validNeighbourNode.gCost || !isValidNeighbourNodeInOpenList)
                   {
                       validNeighbourNode.gCost = newCostToNeighbour;
                       validNeighbourNode.hCost = GetDistance(validNeighbourNode, targetNode);
//set the parent to the current
                       validNeighbourNode.parentNode = currentNode;

//if isnt in the list, add it
                       if(!isValidNeighbourNodeInOpenList)
                       {
                           openNodeList.Add(validNeighbourNode);
                       }
                   }
               }
           }
       }
   }
   private int GetDistance(Node nodeA, Node nodeB)
   {
       //working out the distance between the two nodes
       int dstX = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);
       int dstY = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);

       if(dstX > dstY)
       {
           return 14 * dstY + 10 * (dstX - dstY);
       }
       return 14 * dstX + 10 * (dstY - dstX);
   }

//take in y and x, see if beyond the grid
    private Node GetValidNodeNeighbour(int neighbourNodeXPosition, int neighbourNodeYPosition)
    {
        //if neighbor node position is beyond grid then return null
        if(neighbourNodeXPosition >= gridWidth || neighbourNodeXPosition < 0 || neighbourNodeYPosition >= gridHeight || neighbourNodeYPosition < 0)
        {
            return null;
        }

        //if neighbor is an obstacle or neighbor is in the closed list then skip
        Node neighbourNode = gridNodes.GetGridNode(neighbourNodeXPosition, neighbourNodeYPosition);

//see if the neighbor is in closed list with hash list
        if(neighbourNode.isObstacle || closedNodeList.Contains(neighbourNode))
        {
            return null;
        }
        else
        {
            return neighbourNode;
        }
    }

    private bool PopulateGridNodesFromGridPropertiesDictionary(SceneName sceneName, Vector2Int startGridPosition, Vector2Int endGridPosition)
    {
        //get grid properties dictionarys for the scene
        //have a grid properties manager object that has room for paths obstacles ect
        //prevent the npc from walking through things
        //populating boolean values here
        SceneSave sceneSave;

//try to get scene data and return a scene save into scene save
        if (GridPropertiesManager.Instance.GameObjectSave.sceneData.TryGetValue(sceneName.ToString(), out sceneSave))
        {
            //get dict grid property details
            //all grid info about squares
            if(sceneSave.gridPropertyDetailsDictionary != null)
            {
                //Vector2Int gridDemensions = new Vector2Int();
                //Vector2Int gridOrigin = new Vector2Int();

                //get grid height and width
                //get dimmensions for passed in scene
                if(GridPropertiesManager.Instance.GetGridDimensions(sceneName, out Vector2Int gridDimensions, out Vector2Int gridOrigin))
                {
                    //create nodes based on grid properties dicitonary
                    //set all with values returned
                    gridNodes = new GridNodes(gridDimensions.x, gridDimensions.y);
                    gridWidth = gridDimensions.x;
                    gridHeight = gridDimensions.y;
                    originX = gridOrigin.x;
                    originY = gridOrigin.y;

                    //create open node list
                    openNodeList = new List<Node>();

                    //create close node list
                    closedNodeList = new HashSet<Node>();
                }
                else
                {
                    return false;
                }

                //populate start node
                //get from 2d array, adjust it by the grid origin, we want 0,0 to be bottom left but its actually in the middle so we need to adjust it
                startNode = gridNodes.GetGridNode(startGridPosition.x - gridOrigin.x, startGridPosition.y - gridOrigin.y);
                //populate grid node
                targetNode = gridNodes.GetGridNode(endGridPosition.x - gridOrigin.x, endGridPosition.y - gridOrigin.y);


                //populate obstacle and path info for grid
                //go through all nodes in all positions and check for obstacles and movement penaltys
                for(int x = 0; x < gridDimensions.x; x++)
                {
                    for(int y = 0; y < gridDimensions.y; y++)
                    {
                        GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(x + gridOrigin.x, y + gridOrigin.y, 
                        sceneSave.gridPropertyDetailsDictionary);
                        //if found something
                        if(gridPropertyDetails != null)
                        {
                            //if npc obstacle
                            if(gridPropertyDetails.isNPCObstacle == true)
                            {
                                Node node = gridNodes.GetGridNode(x, y);
                                node.isObstacle = true;
                            }
                            else if (gridPropertyDetails.isPath == true)
                            {
                                Node node = gridNodes.GetGridNode(x, y);
                                node.movementPenalty = pathMovementPenalty;
                            }
                            else
                            {
                                Node node = gridNodes.GetGridNode(x, y);
                                node.movementPenalty = defaultMovementPenalty;
                            }
                        }
                    }
                }

            }
            else 
            {
                return false;
            }
        }
        else
        {
            return false;
        }
        return true;
    }

}

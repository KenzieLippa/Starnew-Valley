using System;
using UnityEngine;

public class Node : IComparable<Node>
{
    //stored in a list and the needs this in order to sort
    //1 node per square on tilemap
    //have all node details and penalty
    //holds parent nodes as well
   
   //position that the node is on the grid
   public Vector2Int gridPosition;
   public int gCost = 0; // distance from starting node
   public int hCost = 0; // distance from finishing node
   //if there is an obstacle that the algorythm cant transverse then it will be set to true
   public bool isObstacle = false;
   //when doing paths we can use this to prevent the algorythm to have penalty to encourage the algorythm to use the paths
   public int movementPenalty;
   //holds the parent node to this node
   //root evaluated through
   public Node parentNode;

   public Node(Vector2Int gridPosition)
   {
       //set to the passed in value
       // this is a null constructor
       this.gridPosition = gridPosition;
       parentNode = null;
   }
   //getter property to return fcost
   public int FCost
   {
       get
       {
           //calcs and returns f cost
           return gCost + hCost;
       }
   }

//using a sort method so we need to adapt to allow for this to work
//returns -1,0,1 if is greater than equal to or less than
   public int CompareTo(Node nodeToCompare)
   {
       //compare will be <0 if this instance fcost is less than nodeToCompare.fcost
       //compare will be >0 if this instance fcost is greater than nodeToCompare.fcost
       //compare will be ==0 if the values are the same

       //generate the correct node to compare fcost
       int compare = FCost.CompareTo(nodeToCompare.FCost);
       if(compare == 0)
       {
           //compare hcost as a tie breaker
           compare = hCost.CompareTo(nodeToCompare.hCost);
       }
       return compare;
   }
 
}

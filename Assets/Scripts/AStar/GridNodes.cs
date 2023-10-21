using UnityEngine;

public class GridNodes 
{
    //member variables
   private int width;
   private int height;

//2d array of nodes for tile map for path finding
   private Node[,] gridNode;

//have constructor because not from monobehaviour
   public GridNodes(int width, int height)
   {
       this.width = width;
       this.height = height;

//initialise the grid node
       gridNode = new Node[width, height];

//iterates from 0 to width-1 and same for y but with height -1
       for(int x = 0; x < width; x++)
       {
           for(int y = 0; y < height; y++)
           {
               //make a new node with the constructor
               gridNode[x, y] = new Node(new Vector2Int(x, y));
           }
       }
   }

//returns the node at grid position x,y that was passed in
//check to see if within range and then return node at that position
   public Node GetGridNode(int xPosition, int yPosition)
   {
       if (xPosition < width && yPosition < height)
       {
           return gridNode[xPosition, yPosition];
       }
       else
       {
           Debug.Log("Requested grid node is out of range");
           return null;
       }
   }
}

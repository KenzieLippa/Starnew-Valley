
using System.Collections.Generic;
using UnityEngine;

//create a menu in editor to create objects of this type
//menu path
[CreateAssetMenu(fileName = "so_GridProperties", menuName = "Scriptable Objects/Grid Properties")]

//inherits from scriptable object
public class SO_GridProperties : ScriptableObject
{
    //member objects
    //for every scene we will have one of these to contain all properties in a list of values being painted on the tile map

    //what scene this object is for
    public SceneName sceneName;

    //how large the grid is
    //tilemaps can take varying sizes and there are many different boolean tilemaps
    //maximum bounds
    public int gridWidth;
    public int gridHeight;

    
    //capture the origin of the maximum bounds
    public int originX;
    public int originY;

    //for every grid property on the tile map, read in and add to list
    [SerializeField]
    public List<GridProperty> gridPropertyList;
}

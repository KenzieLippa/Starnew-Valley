using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

//run in the editor once its placed in the scene
[ExecuteAlways]

public class TilemapGridProperties : MonoBehaviour
{ 
    #if UNITY_EDITOR

//only in the unity editor
    private Tilemap tilemap;

    //populate in the editor
    //drag in the scriptable object asset relating to this scene
    [SerializeField] private SO_GridProperties gridProperties = null;
    //use that to indicate which is the property being calculated
    [SerializeField] private GridBoolProperty gridBoolProperty = GridBoolProperty.diggable;


    private void OnEnable()
    {
        //only populate in the editor
        //passes in the gameobject and only run the code if the object isnt running at the time
        if (!Application.IsPlaying(gameObject))
        {
            //populate with the component tilemap when the object is enabled
            //cache the value of the tilemap compoentn and cache the value
            tilemap = GetComponent<Tilemap>();

            //is the SO passed in, if so then clear it
            if(gridProperties != null)
            {
                //enable the object before painting, once enabled it enables all the bool tyle maps
                //says to clear out everything currently in the list, after painting it then disable it
                gridProperties.gridPropertyList.Clear();
            }
        }
    }


    //look through the current map and remeber where all the things are painted
    private void OnDisable()
    {
        //only populate in the editor
        if (!Application.IsPlaying(gameObject))
        {

            UpdateGridProperties();

            if(gridProperties != null)
            {
                //this is required to ensure that the updated gridproperties game object gets saved when the game is saved - otherwise they are not saved
                //only works in editor, not in build
                EditorUtility.SetDirty(gridProperties);
            }
        }
    }

    private void UpdateGridProperties()
    {
        //compress tilemap bounds
        //if you paint outside and then delete the square then the bounds aren't auto reset
        //this checks to see where the tiles currently are and then leaves only those in the tilemap bounds
        tilemap.CompressBounds();

        //only populate in the editor
        if (!Application.IsPlaying(gameObject))
        {
            if(gridProperties != null)
            {
                //bounds of the tile map that the user is currently are
                //bottom left
                Vector3Int startCell = tilemap.cellBounds.min;
                //top right
                Vector3Int endCell = tilemap.cellBounds.max;

                //all th x values
                for(int x = startCell.x; x<endCell.x; x++)
                {
                    //all the y values. like that square you made a while ago
                    for(int y = startCell.y; y<endCell.y; y++)
                    {
                        //get tile method gets the tile at the specified position
                        TileBase tile = tilemap.GetTile(new Vector3Int(x, y, 0));

                        //dont change default if already false, if there is a tile then add a value to the property list
                        //gridproperty is the class we made, makes a new coord and whether its true or not
                        if(tile != null)
                        {
                            gridProperties.gridPropertyList.Add(new GridProperty(new GridCoordinate(x, y), gridBoolProperty, true));
                        }
                    }
                }
            }
        }
    }
    private void Update()
    {
        //only populate in the editor
        //a warning to make sure theyre disabled
        if (!Application.IsPlaying(gameObject))
        {
            Debug.Log("DISABLE PROPERTY TILEMAPS");
        }
    }
    #endif
    
}

[System.Serializable]

public class GridProperty 
{
    //3 member variables

    //class
    public GridCoordinate gridCoordinate;
    //enum
    public GridBoolProperty gridBoolProperty;
    //regular boolean for the value taken off the tilemap
    public bool gridBoolValue = false;

    //constructor
    public GridProperty(GridCoordinate gridCoordinate, GridBoolProperty gridBoolProperty, bool gridBoolValue)
    {
        this.gridCoordinate = gridCoordinate;
        this.gridBoolProperty = gridBoolProperty;
        this.gridBoolValue = gridBoolValue;
    }
 //paint to indicate the yes value, use the script and it will capture all the variables like magic or something
}

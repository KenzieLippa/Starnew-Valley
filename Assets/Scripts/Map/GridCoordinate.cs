using UnityEngine;

//can be saved to file as part of the save game functionality
[System.Serializable]

public class GridCoordinate
{
    //coords on tilemap
    public int x;
    public int y;

    //constructor to set x and y
    public GridCoordinate(int p1, int p2)
    {
        x = p1;
        y = p2;
    }

    //allow you to do explicit type conversion from coordinate type to vector types
    //type you want to convert to and the thing you want to convert
    public static explicit operator Vector2(GridCoordinate gridCoordinate)
    {
        //takes in floats so you have to cast them in as floats and then it will cast it in as a float
        //set a vector2 to be a coordinate and unity will now do this conversion
        return new Vector2((float)gridCoordinate.x, (float)gridCoordinate.y);
    }

    public static explicit operator Vector2Int(GridCoordinate gridCoordinate)
    {
        return new Vector2Int(gridCoordinate.x, gridCoordinate.y);
    }

    public static explicit operator Vector3(GridCoordinate gridCoordinate)
    {
        return new Vector3((float)gridCoordinate.x, (float)gridCoordinate.y, 0f);
    }

    public static explicit operator Vector3Int(GridCoordinate gridCoordinate)
    {
        return new Vector3Int(gridCoordinate.x, gridCoordinate.y,0);
    }
}

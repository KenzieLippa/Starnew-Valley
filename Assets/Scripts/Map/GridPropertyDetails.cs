[System.Serializable]

public class GridPropertyDetails 
{
    //holds info for every scene on the tile map
    public int gridX;
    public int gridY;

    //store values that you capture on the tile maps
    public bool isDiggable = false;
    public bool canDropItem = false;
    public bool canPlaceFurniture = false;
    public bool isPath = false;
    public bool isNPCObstacle = false;

    //info captured during gameplay
    public int daysSinceDug = -1;

    //keep track of when it was last watered
    public int daysSinceWatered = -1;
    //seed item code for the planted seed
    public int seedItemCode = -1;
    //how many days the seeds been planted for
    public int growthDays = -1;

    //how many days since the last harvest for multiple harvests

    public int daysSinceLastHarvest = -1;


    public GridPropertyDetails() {



    }

}

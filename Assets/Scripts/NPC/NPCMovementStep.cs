using UnityEngine;

public class NPCMovementStep 
{
    //for every node of the path we hold that in this class
    //holds scene name, to know where the move step is
    //allow the npc to know what time it should be at each step
    public SceneName sceneName;
    public int hour;
    public int minute;
    public int second;
    public Vector2Int gridCoordinate;

}

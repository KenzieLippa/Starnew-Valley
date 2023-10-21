//ohno
//all animation states possible
public enum AnimationName
{
    idleDown,
    idleUp,
    idleRight,
    idleLeft,
    walkUp,
    walkDown,
    walkRight,
    walkLeft,
    runUp,
    runDown,
    runRight,
    runLeft,
    useToolUp,
    useToolDown,
    useToolRight,
    useToolLeft,
    swingToolUp,
    swingToolDown,
    swingToolRight,
    swingToolLeft,
    liftToolUp,
    liftToolDown,
    liftToolRight,
    liftToolLeft,
    holdToolUp,
    holdToolDown,
    holdToolRight,
    holdToolLeft,
    pickDown,
    pickUp,
    pickRight,
    pickLeft,
    count
}

//the part being animated
public enum CharacterPartAnimator
{
    body,
    arms,
    hair,
    tool,
    hat,
    count
}
public enum PartVariantColor
{
    none,
    count
}
public enum PartVariantType
{
    none,
    carry,
    hoe,
    pickaxe,
    axe,
    scythe,
    wateringCan,
    count
}

//identify which bool property each graph relates to
public enum GridBoolProperty
{
    diggable,
    canDropItem,
    canPlaceFurniture,
    isPath,
    isNPCObstacle
}

//inventory locations (as game develops further you may want to put things in other places)
public enum InventoryLocation
{
    player,
    chest,
    count
}
//use count to figure out what you have
//text representation of underlying int

//for scenes
public enum SceneName
{
    Scene1_Farm,
    Scene2_Field,
    Scene3_Cabin
}

//for seasons
public enum Season
{
    Spring,
    Summer,
    Autumn,
    Winter,
    none,
    count
}


public enum ToolEffect
{
    none,
    watering
}
// an enumeration, defines a number of specific values

public enum HarvestActionEffect
{
    deciduousLeavesFalling,
    pineConesFalling,
    choppingTreeTrunk,
    breakingStone,
    reaping,
    none
}
public enum Weather
{
    dry,
    raining,
    snowing,
    none,
    count
}

public enum Direction
{
    up,
    down,
    left,
    right,
    none
}
public enum ItemType
{
    Seed,
    Commodity,
    //wood etc
    Watering_tool,
    Hoeing_tool,
    Chopping_tool,
    Breaking_tool,
    Reaping_tool,
    Collecting_tool,
    Reapable_scenary,
    Furniture,
    none,
    count
        //gives number in here
}

public enum Facing
{
    none, 
    front,
    back,
    right

}

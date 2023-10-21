
using UnityEngine;
//dont need to be instanciated but can just be refrenced

public static class Settings 
{

    //scenes
    public const string PersistentScene = "PersistentScene";
    //creating integer fields
    //use this to set hash values for the parameters


    //fade in and fade out seconds
    //and the target alpha
    public const float fadeInSeconds = 0.25f;
    public const float fadeOutSeconds = 0.35f;
    public const float targetAlpha = 0.45f;
    //can mess with this later

    //tilemap
    public const float gridCellSize = 1f; //grid cell size in unity units
    public const float gridCellDiagonalSize = 1.41f; //diagonal distance between untiy cell centres
    public const int maxGridWidth = 99999;
    public const int maxGridHeight = 99999;
    //cursor size
    public static Vector2 cursorSize = Vector2.one;

    //adjust the players position
    //adjust by 11 pixels (16 pixels is a unity unit
    public static float playerCentreYOffset = 0.875f;

    //player movement
    public const float runningSpeed = 5.333f;
    public const float walkingSpeed = 2.666f;
    //pause when using a tool
    public static float useToolAnimationPause = 0.25f;
    //run with coroutines and yields to help with correctness
    public static float liftToolAnimationPause = 0.4f;
    public static float pickAnimationPause = 1f;
    public static float afterUseToolAnimationPause = 0.2f;
    public static float afterLiftToolAnimationPause = 0.4f;
    public static float afterPickAnimationPause = 0.2f;

    //NPC movement
    //see if within a pixels size from the end of the loop
    public static float pixelSize = 0.0625f;


    //player inventory capacity
    public static int playerInitialInventoryCapacity = 24;
    public static int playerMaximumInventoryCapacity = 48;

//npc animation parameters
//for all the different animations
 
    public static int walkUp;
    public static int walkDown;
    public static int walkLeft;
    public static int walkRight;
    public static int eventAnimation;

    //Player Animation Parameters
    public static int xInput;
    public static int yInput;
    public static int isWalking;
    public static int isRunning;
    public static int toolEffect;
    public static int isUsingToolRight;
    public static int isUsingToolLeft;
    public static int isUsingToolUp;
    public static int isUsingToolDown;
    public static int isLiftingToolRight;
    public static int isLiftingToolLeft;
    public static int isLiftingToolUp;
    public static int isLiftingToolDown;
    public static int isSwingingToolRight;
    public static int isSwingingToolLeft;
    public static int isSwingingToolUp;
    public static int isSwingingToolDown;
    public static int isPickingRight;
    public static int isPickingLeft;
    public static int isPickingUp;
    public static int isPickingDown;



    //Shared Animation Parameters
    public static int idleUp;
    public static int idleDown;
    public static int idleRight;
    public static int idleLeft;

    //next set up a constructor for when its instantiated
    //run when created also, for a static class its not run but rather just created 1 time

    //Tools
    //strings that hold descriptions for tool types
    public const string HoeingTool = "Hoe";
    public const string ChoppingTool = "Axe";
    public const string BreakingTool = "Picaxe";
    public const string ReapingTool = "Scythe";
    public const string WateringTool = "Watering Can";
    public const string CollectingTool = "Basket";

    //reaping
    //gonna trigger a routine for detection area
    public const int maxCollidersToTestPerReapSwing = 15;
    //for every swing can only delete two items max
    public const int maxTargetComponentsToDestroyPerReapSwing = 2;



    //time settings
    //every 1/7 of a second is a game minute
    //keeps time moving
    public const float secondsPerGameSecond = 0.012f;

    //static constructor
    static Settings()
    {
        //set up hash values for each integer value of the parameters
        //using a static method on the animator class known as "StringToHash" on the animator class
        //put in the parameter string
        //converts to hash and puts it in the variables
        
        //npc animation parameters
        //generate hash values for the parameters
        walkUp = Animator.StringToHash("walkUp");
        walkDown = Animator.StringToHash("walkDown");
        walkLeft = Animator.StringToHash("walkLeft");
        walkRight = Animator.StringToHash("walkRight");
        eventAnimation = Animator.StringToHash("eventAnimation");

        //player animation parameters
        xInput = Animator.StringToHash("xInput");
        yInput = Animator.StringToHash("yInput");
        isWalking = Animator.StringToHash("isWalking");
        isRunning = Animator.StringToHash("isRunning");
        toolEffect = Animator.StringToHash("toolEffect");
        isUsingToolRight = Animator.StringToHash("isUsingToolRight");
        isUsingToolLeft = Animator.StringToHash("isUsingToolLeft");
        isUsingToolUp = Animator.StringToHash("isUsingToolUp");
        isUsingToolDown = Animator.StringToHash("isUsingToolDown");
        isLiftingToolRight = Animator.StringToHash("isLiftingToolRight");
        isLiftingToolLeft = Animator.StringToHash("isLiftingToolLeft");
        isLiftingToolUp = Animator.StringToHash("isLiftingToolUp");
        isLiftingToolDown = Animator.StringToHash("isLiftingToolDown");
        isSwingingToolRight = Animator.StringToHash("isSwingingToolRight");
        isSwingingToolLeft = Animator.StringToHash("isSwingingToolLeft");
        isSwingingToolUp = Animator.StringToHash("isSwingingToolUp");
        isSwingingToolDown = Animator.StringToHash("isSwingingToolDown");
        isPickingRight = Animator.StringToHash("isPickingRight");
        isPickingLeft = Animator.StringToHash("isPickingLeft");
        isPickingUp = Animator.StringToHash("isPickingUp");
        isPickingDown = Animator.StringToHash("isPickingDown");



        //Shared animation parameters
        idleUp = Animator.StringToHash("idleUp");
        idleDown = Animator.StringToHash("idleDown");
        idleLeft = Animator.StringToHash("idleLeft");
        idleRight = Animator.StringToHash("idleRight");
    }


}

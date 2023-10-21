using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

//require all of these for npc movments
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NPCPath))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]

public class NPCMovement : MonoBehaviour
{
    //member variables
    public SceneName npcCurrentScene;
    [HideInInspector] public SceneName npcTargetScene;
    //grid based
    [HideInInspector] public Vector3Int npcCurrentGridPosition;
    [HideInInspector] public Vector3Int npcTargetGridPosition;
    //unity units based
    [HideInInspector] public Vector3 npcTargetWorldPosition;
    public Direction npcFacingDirectionAtDestination;

//track and manage current and next grid positions
    private SceneName npcPreviousMovementStepScene;
    private Vector3Int npcNextGridPosition;
    private Vector3 npcNextWorldPosition;

    [Header("NPC Movement")]
    public float npcNormalSpeed = 2f;

//not slower or faster than min or max
    [SerializeField] private float npcMinSpeed =1f;
    [SerializeField] private float npcMaxSpeed = 3f;
    //when move set to true
    private bool npcIsMoving = false;

//what gets played when target is reached
    [HideInInspector] public AnimationClip npcTargetAnimationClip;

    [Header("NPC Animation")]
    [SerializeField] private AnimationClip blankAnimation = null;

    private Grid grid;
    private Rigidbody2D rigidBody2D;
    private BoxCollider2D boxCollider2D;
    //used in movement coroutine, yield and wait for the fixed update
    private WaitForFixedUpdate waitForFixedUpdate;
    private Animator animator;
    //overide the blank animation in the controller with the actual animation
    private AnimatorOverrideController animatorOverrideController;
    private int lastMoveAnimationParameter;
    private NPCPath npcPath;
    //does npc exist?
    private bool npcInitialised = false;
    private SpriteRenderer spriteRenderer;
    [HideInInspector] public bool npcActiveInScene = false;

    //some of the things needed to do are dependent on whether the scenes loaded
    private bool sceneLoaded = false;

//easily stock later when we want to
    private Coroutine moveToGridPositionRoutine;

//on enable and disable, subscribe to the after scene load method
    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += AfterSceneLoad;
        EventHandler.BeforeSceneUnloadEvent += BeforeSceneUnloaded;
    }
    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoad;
        EventHandler.BeforeSceneUnloadEvent -= BeforeSceneUnloaded;
    }

    private void Awake()
    {
        //get all variables caching the components in
        rigidBody2D = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        npcPath = GetComponent<NPCPath>();
        spriteRenderer = GetComponent<SpriteRenderer>();

//pass in the animator and the runtime animator controller 
        animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = animatorOverrideController;
        
        //initialise target world position, target grid position & target scene to current
        npcTargetScene = npcCurrentScene;
        npcTargetGridPosition = npcCurrentGridPosition;
        npcTargetWorldPosition = transform.position;
    }

    //start is called before the first frame updates
    private void Start()
    {
        waitForFixedUpdate = new WaitForFixedUpdate();
//set the idle animation for the npc
        SetIdleAnimation();
    }

    private void FixedUpdate()
    {
        //npc moving component moves the npc
        //pulling the stack to see if there are steps remaining
        //only process if loaded
        if(sceneLoaded)
        {
            //only process if npc not moving
            if(npcIsMoving == false)
            {
                //set npc current and next grid position - to take into account the npc might be animating
                //returns grid position as an int
                npcCurrentGridPosition = GetGridPosition(transform.position);
                npcNextGridPosition = npcCurrentGridPosition;

                if(npcPath.npcMovementStepStack.Count > 0)
                {
                    //look at and return it but dont remove it
                    NPCMovementStep npcMovementStep = npcPath.npcMovementStepStack.Peek();
                    //current scene with the movement step
                    npcCurrentScene = npcMovementStep.sceneName;

                    //if npc is about to move to a new scene reset position to starting point in a new scene and update the step times
                    //is the scene in the step different from the current scene
                    //if it is then we know we need to change scenes
                    if(npcCurrentScene != npcPreviousMovementStepScene)
                    {
                        //new starting position
                        npcCurrentGridPosition = (Vector3Int)npcMovementStep.gridCoordinate;
                        npcNextGridPosition = npcCurrentGridPosition;
                        transform.position = GetWorldPosition(npcCurrentGridPosition);
                        npcPreviousMovementStepScene = npcCurrentScene;
                        npcPath.UpdateTimesOnPath();
                    }

                    //If NPC is in current scene then set NPC to active to make visible, pop the movement step off the stack and then call method to move NPC
                    //convert to string to compare
                    if (npcCurrentScene.ToString() == SceneManager.GetActiveScene().name)
                    {
                        //set npc active if in the current scene
                        SetNPCActiveInScene();

                    //now we remove the step from the stack     
                        npcMovementStep = npcPath.npcMovementStepStack.Pop();

                        npcNextGridPosition = (Vector3Int)npcMovementStep.gridCoordinate;

                        //new time span for moving
                        TimeSpan npcMovementStepTime = new TimeSpan(npcMovementStep.hour, npcMovementStep.minute, npcMovementStep.second);
                        
                        //pass in current time
                        MoveToGridPosition(npcNextGridPosition, npcMovementStepTime, TimeManager.Instance.GetGameTime());
                    }
                    //else if NPC is not in current scene then set npc inactive to make invisible
                    // - once the movement step time is less than game time(in the past) then pop movement 
                    //step off the stack and set NPC position to movement step position
                    else
                    {
                        //if npc not in current scene
                        SetNPCInactiveInScene();
                        //not incramentally if not seen, just pop it into position if the game time is past when the npc should be in a position
                        //npc position
                        npcCurrentGridPosition = (Vector3Int)npcMovementStep.gridCoordinate;
                        npcNextGridPosition = npcCurrentGridPosition;
                        transform.position = GetWorldPosition(npcCurrentGridPosition);

                        //pass in current movement step hours and minutes
                        TimeSpan npcMovementStepTime = new TimeSpan(npcMovementStep.hour, npcMovementStep.minute, npcMovementStep.second);
                        //get game time
                        TimeSpan gameTime = TimeManager.Instance.GetGameTime();
                        //is it less then the game time so are you in the past
                        if(npcMovementStepTime < gameTime)
                        {
                            //pop the movement step
                            npcMovementStep = npcPath.npcMovementStepStack.Pop();
                            //move them to the place
                            npcCurrentGridPosition = (Vector3Int)npcMovementStep.gridCoordinate;
                            npcNextGridPosition = npcCurrentGridPosition;
                            transform.position = GetWorldPosition(npcCurrentGridPosition);
                        }
                    }
                }
                //else if no more NPC movement steps
                else
                {
                    ResetMoveAnimation();

                    SetNPCFacingDirection();

                    SetNPCEventAnimation();
                }

            }
        }
    }

    public void SetScheduleEventDetails(NPCScheduleEvent npcScheduleEvent)
    {
        //sets variables based on the schedule
        npcTargetScene = npcScheduleEvent.toSceneName;
        npcTargetGridPosition = (Vector3Int)npcScheduleEvent.toGridCoordinate;
        npcTargetWorldPosition = GetWorldPosition(npcTargetGridPosition);
        npcFacingDirectionAtDestination = npcScheduleEvent.npcFacingDirectionAtDestination;
        npcTargetAnimationClip = npcScheduleEvent.animationAtDestination;
        ClearNPCEventAnimation();
    }

    private void SetNPCEventAnimation()
    {
        //look to see if theres a clip specified
        if(npcTargetAnimationClip != null)
        {
            //reset idle
            ResetIdleAnimation();
            //use override controller with the blank animation and then switch it over
            animatorOverrideController[blankAnimation] = npcTargetAnimationClip;
            //set trigger to true to trigger the new animation
            animator.SetBool(Settings.eventAnimation, true);
        }
        else
        {
            //replace blank with blank and then set trigger to false
            animatorOverrideController[blankAnimation] = blankAnimation;
            animator.SetBool(Settings.eventAnimation, false);
        }
    }

    public void ClearNPCEventAnimation()
    {
        //reset blank animation and set as being false, then clear rotation
        animatorOverrideController[blankAnimation] = blankAnimation;
        animator.SetBool(Settings.eventAnimation, false);

        //clear any rotation on npc
        transform.rotation = Quaternion.identity;
    }

    private void SetNPCFacingDirection()
    {
        ResetIdleAnimation();

        switch(npcFacingDirectionAtDestination)
        {
            case Direction.up:
                animator.SetBool(Settings.idleUp, true);
                break;
            case Direction.down:
                animator.SetBool(Settings.idleDown, true);
                break;
            case Direction.left:
                animator.SetBool(Settings.idleLeft, true);
                break;
            case Direction.right:
                animator.SetBool(Settings.idleRight, true);
                break;
            case Direction.none:
                break;
            default:
                break;
        }
    }

    public void SetNPCActiveInScene()
    {
        //make visable
        spriteRenderer.enabled = true;
        //enable its collider
        boxCollider2D.enabled = true;
        //make sure its on
        npcActiveInScene = true;
    }

    public void SetNPCInactiveInScene()
    {
        //disables the box colider nd sprite
        spriteRenderer.enabled = false;
        boxCollider2D.enabled = false;
        npcActiveInScene = false;
    }

    private void AfterSceneLoad()
    {
        //populate the grid variable
        grid = GameObject.FindObjectOfType<Grid>();

//see if initialized, if not then initialize
        if(!npcInitialised)
        {
            InitialiseNPC();
            npcInitialised = true;

        }
//scene load is true
        sceneLoaded = true;

    }

    private void BeforeSceneUnloaded()
    {
        sceneLoaded = false;
    }

    ///<summary>
    /// returns grid position given the world position
    ///</summary>
    private Vector3Int GetGridPosition(Vector3 worldPosition)
    {
        //takes world position and converts to grid position
        if(grid != null)
        {
            return grid.WorldToCell(worldPosition);
        }
        else
        {
            //if not on the grid
            return Vector3Int.zero;
        }
    }

    ///<summary>
    /// returns the world position (center of grid square) from gridPosition
    ///</summary>
    public Vector3 GetWorldPosition(Vector3Int gridPosition)
    {
        //convert grid to world
        Vector3 worldPosition = grid.CellToWorld(gridPosition);

        //get center of grid square
        //based on size returns the center of grid position in world position space
        return new Vector3(worldPosition.x + Settings.gridCellSize / 2f, worldPosition.y + Settings.gridCellSize / 2f, worldPosition.z);
    }
    //if save is triggered while npc is moving dont want to save movement just want to save final position

    public void CancelNPCMovement()
    {
        //clear the current path
        npcPath.ClearPath();
        //set next grid position to zero
        npcNextGridPosition = Vector3Int.zero;
        //set world position to zero
        npcNextWorldPosition = Vector3.zero;
        //not moving anymore
        npcIsMoving = false;

//is a coroutine active right now?
        if(moveToGridPositionRoutine != null)
        {
            //edges npc forward frame by frame, if active please stop
            StopCoroutine(moveToGridPositionRoutine);
        }
        //reset move animation
        ResetMoveAnimation();

        //clear event animation
        ClearNPCEventAnimation();
        npcTargetAnimationClip = null;

        //reset idle animation
        ResetIdleAnimation();

        //set idle animation
        SetIdleAnimation();

    }

    private void InitialiseNPC()
    {
        // active in scene
        if(npcCurrentScene.ToString() == SceneManager.GetActiveScene().name)
        {
            SetNPCActiveInScene();
        }
        else
        {
            SetNPCInactiveInScene();
        }

        npcPreviousMovementStepScene = npcCurrentScene;
        //set NPC current grid position
        npcCurrentGridPosition = GetGridPosition(transform.position);

        //set next grid position and target grid position to grid position
        npcNextGridPosition = npcCurrentGridPosition;
        npcTargetGridPosition = npcCurrentGridPosition;
        npcTargetWorldPosition = GetWorldPosition(npcTargetGridPosition);

        //get npc world position 
        npcNextWorldPosition = GetWorldPosition(npcCurrentGridPosition);
    }

    private void MoveToGridPosition(Vector3Int gridPosition, TimeSpan npcMovementStepTime, TimeSpan gameTime)
    {
        //starts a coroutine passes in grid position, npc movment, step time and game time
        moveToGridPositionRoutine = StartCoroutine(MoveToGridPositionRoutine(gridPosition, npcMovementStepTime, gameTime));
    }
    private IEnumerator MoveToGridPositionRoutine(Vector3Int gridPosition, TimeSpan npcMovementStepTime, TimeSpan gameTime)
    {
        //indicate that the npc is moving
        npcIsMoving = true;

        //set move animation and set animation based on where we are moving to
        SetMoveAnimation(gridPosition);

    //get next position
        npcNextWorldPosition = GetWorldPosition(gridPosition);

        //if movement step time is in the future, otherwise skip and move NPC immediately to position
        if (npcMovementStepTime > gameTime)
        {
            //calculate time difference in seconds
            //how much time left to move
            float timeToMove = (float)(npcMovementStepTime.TotalSeconds - gameTime.TotalSeconds);

            //calculate speed
            //code correction for the npcCalculate speed 
            //want to be bigger than the minspeed or the the calculated speed
            //pick which ever is bigger, min speed or the calculation
            float npcCalculatedSpeed  = Mathf.Max(npcMinSpeed, Vector3.Distance(transform.position, npcNextWorldPosition) / timeToMove / Settings.secondsPerGameSecond);

            //// if speed is at least npc min speed and less than npc max speed then process, otherwise skip and move npc immediately to position
            if(npcCalculatedSpeed >= npcMinSpeed && npcCalculatedSpeed <= npcMaxSpeed)
            {
                //loop through until 1 pixel away
                while(Vector3.Distance(transform.position, npcNextWorldPosition) > Settings.pixelSize)
                {
                    //edge rigid body toward where we want to go
                    Vector3 unitVector = Vector3.Normalize(npcNextWorldPosition - transform.position);
                    //calculate the distance we want to move
                    //loop through this as part of the fixed update
                    //speed times time calculates the distance in x direction and y direction
                    Vector2 move = new Vector2(unitVector.x * npcCalculatedSpeed * Time.fixedDeltaTime, unitVector.y * npcCalculatedSpeed * Time.fixedDeltaTime);

                    //move from current position and adds move to it
                    rigidBody2D.MovePosition(rigidBody2D.position + move);

                    yield return waitForFixedUpdate;
                }
            }
        }

//update position precisely 
        rigidBody2D.position = npcNextWorldPosition;
        npcCurrentGridPosition = gridPosition;
        npcNextGridPosition = npcCurrentGridPosition;
        npcIsMoving = false;
    }

    private void SetMoveAnimation(Vector3Int gridPosition)
    {
        //reset idle animation
        ResetIdleAnimation();

        //reset move animation
        ResetMoveAnimation();

        //get world position of where we are going
        Vector3 toWorldPosition = GetWorldPosition(gridPosition);

        //get direction vector from current position to where we're going to
        Vector3 directionVector = toWorldPosition - transform.position;

//if our direction x is greater than y then use the right and left animation, mostly going to be doing that
        if(Mathf.Abs(directionVector.x) >= Mathf.Abs(directionVector.y))
        {
            // use left / right animation
            if(directionVector.x > 0)
            {
                animator.SetBool(Settings.walkRight, true);

            }
            else
            {
                animator.SetBool(Settings.walkLeft, true);
            }

        }
        else{
            //use up/down animation
            if(directionVector.y > 0)
            {
                animator.SetBool(Settings.walkUp, true);
            }
            else
            {
                animator.SetBool(Settings.walkDown, true);
            }
        }
    }

    private void SetIdleAnimation()
    {
        animator.SetBool(Settings.idleDown, true);
    }

    private void ResetMoveAnimation()
    {
        animator.SetBool(Settings.walkRight, false);
        animator.SetBool(Settings.walkLeft, false);
        animator.SetBool(Settings.walkUp, false);
        animator.SetBool(Settings.walkDown, false);

    }
    private void ResetIdleAnimation()
    {
        animator.SetBool(Settings.idleRight, false);
        animator.SetBool(Settings.idleLeft, false);
        animator.SetBool(Settings.idleUp, false);
        animator.SetBool(Settings.idleDown, false);
    }
}

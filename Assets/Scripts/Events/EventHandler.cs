using System;
using System.Collections.Generic;
using UnityEngine;



//next make the  delegate for the movement event
//create own delegate because default doesnt account for enough parameters
//info to pass needs to be passed
//c# specify each type before defining, also all of these i believe are triggers on the animation tree for the player
public delegate void MovementDelegate(float inputX, float inputY, bool isWalking, bool isRunning, bool isIdle, bool isCarrying,
    ToolEffect toolEffect,
    bool isUsingToolRight, bool isUsingToolLeft, bool isUsingToolUp, bool isUsingToolDown,
    bool isLiftingToolRight, bool isLiftingToolLeft, bool isLiftingToolUp, bool isLiftingToolDown,
    bool isPickingRight, bool isPickingLeft, bool isPickingUp, bool isPickingDown,
    bool isSwingingToolRight, bool isSwingingToolLeft, bool isSwingingToolUp, bool isSwingingToolDown,
    bool idleUp, bool idleDown, bool idleLeft, bool idleRight);
//refrence type that will hold refrences to methods
//if you have a bunch of parameters creating a delegate is more important
 



//first define the class, public to be seen, static to be modified

public static class EventHandler
{
    //Drop selected item event
    //subscribers see
    public static event Action DropSelectedItemEvent;

    //publishers call
    public static void CallDropSelectedItemEvent()
    {
        if(DropSelectedItemEvent != null)
        {
            DropSelectedItemEvent();
        }
    }

    public static event Action RemoveSelectedItemFromInventoryEvent;

    //plant item and then remove from inventory
    public static void CallRemoveSelectedItemFromInventoryEvent()
    {
        if(RemoveSelectedItemFromInventoryEvent != null)
        {
            RemoveSelectedItemFromInventoryEvent();
        }
    }

    //Harvest Action effect event
    //enum and the action
   public static event Action<Vector3, HarvestActionEffect> HarvestActionEffectEvent;

   public static void CallHarvestActionEffectEvent(Vector3 effectPosition, HarvestActionEffect harvestActionEffect){
       if(HarvestActionEffectEvent != null){
           HarvestActionEffectEvent(effectPosition, harvestActionEffect);
       }
   }

    //inventory update event
    //can use action, a system defined delegate because there arent many parameters
    //one is the location and the other is the list
    //any interested code would subscribe to this event
    //create a function that is recognized and called by any piece of code that publishes or triggers the event
    public static event Action<InventoryLocation, List<InventoryItem>> InventoryUpdatedEvent;

    public static void CallInventoryUpdatedEvent(InventoryLocation inventoryLocation, List<InventoryItem> inventoryList)
    {
        if(InventoryUpdatedEvent != null)
        {
            //if the event exsists, pass the inventory location and inventory list into the updated event
            InventoryUpdatedEvent(inventoryLocation, inventoryList);
        }
    }

    //Instantiate crop prefab
    public static event Action InstantiateCropPrefabsEvent;

//triggers the event
//call as part od initializing grid properites to get the grid running
    public static void CallInstantiateCropPrefabsEvent()
    {
        if(InstantiateCropPrefabsEvent != null)
        {
            InstantiateCropPrefabsEvent();
        }
    }


    
    // movement event
    public static event MovementDelegate MovementEvent;
    //when ever a subscriber wants to know about this then they will refrence it and that area will run


    //movement event call for publishers
    public static void CallMovementEvent(float inputX, float inputY, bool isWalking, bool isRunning, bool isIdle, bool isCarrying,
    ToolEffect toolEffect,
    bool isUsingToolRight, bool isUsingToolLeft, bool isUsingToolUp, bool isUsingToolDown,
    bool isLiftingToolRight, bool isLiftingToolLeft, bool isLiftingToolUp, bool isLiftingToolDown,
    bool isPickingRight, bool isPickingLeft, bool isPickingUp, bool isPickingDown,
    bool isSwingingToolRight, bool isSwingingToolLeft, bool isSwingingToolUp, bool isSwingingToolDown,
    bool idleUp, bool idleDown, bool idleLeft, bool idleRight)
    {
        //first check for subscribers, only execute if there are subscribers
        if(MovementEvent != null)
            MovementEvent(inputX, inputY, isWalking, isRunning, isIdle, isCarrying, toolEffect,
               isUsingToolRight, isUsingToolLeft, isUsingToolUp, isUsingToolDown,
               isLiftingToolRight, isLiftingToolLeft, isLiftingToolUp, isLiftingToolDown,
               isPickingRight, isPickingLeft, isPickingUp, isPickingDown,
               isSwingingToolRight, isSwingingToolLeft, isSwingingToolUp, isSwingingToolDown,
               idleUp, idleDown, idleLeft, idleRight);



    }
    //events in game will want to know when time changes
    //will need one for every time that advances

    //TIME EVENTS
    //Advance game minutes
    //built in action delegate
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameMinuteEvent;

    //calls the advanceGameMinute event, pass in all of these parameters
    public static void CallAdvanceGameMinuteEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour,
        int gameMinute, int gameSecond)
    {
        if(AdvanceGameMinuteEvent != null)
        {
            AdvanceGameMinuteEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
        }
    }


    //Advance game hour
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameHourEvent;

    //calls the advanceGameHour event

    public static void CallAdvanceGameHourEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour,
        int gameMinute, int gameSecond)
    {
        if(AdvanceGameHourEvent != null)
        {
            AdvanceGameHourEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
        }
    }

    //Advance game day
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameDayEvent;

    //calls the advanceGameDayEvent event
    public static void CallAdvanceGameDayEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour,
        int gameMinute, int gameSecond)
    {
        if(AdvanceGameDayEvent != null)
        {
            AdvanceGameDayEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
        }
    }

    //Advance game season
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameSeasonEvent;

    //calls advance game season event
    public static void CallAdvanceGameSeasonEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour,
        int gameMinute, int gameSecond)
    {
        if(AdvanceGameSeasonEvent != null)
        {
            AdvanceGameSeasonEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
        }
    }

    //Advance game year
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameYearEvent;

    //calls the advance game year event
    public static void CallAdvanceGameYearEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour,
        int gameMinute, int gameSecond)
    {
        if(AdvanceGameYearEvent != null)
        {
            AdvanceGameYearEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
        }
    }


    //scene load events


    //Before scene uload fade out event
    //subscribers subscribe to this

    //scene controller will use these to trigger scene transitions
    public static event Action BeforeSceneUnloadFadeOutEvent;

    //notifactions really, no functionality
    //publishers do this
    public static void CallBeforeSceneUnloadFadeOutEvent()
    {
        if(BeforeSceneUnloadFadeOutEvent != null)
        {
            BeforeSceneUnloadFadeOutEvent();
        }
    }

    //before scene unload event
    public static event Action BeforeSceneUnloadEvent;


    public static void CallBeforeSceneUnloadEvent()
    {
        if(BeforeSceneUnloadEvent != null)
        {
            BeforeSceneUnloadEvent();
        }
    }

    //after scene loaded event
    public static event Action AfterSceneLoadEvent;

    public static void CallAfterSceneLoadEvent()
    {
        if(AfterSceneLoadEvent != null)
        {
            AfterSceneLoadEvent();
        }
    }

    //after scene load fade in event
    public static event Action AfterSceneLoadFadeInEvent;

    public static void CallAfterSceneLoadFadeInEvent()
    {
        if(AfterSceneLoadFadeInEvent != null)
        {
            AfterSceneLoadFadeInEvent();
        }
    }




}

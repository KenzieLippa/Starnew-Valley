using UnityEngine;

[System.Serializable]
public class NPCScheduleEvent
{
    //data container for event we want to test for
   public int hour;
   public int minute;
   public int priority;
   public int day;
   public Weather weather;
   public Season season;
     //what scene do we want to move to
   public SceneName toSceneName;
   public GridCoordinate toGridCoordinate;
   //what direction will they face when they hit their destination
   //trigger an idle animation
   public Direction npcFacingDirectionAtDestination = Direction.none;
   public AnimationClip animationAtDestination;

   public int Time
   {
       get
       {
           //calc and return the hours
           return (hour * 100) + minute;
       }
   }
//as events are called they will be created and then executed
//tells when it should happen, what priority it is, if theres a specific day
//when theres some kind of weather or season 
//what animation should be played at the destination
   public NPCScheduleEvent(int hour, int minute, int priority, int day, Weather weather, Season season, SceneName toSceneName, GridCoordinate toGridCoordinate, 
   AnimationClip animationAtDestination)
   {
       //update member variables 
       this.hour = hour;
       this.minute = minute;
       this.priority = priority;
       this.day = day;
       this.weather = weather;
       this.season = season;
       this.toSceneName = toSceneName;
       this.toGridCoordinate = toGridCoordinate;
       this.animationAtDestination = animationAtDestination;

   }
   public NPCScheduleEvent()
   {

   }

//if want to look at the info then use this
   public override string ToString()
   {
       return $"Time: {Time}, Priority: {priority}, Day: {day} Weather: {weather}, Season: {season}";
   }
}

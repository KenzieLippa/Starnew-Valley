using System.Collections.Generic;
using UnityEngine;

//need an npc path component to function
[RequireComponent(typeof(NPCPath))]
public class NPCSchedule : MonoBehaviour
{
    //so event holds the schedule events
   [SerializeField] private SO_NPCScheduleEventList so_NPCScheduleEventList = null;
   //member variable of sorted set type, load events from so to this sorted set and then will order them
   private SortedSet<NPCScheduleEvent> npcScheduleEventSet;
   //refrence the path
   private NPCPath npcPath;

   private void Awake()
   {
       //load npc schedule event list into a sorted set
       //pass into the constructor the npc sort class, impliments comparison functionality
       //any value will be sorted from this
       npcScheduleEventSet = new SortedSet<NPCScheduleEvent>(new NPCScheduleEventSort());
       //loop through all objects in so list and add it to the newly created set
       foreach (NPCScheduleEvent npcScheduleEvent in so_NPCScheduleEventList.npcScheduleEventList)
       {
           npcScheduleEventSet.Add(npcScheduleEvent);
       }

       //Get npc path component
       //cache in member variable
       npcPath = GetComponent<NPCPath>();
   }

   private void OnEnable()
   {
       //subscribe to advance minute event
       EventHandler.AdvanceGameMinuteEvent += GameTimeSystem_AdvanceMinute;
   }

   private void OnDisable()
   {
       //unsubscribing to the event
       EventHandler.AdvanceGameMinuteEvent -= GameTimeSystem_AdvanceMinute;
   }

//gets called every game minute
   private void GameTimeSystem_AdvanceMinute(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
   {
       //calc game time
       int time = (gameHour * 100) + gameMinute;
       //attempt to get matching schedule

//get a matching schedule
       NPCScheduleEvent matchingNPCScheduleEvent = null;

//loop through all schedule event in schedule set
       foreach (NPCScheduleEvent npcScheduleEvent in npcScheduleEventSet)
       {
           //if time match then continue, if not then break if the event time is higher than the current time
           if (npcScheduleEvent.Time == time)
           {
               //time match now check if parameters match
               //check to see if we have matches on the parameters
               //highest priority first, once a match is found then drop out
                if(npcScheduleEvent.day != 0 && npcScheduleEvent.day != gameDay)
                    continue;
                if(npcScheduleEvent.season != Season.none && npcScheduleEvent.season != gameSeason)
                    continue;
                if(npcScheduleEvent.weather != Weather.none && npcScheduleEvent.weather != GameManager.Instance.currentWeather)
                    continue;
                //schedule matches
                //Debug.Log("schedule matches" +npcScheduleEvent);
                //have a match then set it
                matchingNPCScheduleEvent = npcScheduleEvent;
                break;     

           }
           else if(npcScheduleEvent.Time > time)
           {
               break;
           }
           
       }
       //now test is matching schedule = null and do something;
       if(matchingNPCScheduleEvent != null)
       {
           //build path for matching schedule
           npcPath.BuildPath(matchingNPCScheduleEvent);
       }
   }
}

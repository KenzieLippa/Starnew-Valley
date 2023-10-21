
using System.Collections.Generic;


public class NPCScheduleEventSort : IComparer<NPCScheduleEvent>
{
    //pass into a public class through a constructor and sorts the list
    public int Compare(NPCScheduleEvent npcScheduleEvent1, NPCScheduleEvent npcScheduleEvent2)
    {
        //do both have the same time
        if(npcScheduleEvent1?.Time == npcScheduleEvent2?.Time)
        {
            //then check the priority of the event
            // ?. is a null operator to avoid null refrence errors, drops out if theres a null and stops checking
            if(npcScheduleEvent1?.priority < npcScheduleEvent2?.priority)
            {
                return -1;
            }
            else
            {
                return 1;
            }

        }
        //if not the same time then sort them by time
        else if(npcScheduleEvent1?.Time > npcScheduleEvent2?.Time)
        {
            return 1;
        }
        else if(npcScheduleEvent1?.Time < npcScheduleEvent2?.Time)
        {
            return -1;
        }
        else
        {
            return 0;
        }
       
    }

}

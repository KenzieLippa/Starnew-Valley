using TMPro;
using UnityEngine;

public class GameClock : MonoBehaviour
{
    //populate in inspector baby!
    [SerializeField] private TextMeshProUGUI timeText = null;
    [SerializeField] private TextMeshProUGUI dateText = null;
    [SerializeField] private TextMeshProUGUI seasonText = null;
    [SerializeField] private TextMeshProUGUI yearText = null;


    //subscribin time!!!

    //first we subscribe to the minutes event
    private void OnEnable()
    {
        EventHandler.AdvanceGameMinuteEvent += UpdateGameTime;
    }
    //now we unsubscribe
    private void OnDisable()
    {
        EventHandler.AdvanceGameMinuteEvent -= UpdateGameTime;
    }

    //passed around for the event as well
    private void UpdateGameTime(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        //update time
        //subtracts the remainder so the incraments are only in 10s
        gameMinute = gameMinute - (gameMinute % 10);

        string ampm = "";
        string minute;

        //logic check to see if afternoon
        if(gameHour >= 12)
        {
            ampm = "pm";
        }
        else
        {
            ampm = "am";
        }

        if(gameHour >= 13)
        {
            gameHour -= 12;

        }
        if(gameMinute < 10)
        {
            //two digits for the game minute
            minute = "0" + gameMinute.ToString();
        }
        else
        {
            minute = gameMinute.ToString();
        }


        string time = gameHour.ToString() + ":" + minute + ampm;

        timeText.SetText(time);
        dateText.SetText(gameDayOfWeek + "." + gameDay .ToString());
        seasonText.SetText(gameSeason.ToString());
        yearText.SetText("Year " + gameYear);
    }
}

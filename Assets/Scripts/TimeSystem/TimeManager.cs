using System;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : SingletonMonobehaviour<TimeManager>, ISaveable
{
    //makes a singleton game object

    //keeping track of current time 
    private int gameYear = 1;
    private Season gameSeason = Season.Spring;
    private int gameDay = 1;
    private int gameHour = 6;
    private int gameMinute = 30;
    private int gameSecond = 0;
    private string gameDayOfWeek = "Mon";

    private bool gameClockPaused = false;

    //update to see if a game second has happened
    private float gameTick = 0f;

    //get property variables and GUID set up
    private string _iSaveableUniqueID;
    public string ISaveableUniqueID{get{return _iSaveableUniqueID;} set{_iSaveableUniqueID = value;}}

    private GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave {get{return _gameObjectSave;} set{_gameObjectSave = value;}}

    //initialize GUID
    protected override void Awake()
    {
        //want singleton as well
        base.Awake();

        //get the guid from the component
        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        //game object save property initialized
        GameObjectSave = new GameObjectSave();
    }
    private void OnEnable()
    {
        ISaveableRegister();

//link to the new methods about before fading and after fading
//pause the game clock as scenes are switched
        EventHandler.BeforeSceneUnloadEvent += BeforeSceneUnloadFadeOut;
        EventHandler.AfterSceneLoadEvent += AfterSceneLoadFadeIn;
    }

    private void OnDisable()
    {
        ISaveableDeregister();

        EventHandler.BeforeSceneUnloadEvent -= BeforeSceneUnloadFadeOut;
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoadFadeIn;
    }
    private void BeforeSceneUnloadFadeOut()
    {
        //pause before
        gameClockPaused = true;
    }
    private void AfterSceneLoadFadeIn()
    {
        //unpause after
        gameClockPaused = false;
    }
    private void Start()
    {
        //any object curious about the passing of minutes will get this notification right away
        EventHandler.CallAdvanceGameMinuteEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
    }

    private void Update()
    {
        if (!gameClockPaused)
        {
            //if game not paused call this method
            GameTick();
        }
    }

    private void GameTick()
    {
        //add time to game tick
        gameTick += Time.deltaTime;

        //see if a game second has happened
        if (gameTick >= Settings.secondsPerGameSecond)
        {
            //subtract the seconds per game second tick
            gameTick -= Settings.secondsPerGameSecond;
            //update game seconds
            UpdateGameSecond();
        }
    }

    private void UpdateGameSecond()
    {
        //increase the game second
        gameSecond++;
        //test if its greater than 59
        if (gameSecond > 59)
        {
            gameSecond = 0;
            gameMinute++;


            //and so on using logic
            if (gameMinute > 59)
            {
                gameMinute = 0;
                gameHour++;


                if (gameHour > 23)
                {
                    gameHour = 0;
                    gameDay++;


                    if (gameDay > 30)
                    {
                        gameDay = 1;

                        //game season is technically an enum so use this to incrament
                        int gs = (int)gameSeason;
                        gs++;
                        //cast back to enum so you can see the seasons
                        gameSeason = (Season)gs;

                        if (gs > 3)
                        {
                            gs = 0;
                            //cast it back as a number so it can be checked again
                            gameSeason = (Season)gs;

                            gameYear++;

                            if (gameYear > 9999)
                            {
                                gameYear = 1;
                            }

                            EventHandler.CallAdvanceGameYearEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);

                        }

                        EventHandler.CallAdvanceGameSeasonEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
                    }
                    gameDayOfWeek = GetDayOfWeek();
                    EventHandler.CallAdvanceGameDayEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);

                }

                EventHandler.CallAdvanceGameHourEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
            }

            EventHandler.CallAdvanceGameMinuteEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
           // Debug.Log("Game Year: " + gameYear + " Game Season: " + gameSeason + " GameDay: " + gameDay + " GameHour: " + gameHour +
                //" Game Minute: " + gameMinute);
        }
        //would call seconds here if we were going to call them
    }

    private string GetDayOfWeek()
    {
        //calc total days happened thusfar
        int totalDays = (((int)gameSeason) * 30) + gameDay;
        //gives the remainder
        int dayOfWeek = totalDays % 7;

        switch (dayOfWeek)
        {
            case 1:
                return "Mon";

            case 2:
                return "Tue";

            case 3:
                return "Wed";

            case 4:
                return "Thu";

            case 5:
                return "Fri";

            case 6:
                return "Sat";

            case 0:
                return "Sun";

            default:
                return "";
        }
    }

    public TimeSpan GetGameTime()
    {
        //returns timespan value
        TimeSpan gameTime = new TimeSpan(gameHour, gameMinute, gameSecond);
        return gameTime;
    }

    //TODO: Remove
    ///<summary>
	///Advance 1 game minute
	/// </summary>

    public void TestAdvanceGameMinute()
    {
        for(int i =0; i<60; i++)
        {
            UpdateGameSecond();
        }
    }

    //TODO: Remove
    ///<summary>
	///Advance 1 game day
	/// </summary>

    public void TestAdvanceGameDay()
    {
        for(int i = 0; i<86400; i++)
        {
            //want everything in the gameseconds method to runs
            UpdateGameSecond();
        }
    }

    public void ISaveableRegister()
    {
        //add to the list
        SaveLoadManager.Instance.iSaveableObjectList.Add(this);
    }
    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
    }

    public GameObjectSave ISaveableSave()
    {
        //delete existing scnee save if it exists
        GameObjectSave.sceneData.Remove(Settings.PersistentScene);

        //create new save scene
        SceneSave sceneSave = new SceneSave();

        //create new int dictionary
        sceneSave.intDictionary = new Dictionary<string, int>();

        //create new string dictionary
        sceneSave.stringDictionary = new Dictionary<string, string>();

        //add values to the int dictionary
        //key with the string
        sceneSave.intDictionary.Add("gameYear", gameYear);
        sceneSave.intDictionary.Add("gameDay", gameDay);
        sceneSave.intDictionary.Add("gameHour", gameHour);
        sceneSave.intDictionary.Add("gameMinute", gameMinute);
        sceneSave.intDictionary.Add("gameSecond", gameSecond);

        //add values to the string dictionary
        sceneSave.stringDictionary.Add("gameDayOfWeek", gameDayOfWeek);
        sceneSave.stringDictionary.Add("gameSeason", gameSeason.ToString());

        //add scene save to game object for persistent scene
        GameObjectSave.sceneData.Add(Settings.PersistentScene, sceneSave);

//serialize and return to files fool
        return GameObjectSave;
    }

    public void ISaveableLoad(GameSave gameSave)
    {
        //get saved game object from game save data
        //see if we have a value related to that game object, put it in the save
        if(gameSave.gameObjectData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            //set to be the retrieved value
            GameObjectSave = gameObjectSave;

            // get save scene data for game object
            if(GameObjectSave.sceneData.TryGetValue(Settings.PersistentScene, out SceneSave sceneSave))
            {
                //if int and string dictionarys are found
                if(sceneSave.intDictionary != null && sceneSave.stringDictionary != null)
                {
                    //populate saved int values
                    //using string keys
                    if(sceneSave.intDictionary.TryGetValue("gameYear", out int savedGameYear))
                        gameYear = savedGameYear;
                    if(sceneSave.intDictionary.TryGetValue("gameDay", out int savedGameDay))
                        gameDay = savedGameDay;
                    if(sceneSave.intDictionary.TryGetValue("gameHour", out int savedGameHour))
                        gameHour = savedGameHour;
                    if(sceneSave.intDictionary.TryGetValue("gameMinute", out int savedGameMinute))
                        gameMinute = savedGameMinute;
                    if(sceneSave.intDictionary.TryGetValue("gameSecond", out int savedGameSecond))
                        gameSecond = savedGameSecond;

                    //populate string of saved values
                    if(sceneSave.stringDictionary.TryGetValue("gameDayOfWeek", out string savedGameDayOfWeek))
                        gameDayOfWeek = savedGameDayOfWeek;

                    //convert back to the season enum after getting the string back
                    if(sceneSave.stringDictionary.TryGetValue("gameSeason", out string savedGameSeason))
                    {
                        //return enum that matches it
                        if(Enum.TryParse<Season>(savedGameSeason, out Season season))
                        {
                            gameSeason = season;
                        }
                    }

                    //zero game tick
                    gameTick = 0f;

                    //trigger advance minute event
                    //update clock in gui
                    EventHandler.CallAdvanceGameMinuteEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);

                    //refresh game clock

                }
            }
        }
    }
    public void ISaveableStoreScene(string sceneName)
    {
        //nothign required here since the time manager is running on the persistant scene
    }
    public void ISaveableRestoreScene(string sceneName)
    {
        //nothing required since manager is running in the persistant scene 
    }
}

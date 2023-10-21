using UnityEngine;

public class GameManager : SingletonMonobehaviour<GameManager>
{

    public Weather currentWeather;

    //start is called before the first frame is called
    protected override void Awake()
    {
        base.Awake();

        //TODO: Need a resolution settings option screen
        Screen.SetResolution(1440, 900, FullScreenMode.FullScreenWindow, 0);
        //was 1920 X 1080

        //set starting weather
        currentWeather = Weather.dry;
        
    }
}

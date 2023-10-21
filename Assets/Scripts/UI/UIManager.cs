using UnityEngine.UI;
using UnityEngine;

public class UIManager : SingletonMonobehaviour<UIManager>
{
    //set up as a property boolean value for on or off
    private bool _pauseMenuOn = false;
    //set to be pause menu game object
    [SerializeField] private UIInventoryBar uiInventoryBar = null;
    [SerializeField] private PauseMenuInventoryManagement pauseMenuInventoryManagement = null;
    [SerializeField] private GameObject pauseMenu = null;
    //populated with the game object tabs with the functions on them, not the buttons
    [SerializeField] private GameObject[] menuTabs = null;
    //this is the menu buttons field so that the script can control them
    //allows to manipulate colors as clicked and unclicked
    [SerializeField] private Button[] menuButtons = null;


    public bool PauseMenuOn {get => _pauseMenuOn; set => _pauseMenuOn = value;}

//want the base class awake method but then set the pause menu to be inactive
    protected override void Awake()
    {
        base.Awake();

        pauseMenu.SetActive(false);
    }

    //update is called every frame
    private void Update()
    {
        //calls the pause menu
        PauseMenu();
    }

    private void PauseMenu()
    {
        //toggle pause menu if escape is pressed
        //looks at the current state of things
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            //if on, disable
            if(PauseMenuOn)
            {
                DisablePauseMenu();
            }
            //else disable
            else
            {
                EnablePauseMenu();
            }
        }
    }

    private void EnablePauseMenu()
    {
        //destroy any currently dragged items 
        uiInventoryBar.DestroyCurrentlyDraggedItems();

        //clear currently selected items
        uiInventoryBar.ClearCurrentlySelectedItems(); 
        //set on
        PauseMenuOn = true;
        //disable player input so the person cant just run around
        Player.Instance.PlayerInputIsDisabled = true;
        //disable time so that the game stops, also stops the update methods
        //freezes the game
        Time.timeScale = 0;
        //set the canvas active
        pauseMenu.SetActive(true);

        //trigger garbage collector
        System.GC.Collect();

        //Highlight selected button
        HighlightButtonForSelectedTab();
    }

    public void DisablePauseMenu()
    {
        //destroy any currently dragged items
        pauseMenuInventoryManagement.DestroyCurrentlyDraggedItems();
        //turns the pause menu off
        PauseMenuOn = false;
        //enables the player input
        Player.Instance.PlayerInputIsDisabled = false;
        //re-enables the game
        Time.timeScale = 1;
        //turns off the pause menu
        pauseMenu.SetActive(false);
    }

//loops through all menu tabs and looks to see if each one is active
    private void HighlightButtonForSelectedTab()
    {
        for(int i =0; i < menuTabs.Length; i++)
        {
            //current active state of the object
            if(menuTabs[i].activeSelf)
            {
                SetButtonColorToActive(menuButtons[i]);
            }
            else
            {
                SetButtonColorToInactive(menuButtons[i]);
            }
        }
    }

//pass in the button
//colors in inspector are stored in the data blocok
//retrieve the buttons from the block and set normal color to pressed color
//then set back to the temp color
    private void SetButtonColorToActive(Button button)
    {
        ColorBlock colors = button.colors;
        colors.normalColor = colors.pressedColor;
        button.colors = colors;
    }

    private void  SetButtonColorToInactive(Button button)
    {
        //slightly greyed out color but otherwise the same
        ColorBlock colors = button.colors;
        colors.normalColor = colors.disabledColor;
        button.colors = colors;
    }

//call from the onclick method
    public void SwitchPauseMenuTab(int tabNum)
    {
        for(int i = 0; i < menuTabs.Length; i++)
        {
            //if the number passed in is not the current number then its not the active tab
            if(i != tabNum)
            {
                menuTabs[i].SetActive(false);
            }
            else
            {
                menuTabs[i].SetActive(true);
            }
        }
        //set the highlights for the tab
        HighlightButtonForSelectedTab();
    }

    public void QuitGame()
    {
        //executes quit method
        Application.Quit();
    }


}


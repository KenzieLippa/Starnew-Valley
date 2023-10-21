using System;
using System.Collections;
//allows for loading and unloading classes
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class SceneControllerManager : SingletonMonobehaviour<SceneControllerManager>
{
    //will be true to start fading and false when not
    private bool isFading;
    //how long to fade from transparent to black
    [SerializeField] private float fadeDuration = 1f;
    //link with the group created in inspector
    [SerializeField] private CanvasGroup faderCanvasGroup = null;
    //link in inspector to
    [SerializeField] private Image faderImage = null;
    public SceneName startingSceneName;

    //if 1 then the black image obscures the screen
    //if 0 then the black image is invisable
    private IEnumerator Fade(float finalAlpha)
    {
        //set the fading flag to true so the FadeSwitchesScene coroutine wont be called again
        isFading = true;

        //Make sure the canvasGroup raycasts into the scene so no more input can be accepted
        faderCanvasGroup.blocksRaycasts = true;

        //Calculate how fast the canvasGroup should fade based on its current alpha, its final alpha and how long it has to change between the two
        //distance between divided by time
        float fadeSpeed = Mathf.Abs(faderCanvasGroup.alpha - finalAlpha) / fadeDuration;

        //while the canvas group hasnt reached the final alpha yet
        //not always exact because of float point math
        while(!Mathf.Approximately(faderCanvasGroup.alpha, finalAlpha))
        {
            //move alpha towards its target alpha
            faderCanvasGroup.alpha = Mathf.MoveTowards(faderCanvasGroup.alpha, finalAlpha, fadeSpeed * Time.deltaTime);

            //wait for a frame then continue
            yield return null;
        }

        //set the flag to false because the fading has finished
        isFading = false;

        //stop blocking the raycasts because the fade is done
        faderCanvasGroup.blocksRaycasts = false;
    }

    //coroutine where the building blocks of the script are put together
    //allow to break execution between frames
    //Ienumerators are for code routines
    private IEnumerator FadeAndSwitchScenes(string sceneName, Vector3 spawnPosition)
    {
        //call before scene unload fade out event
        //tell the event handler that this will be triggered
        EventHandler.CallBeforeSceneUnloadFadeOutEvent();

        //start fading to black and wait for it to finish before continuing
        //yield return hold until the fade coroutine has finished

        yield return StartCoroutine(Fade(1f));

        //store scene data
        SaveLoadManager.Instance.StoreCurrentSceneData();

        //set player position

        Player.Instance.gameObject.transform.position = spawnPosition;

        //call before scene unload event
        EventHandler.CallBeforeSceneUnloadEvent();

        //Unload the current active scene
        //wait until the built in methods have finished
        yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);

        //Start loading the given scene and wait for it to finish
        yield return StartCoroutine(LoadSceneAndSetActive(sceneName));

        //Call after scene load event
        EventHandler.CallAfterSceneLoadEvent();

        //restore the new scene data
        SaveLoadManager.Instance.RestoreCurrentSceneData();

        //Start fading back in and wait for it to finish before exiting the function
        yield return StartCoroutine(Fade(0f));

        //Call after scene load fade in event
        EventHandler.CallAfterSceneLoadFadeInEvent();


    }
    private IEnumerator LoadSceneAndSetActive(string sceneName)
    {
        //allow the given scene to load over several frames and add it to the already loaded scene (just the persistant scene so far)
        //load in addition to the persistant scene that already exsists
        //wait to be done
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        //find the scene that was most recently loaded (the one at the last index of the loaded scene)
        //newly loaded scene is always sceneCount -1
        Scene newlyLoadedScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);

        //set the newly loaded scene as the active scene(this marks it as the one to be unloaded next)
        SceneManager.SetActiveScene(newlyLoadedScene);
    }

    private IEnumerator Start()
    {
        //set initial alpha to start off with a black screen
        faderImage.color = new Color(0f, 0f, 0f, 1f);
        //makes it fully visable
        faderCanvasGroup.alpha = 1f;

        //Start the first scene loading and wait for it to finish
        yield return StartCoroutine(LoadSceneAndSetActive(startingSceneName.ToString()));

        //if this event has any subscribers call them
        EventHandler.CallAfterSceneLoadEvent();

        SaveLoadManager.Instance.RestoreCurrentSceneData();

        //once the scene is finished loading, start fading in
        StartCoroutine(Fade(0f));
    }

    //this will be an external point of contact and influence from the rest of the project
    //called every time the player wants to leave the scene

    //name to switch to and a position to spawn the player in
    public void FadeAndLoadScene(string sceneName, Vector3 spawnPosition)
    {
        //if fading isnt happening then start fading and switch scenes
        if (!isFading)
        {
            //calls a coroutine to fade and switch scenes
            StartCoroutine(FadeAndSwitchScenes(sceneName, spawnPosition));
        }
    }
}

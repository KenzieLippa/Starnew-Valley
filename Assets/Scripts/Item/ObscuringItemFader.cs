using System.Collections;

using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]

public class ObscuringItemFader : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        //seems that the <> are for the types
    }
    public void FadeOut()
    {
        //call a code routine, trigger this behaviour and then it will run for a frame, fade it out and then yeild back
        //split the execution and then show a finished thing with a yeild or return statement
        StartCoroutine(FadeOutRoutine());
    }
    public void FadeIn()
    {
        StartCoroutine(FadeInRoutine());

    }
    private IEnumerator FadeInRoutine()
    {
        float currentAlpha = spriteRenderer.color.a;
        float distance = 1f - currentAlpha;
        while(1f - currentAlpha > 0.01f)
        {
            currentAlpha = currentAlpha + distance / Settings.fadeInSeconds * Time.deltaTime;
            spriteRenderer.color = new Color(1f, 1f, 1f, currentAlpha);
            yield return null;
        }
        spriteRenderer.color = new Color(1f, 1f, 1f, 1f); 
    }
    private IEnumerator FadeOutRoutine()
    {
        //capture the current value of the sprite value
        float currentAlpha = spriteRenderer.color.a;
        //calculate distance between current alpha and target alpha
        float distance = currentAlpha - Settings.targetAlpha;
        //change on a frame by frame basis
        //amount left to fade
        while(currentAlpha - Settings.targetAlpha > 0.01f)
        {
            currentAlpha = currentAlpha - distance / Settings.fadeOutSeconds * Time.deltaTime;
            spriteRenderer.color = new Color(1f, 1f, 1f, currentAlpha);
            //wont tint the sprite but will change the alpha
            yield return null;
            //basically pause the while loop and return nothing
        }
        spriteRenderer.color = new Color(1f, 1f, 1f, Settings.targetAlpha);
    }
}

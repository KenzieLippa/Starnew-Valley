using System.Collections;

using UnityEngine;

public class ItemNudge : MonoBehaviour
{
    //attached as a component to items, items will have a box collider 2d as a trigger, when collided
    //trigger methods and move the object
    //work out whether to right or left and start coroutine to rotate clocwise or otherwise
    //gradually rotate the object
    private WaitForSeconds pause;
    //defining a new variable type

    private bool isAnimating = false;

    //once starts or stops then set it to true, dont animate two
    private void Awake()
    {
        pause = new WaitForSeconds(0.04f);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        //only want to process if not already animating
        if(isAnimating == false)
        {
            //relation to the players collider entering the field
            if(gameObject.transform.position.x < collision.gameObject.transform.position.x)
            {
                StartCoroutine(RotateAntiClock());

            }
            else
            {
                StartCoroutine(RotateClock());
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(isAnimating == false)
        {
            if(gameObject.transform.position.x < collision.gameObject.transform.position.x)
            {
                StartCoroutine(RotateAntiClock());
            }
            else
            {
                StartCoroutine(RotateClock());
            }
        }
    }
    private IEnumerator RotateAntiClock()
    {
        isAnimating = true;

        //do the rotation
        for (int i = 0; i < 4; i++)
        {
            gameObject.transform.GetChild(0).Rotate(0f, 0f, 2f);
            //a slight pause for 0.4 seconds which was set earlier in the awake method
            //
            yield return pause;

        }
        for (int i = 0; i < 5; i++)
        {
            gameObject.transform.GetChild(0).Rotate(0f, 0f, -2f);
            yield return pause;
        }
        gameObject.transform.GetChild(0).Rotate(0f, 0f, 2f);
        yield return pause;
        isAnimating = false;
      
    }
    private IEnumerator RotateClock()
    {
        isAnimating = true;
        for (int i = 0; i < 4; i++)
        {
            gameObject.transform.GetChild(0).Rotate(0f, 0f, -2f);
            yield return pause;
        }
        for (int i = 0; i < 5; i++)
        {
            gameObject.transform.GetChild(0).Rotate(0f, 0f, 2f);
            yield return pause;
        }
        gameObject.transform.GetChild(0).Rotate(0f, 0f, -2f);
        yield return pause;
        isAnimating = false; 
    }
}

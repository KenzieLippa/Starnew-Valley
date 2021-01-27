
using UnityEngine;

public class TriggerObscuringItemFader : MonoBehaviour
{
   //attached to player that already has a collider
   //when player collides with something it triggers the obscuring fader script
   //for going in range of the colider
   private void OnTriggerEnter2D(Collider2D collision)
    {
        //get the game object collided with then get all the components (and children) for fading in and out and trigger the fade out
        //create an array with obscuring fader components
        ObscuringItemFader[] obscuringItemFader = collision.gameObject.GetComponentsInChildren<ObscuringItemFader>();
        //returns a bunch of fader components after looking at the parent

        //next look at the length of the array to see if theres anything added to it
        if (obscuringItemFader.Length > 0)
        {
            for (int i = 0; i<obscuringItemFader.Length; i++)
            {
                //to loop through baby
                obscuringItemFader[i].FadeOut();
                //for every obscuring fader  component that we've found we will trigger the component
                //this will also trigger the script attached to it for the fade out function
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        //get the game object we have collided with then get all the obscuring item fader components on it and its children and trigger the fade in
        ObscuringItemFader[] obscuringItemFader = collision.gameObject.GetComponentsInChildren<ObscuringItemFader>();
        if(obscuringItemFader.Length > 0)
        {
            for (int i = 0; i < obscuringItemFader.Length; i++)
            {
                obscuringItemFader[i].FadeIn();
                
            }


        }
    }
}

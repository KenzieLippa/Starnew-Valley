using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//need a box collider 2d always for this
[RequireComponent(typeof(BoxCollider2D))]
public class SceneTeleport : MonoBehaviour
{
    //first is the name of the scene we want to go to
    [SerializeField] private SceneName sceneNameGoto = SceneName.Scene1_Farm;
    [SerializeField] private Vector3 scenePositionGoto = new Vector3();

  private void OnTriggerStay2D(Collider2D collision)
    {
        //expect player to be the collision object
        //player walks into the teleporter
        //then make sure that the player activated the collsion
        Player player = collision.GetComponent<Player>();

        if(player != null)
        {
            //calc new position
            //its a bastard ternerary
            //check to see if its true, first yes, second no
            //if the position is 0 and not specified then dont use it, if it is specified then use it
            float xPosition = Mathf.Approximately(scenePositionGoto.x, 0f) ? player.transform.position.x : scenePositionGoto.x;

            float yPosition = Mathf.Approximately(scenePositionGoto.y, 0f) ? player.transform.position.y : scenePositionGoto.y;

            float zPosition = 0f;


            //teleporting functionately
            //because singleton can be accessed with instance
            //first parameter is scene to go to and the second was the players spawn position
            //create new vector based on whats been calculated
            SceneControllerManager.Instance.FadeAndLoadScene(sceneNameGoto.ToString(), new Vector3(xPosition, yPosition, zPosition));
        }
    }
}

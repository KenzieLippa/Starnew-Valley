﻿
using UnityEngine;
using Cinemachine;

public class SwitchConfineBoundingShape : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SwitchBoundingShape();
    }

    /// <summary>
	/// Switch the colider that cinemachine uses to define the screen
	/// </summary>
    private void SwitchBoundingShape()
    {
        //get the pollygon colider on the cinemachine object that is used to define the bounds
        PolygonCollider2D polygonCollider2D = GameObject.FindGameObjectWithTag(Tags.BoundsConfiner).GetComponent<PolygonCollider2D>();
        //finds that game object
        //get confiner component
        CinemachineConfiner cinemachineConfiner = GetComponent<CinemachineConfiner>();
        cinemachineConfiner.m_BoundingShape2D = polygonCollider2D;

        //clear the cache by accessing confiner
        cinemachineConfiner.InvalidatePathCache();
        //clears and reapplys

            
    }

}


using System.Collections.Generic;
using UnityEngine;

public class HelperMethods 
{



	///<summary>
	///Gets component of type T at position to check. Returns true if at least one is found and the found components are returned in the
	///componentAtPositionList
	/// </summary>

	//outputs a list of the item types that it finds at the location
	//list that we will return the parameters in
	//point to check
	public static bool GetComponentsAtCursorLocation<T>(out List<T> componentsAtPositionList, Vector3 positionToCheck)
    {
		//if find any in the position then make true
		bool found = false;

		//T is for the generic item type so multiple types of items can use this
		List<T> componentList = new List<T>();

		//overlap point all gets a list of colliders that overlap a point in space
		//colliders that are returned are orderd by z coords
		//returns an array of colliders
		Collider2D[] collider2DArray = Physics2D.OverlapPointAll(positionToCheck);

		//loop through all colliders to get an object of type T

		//loop through all the items found to see if any are of type T or the original item passed through
		T tComponent = default(T);

		//loop through iterations
		for(int i = 0; i < collider2DArray.Length; i++)
        {
			//value in the parent component
			tComponent = collider2DArray[i].gameObject.GetComponentInParent<T>();
			//check for the child component
			if(tComponent != null)
            {
				found = true;
				componentList.Add(tComponent);

            }
            else
            {
				tComponent = collider2DArray[i].gameObject.GetComponentInChildren<T>();
				if(tComponent != null)
                {
					found = true;
					componentList.Add(tComponent);
                }
            }
        }
		//add the component to the list
		componentsAtPositionList = componentList;

		//return if found
		return found;
    }


    ///<summary>
	/// Gets components of type T at box with center point and size and angle.
	/// Returns true if at least one found and the found components are returned in the list
	/// </summary>

	//dont need to instantiate to access
	//angle of the box we wouldnt really want angled so we put in 0
	public static bool GetComponentsAtBoxLocation<T>(out List<T> listComponentsAtBoxPosition, Vector2 point, Vector2 size, float angle)
    {
		//did we find anything in the box area we found
		bool found = false;
		//look for the specific component type
		//T is a type generic which means u can take any type and pass it in
		List<T> componentList = new List<T>();

		//returns an array of 2d colliders within the box defined
		//vector2 point, vector2 size (size of cursor)
		//any bounds will be added
		Collider2D[] collider2DArray = Physics2D.OverlapBoxAll(point, size, angle);

		//loop through all colliders to get an object of type T
		for(int i = 0; i<collider2DArray.Length; i++)
        {
			//does it have any componenets
			//get any components in parents and game object
			T tComponent = collider2DArray[i].gameObject.GetComponentInParent<T>();
			if(tComponent != null)
            {
				found = true;
				//add to list
				componentList.Add(tComponent);
            }
            else
            {
				//check children if parents dont have one
				tComponent = collider2DArray[i].gameObject.GetComponentInChildren<T>();
				if(tComponent != null)
                {
					found = true;
					componentList.Add(tComponent);
                }
            }

        }
		//pass into the out list
		//list will be passed back out with the out parameter so the calling method can query the parameter and use the list
		listComponentsAtBoxPosition = componentList;

		return found;
    }

	///<summary>
	///Returns array components of type T at box with centre point and size and angle. the numberOfCollidersToTest for is passed as a parameter
	///found componenets are returned in the array
	/// </summary>

	//generic function
	//center point of the box, then the size of the box, then an angle, which we dont want
	public static T[] GetComponentsAtBoxLocationNonAlloc<T>(int numberOfCollidersToTest, Vector2 point, Vector2 size, float angle)
    {
		//gets a list of colliders within a box location
		//returns numbers placed in a box array
		//results returned in an array
		//not resized without enough elements and no info is collected so there is no trash
		//set an array of type collider2D
		//return any colliders we find
		Collider2D[] collider2DArray = new Collider2D[numberOfCollidersToTest];
		//pass in parameters
		Physics2D.OverlapBoxNonAlloc(point, size, angle, collider2DArray);

		//new component array
		T tComponent = default(T);
		T[] componenetArray = new T[collider2DArray.Length];

		//loop through the loop and for each object
		//get game object collider sits on, if not null then add to the array
		for (int i = collider2DArray.Length - 1; i >= 0; i-- )
		{
			if(collider2DArray[i] != null)
            {
				tComponent = collider2DArray[i].gameObject.GetComponent<T>();
				if(tComponent != null)
                {
					componenetArray[i] = tComponent;
                }
            }
        }
		return componenetArray;
    }
}

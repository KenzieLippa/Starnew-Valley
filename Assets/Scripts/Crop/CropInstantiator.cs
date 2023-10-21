using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// attach to a crop prefab to set the values in the grid property dictionary
///</summary>

public class CropInstantiator : MonoBehaviour
{
    private Grid grid;
    //need a method on the grid

    //allows us to specify things about the crop that will be instaniwted
    //seed item code, growth days, once has been triggered it will set these values in the grid properties manager
    [SerializeField] private int daysSinceDug = -1;
    [SerializeField] private int daysSinceWatered = -1;
    [ItemCodeDescription]
    [SerializeField] private int seedItemCode = 0;
    [SerializeField] private int growthDays = 0;

    private void OnDisable() 
    {
       EventHandler.InstantiateCropPrefabsEvent -= InstantiateCropPrefab;
    }

    private void OnEnable()
    {
        //link to this method
        EventHandler.InstantiateCropPrefabsEvent +=InstantiateCropPrefab;
    }

    private void InstantiateCropPrefab()
    {
        //get grid gameobject
        grid = GameObject.FindObjectOfType<Grid>();

        //get grid position for crop
        Vector3Int cropGridPosition = grid.WorldToCell(transform.position);

        //set crop grid properties
        SetCropGridProperties(cropGridPosition);

        //destroy game object
        Destroy(gameObject);

    }

    private void SetCropGridProperties(Vector3Int cropGridPosition)
    {
        //passes in crop grid position then looks for the seed item component
        if(seedItemCode > 0)
        {
            GridPropertyDetails gridPropertyDetails;

            gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(cropGridPosition.x, cropGridPosition.y);
//is there a grid property detail for this specific thing?
            if(gridPropertyDetails == null)
            {
                gridPropertyDetails = new GridPropertyDetails();

            }
            //set up member field for the grid property details, set it to what was specified in the inspector
            gridPropertyDetails.daysSinceDug = daysSinceDug;
            gridPropertyDetails.daysSinceWatered = daysSinceWatered;
            gridPropertyDetails.seedItemCode = seedItemCode;
            gridPropertyDetails.growthDays = growthDays;
//set those grid properties
            GridPropertiesManager.Instance.SetGridPropertyDetails(cropGridPosition.x, cropGridPosition.y, gridPropertyDetails);
        }
    }
  
}

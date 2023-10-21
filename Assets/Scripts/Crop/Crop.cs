using System.Collections;

using UnityEngine;

//stub class to start
public class Crop : MonoBehaviour
{
    //member variable for harvest action
    private int harvestActionCount = 0;
    [Tooltip("This should be populated from child transform gameObject showing harvest effect spawn point")]
    //point where the harvest effect comes in 
    [SerializeField] private Transform harvestActionEffectTransform = null;

//allow you to give a tool tip when hovered over in the inspector
    [Tooltip("This should be populated from a child game object")]
    [SerializeField] private SpriteRenderer cropHarvestedSpriteRenderer = null;
 
    [HideInInspector]
    public Vector2Int cropGridPosition;

//takes in equipped item details
//already have x and y of crop
//add in boolean properties that match the animation controller
    public void ProcessToolAction(ItemDetails equippedItemDetails, bool isToolRight, bool isToolLeft, bool isToolDown, bool isToolUp)
    {
        //grid property details
        //get x and y grid property details
        GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(cropGridPosition.x, cropGridPosition.y);
//if this doesnt exsist then dont bother
        if(gridPropertyDetails == null)
        {
            return;
        }

        //get seed item details
        //pass in item code on the planted seed, if not then carry on
        ItemDetails seedItemDetails = InventoryManager.Instance.GetItemDetails(gridPropertyDetails.seedItemCode);
        if(seedItemDetails == null)
        return;

        //get crop details
        //use the method on gridproperties and pass in seed item code
        //if not return imediately
        CropDetails cropDetails = GridPropertiesManager.Instance.GetCropDetails(seedItemDetails.itemCode);
        if (cropDetails == null)
        return;

//get animator if present
        Animator animator = GetComponentInChildren<Animator>();

        //trigger tool animation
        if(animator != null)
        {
            //Debug.Log("I made it here");
            if(isToolRight || isToolUp)
            {
                //setting animation triggers for the animation controllers
                animator.SetTrigger("usetoolright");
            }
            else if(isToolLeft || isToolDown)
            {
                animator.SetTrigger("usetoolleft");
            }
        }

        //trigger tool particle effect on crop
        //see if theres a harvest action effect
        if(cropDetails.isHarvestActionEffect)
        {
            //calls the event and then passes the position and which effect it is
            EventHandler.CallHarvestActionEffectEvent(harvestActionEffectTransform.position, cropDetails.harvestActionEffect);
        }
        

        //get required harvest action for tool
        //if there is a required harvest action then it returns it if not then the tool cant be used
        int requiredHarvestActions = cropDetails.RequiredHarvestActionsForTool(equippedItemDetails.itemCode);
        if(requiredHarvestActions == -1)
        return;//tool cant be used to harvest this crop

        //incrament harvest action count
        //valid seed
        //cant get in unless valid
        harvestActionCount += 1;

        //check if required harvest actions made
        if(harvestActionCount >= requiredHarvestActions)
            HarvestCrop(isToolRight, isToolUp, cropDetails, gridPropertyDetails, animator);
        
    }

    private void HarvestCrop(bool isUsingToolRight, bool isUsingToolUp, CropDetails cropDetails, GridPropertyDetails gridPropertyDetails, Animator animator)
    {
        // is there a harvested animation
        if(cropDetails.isHarvestedAnimation && animator != null)
        {
            //preform animation
            //if harvest sprite then add to sprite renderer
            if(cropDetails.harvestedSprite != null)
            {
                //if theres one then set the sprite
                if(cropHarvestedSpriteRenderer != null)
                {
                    cropHarvestedSpriteRenderer.sprite = cropDetails.harvestedSprite;
                }
            }

            //set animation triggers based on the two options
            if(isUsingToolRight || isUsingToolUp)
            {
                animator.SetTrigger("harvestright");
            }
            else
            {
                animator.SetTrigger("harvestleft");
            }
        }
        //delete crop from grid properties
        //reset all parameters
        //no seeds here any more
        gridPropertyDetails.seedItemCode = -1;
        gridPropertyDetails.growthDays = -1;
        gridPropertyDetails.daysSinceLastHarvest = -1;
        gridPropertyDetails.daysSinceWatered = -1;

        //should the crop be hidden before the animation
        //hide the parsnip as it comes up
        if(cropDetails.hideCropBeforeHarvestedAnimation)
        {
            //get the component of the sprite renderer and disable so it cant be seen
            GetComponentInChildren<SpriteRenderer>().enabled = false;
        }

        //check if the booleans been set
        //should box colliders be disabled on harvest
        if(cropDetails.disableCropCollidersBeforeHarvestedAnimation)
        {
            //disable any box colliders
            //load into an array of colliders
            Collider2D[] collider2Ds = GetComponentsInChildren<Collider2D>();
            
            foreach(Collider2D collider2D in collider2Ds)
            {
                collider2D.enabled = false;
            }
        }

        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);
//is there a harvest animation and animator - destroy crop game object after animation completed
        if(cropDetails.isHarvestedAnimation && animator != null)
        {
            StartCoroutine(ProcessHarvestActionsAfterAnimation(cropDetails, gridPropertyDetails, animator));
        }
        else{

//passed in aas parameters
            HarvestActions(cropDetails, gridPropertyDetails);
        }
    }

    private IEnumerator ProcessHarvestActionsAfterAnimation(CropDetails cropDetails, GridPropertyDetails gridPropertyDetails, Animator animator)
    {
        //check if the harvested state has been reached yet in the animator
        while(!animator.GetCurrentAnimatorStateInfo(0).IsName("Harvested"))
        {
            //let it play
            yield return null;
        }
        HarvestActions(cropDetails, gridPropertyDetails);
    }
    private void HarvestActions(CropDetails cropDetails, GridPropertyDetails gridPropertyDetails)
    {
        //spawn what ever was harvested
        SpawnHarvestedItems(cropDetails);

//test if the code is greater than 0 which means its been specified
        if(cropDetails.harvestedTransformItemCode > 0)
        {
            //pass in a method to transform the crop
            CreateHarvestedTransformCrop(cropDetails, gridPropertyDetails);
        }
        //destroy the crop
        Destroy(gameObject); 
    }

    private void SpawnHarvestedItems(CropDetails cropDetails)
    {
        //spawn the items to be produced
        //loop through the number of items in the array
        for(int i = 0; i < cropDetails.cropProducedItemCode.Length; i++)
        {
            int cropsToProduce;
            //calculate how many crops to produce
            if(cropDetails.cropProducedMinQuantity[i] == cropDetails.cropProducedMaxQuantity[i]||
            cropDetails.cropProducedMaxQuantity[i] < cropDetails.cropProducedMinQuantity[i])
            {
                //if there is no difference in the two or somehow the max is lower then just do the minimum
                cropsToProduce = cropDetails.cropProducedMinQuantity[i];
            }
            else
            {
                //random number in the max and min, its exclusive of the edge so you add one to make sure you can achieve it
                cropsToProduce = Random.Range(cropDetails.cropProducedMinQuantity[i], cropDetails.cropProducedMaxQuantity[i]+1);
            }
            for (int j = 0; j < cropsToProduce; j++)
            {
                Vector3 spawnPosition;
                if(cropDetails.spawnCropProducedAtPlayerPosition)
                {
                    //adds to the players inventory
                    //location and item code
                    InventoryManager.Instance.AddItem(InventoryLocation.player, cropDetails.cropProducedItemCode[i]);
                }
                else
                {
                    //random position
                    //create a new spawn position if the first one didnt work
                    
                    spawnPosition = new Vector3(transform.position.x + Random.Range(-1f,1f), transform.position.y + Random.Range(-1f,1f), 0f);
                    SceneItemsManager.Instance.InstantiateSceneItem(cropDetails.cropProducedItemCode[i], spawnPosition);
                }
            }
        }
    }
    private void  CreateHarvestedTransformCrop(CropDetails cropDetails, GridPropertyDetails gridPropertyDetails)
    {
        //update crop in grid properties for transform crop, reset everything
        //seed item code is the new item code
        gridPropertyDetails.seedItemCode = cropDetails.harvestedTransformItemCode;
        gridPropertyDetails.growthDays = 0;
        gridPropertyDetails.daysSinceLastHarvest = -1;
        gridPropertyDetails.daysSinceWatered = -1;

//set new details
        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);

        //display planted crop
        GridPropertiesManager.Instance.DisplayPlantedCrop(gridPropertyDetails);
    }

}

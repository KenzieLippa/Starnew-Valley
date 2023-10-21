
using UnityEngine;

[System.Serializable]
//not on gameobject but on a scriptable object
public class CropDetails 
{
    //in inspector
   [ItemCodeDescription]
   public int seedItemCode; //item code for the corresponding seed
   //grow in a number of stages, will have a number of growth days and then for each element there is a new growth stage
   //this will change the number of days that it takes to grow
   public int[] growthDays; //days growth for each stage
   //sum of all the growth days in the first array
   //public int totalGrowthDays; // total growth days
   //gameobjects assosciated with each stage of growth
   public GameObject[]  growthPrefab; // prefab to use when instantiating growth stages
   //sappling to fully grown tree, a sprite for every growth stage
   public Sprite[] growthSprite; //growth sprite
   //which season the crop grows in if you end up making it more complex later on
   public Season[] seasons; //growth season
   //have a seed and then a grown crop
   //will be the crop that gets harvested
   public Sprite harvestedSprite; //sprite used once harvested
   [ItemCodeDescription]
   //some crops can transform into other crops once harvested, can transform into a stump
   //considered as another crop once its chopped down
   public int harvestedTransformItemCode; //if the item transforms into another item when harvested this item code will be populated
   public bool hideCropBeforeHarvestedAnimation; // if the crop should be disabled before the harvest animation
   public bool disableCropCollidersBeforeHarvestedAnimation; // if colliders on crop should be disabled to avoid the harvested animation
   //effecting any other game objects
   //if a harvest animation plays when its done
   //tree wobble for the tree when chopped
   public bool isHarvestedAnimation; // true if harvested animation to be played on final growth stage prefab
   //if you cut down tree then leaves may fall as your chopping
   public bool isHarvestActionEffect = false; // flag to determine whether there is a harvest action effect
   //crop should be spawned at the players position
   public bool spawnCropProducedAtPlayerPosition; 
//which effect should be played
   public HarvestActionEffect harvestActionEffect;// harvest action effect for the crop

   [ItemCodeDescription]
   //what type of tool you can use to harvest the crop, can also accomidate for better materials
   public int[] harvestToolItemCode; // array of item codes for the tools that can harvest or 0 array elements if not tool required
   //number of harvest actions needed
   public int[] requiredHarvestActions;  // number of harvest actions required for corresponding tool in harvest tool array
   [ItemCodeDescription]
   //array of item codes for the crop, could produce multiple things when harvesting
   public int[] cropProducedItemCode; // array of item codes produced for harvesting crops
   public int[] cropProducedMinQuantity; // array of minimum quanities produced for the minimum crop yield
   public int[] cropProducedMaxQuantity; // if the max quanity is greater than the min quanity then a random number of crops 
   //drops between the two values
   //if you have a bush than it can grow multiple times
   public int[] daysToRegrow; // days to regrow next crop or -1 if its a single use crop

   ///<summary>
   ///returns true if the tool item code can be used to harvest this crop, else return false
   ///</summary>

   public bool CanUseToolToHarvestCrop(int toolItemCode)
   {
        if(RequiredHarvestActionsForTool(toolItemCode)== -1)
        {
            //cant harvest
            return false;
        }
        else{
            //can harvest
            return true;
        }
   }
   ///<summary>
    /// returns -1 if the tool cant be used to harvest the crop, else returns the number of harvest actions required by this tool
   ///<summary>
   public int RequiredHarvestActionsForTool(int toolItemCode)
   {
       for(int i = 0; i < harvestToolItemCode.Length; i++)
       {
           //if found then it returns the harvest action
           if(harvestToolItemCode[i] == toolItemCode)
           {
               return requiredHarvestActions[i];
           }
       }
       //if not then it returns nothing
       return -1;
   }


}

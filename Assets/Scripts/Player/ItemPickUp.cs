
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
   //atached to player, when collide then trigger th method
   private void OnTriggerEnter2D(Collider2D collision)
    {
        //passes in a collision object
        Item item = collision.GetComponent<Item>();
        if(item != null)
        {
            //get from the inventory manager using the item code
            ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(item.ItemCode);
           // Debug.Log(itemDetails.itemDescription);
                //a singleton
                //print to concol

            //if can be picked up then call the manager
            if(itemDetails.canBePickedUp == true)
            {
                //add item to the inventory
                //singleton allows to access the instance
                //pass in the item that was walked over, pass in the game object and what it collided with
                InventoryManager.Instance.AddItem(InventoryLocation.player, item, collision.gameObject);
            }
        }
    }
}

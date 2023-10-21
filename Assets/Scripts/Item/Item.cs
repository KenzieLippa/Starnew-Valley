
using UnityEngine;

public class Item : MonoBehaviour
{
    //apply the thing we just made
    [ItemCodeDescription]
    [SerializeField]
    //serialize shows in inspector and can be updated
    private int _itemCode;


    private SpriteRenderer spriteRenderer;


    public int ItemCode { get { return _itemCode; } set { _itemCode = value; } }
    //getter and setters, the getter gets the value (return will display it) and the setter sets the value with whatever you call the function with

    private void Awake()
    {
        //populate the sprite renderer
        //atached to a game object with a child object with a sprite renderer
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        //cache in the sprite render field
       
    }
    private void Start()
    {
        //check item code, if not 0 then initialize the item
        if (ItemCode != 0)
        {
            Init(ItemCode);
        }
    }

    public void Init(int itemCodeParam)
    {
        //if not 0 then reset the item param and retrieve the item manager
        if (itemCodeParam != 0)
        {
            //not in this lecture
            ItemCode = itemCodeParam;
            ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(ItemCode);
            //singleton can access an instance
            spriteRenderer.sprite = itemDetails.itemSprite;
            if (itemDetails.itemType == ItemType.Reapable_scenary)
            {
                gameObject.AddComponent<ItemNudge>();
                //adds this component automatically to the object
            }
        }

    }
    //add auto the item nudge if of type reapable scenary

}

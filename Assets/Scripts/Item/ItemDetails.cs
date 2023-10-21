
using UnityEngine;

//include in scriptable objects
[System.Serializable] //needs brackets

public class ItemDetails
{
    public int itemCode;
    //unique item identifier for the object
    public ItemType itemType;
    public string itemDescription;
    public Sprite itemSprite;
    public string itemLongDescription; //for item bars etc
    public short itemUseGridRadius;// for distance you can affect the object you want to interact with
    public float itemUseRadius;//for non grid based spacing
    public bool isStartingItem;
    public bool canBePickedUp;
    public bool canBeDropped;
    public bool canBeEaten;
    public bool canBeCarried;// if item clicked on then they will carry it above their head

}

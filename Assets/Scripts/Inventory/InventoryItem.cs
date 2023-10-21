[System.Serializable]
//want to hold inventory when saving
//include two fields, the item code and quantity
//through this we can access the other info and remeber how many the player has
public struct InventoryItem
{
    public int itemCode;
    public int itemQuantity;

}
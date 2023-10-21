[System.Serializable]

//every item will have to have a scene position
public class SceneItem 
{
    //item code 
    public int itemCode;
    //define the position
    public Vector3Serializable position;
    //name of item
    public string itemName;

    //constructor that allows to create an instance of the scene item without parameters
    public SceneItem()
    {
        //create new instance of the position
        position = new Vector3Serializable();
    }
}

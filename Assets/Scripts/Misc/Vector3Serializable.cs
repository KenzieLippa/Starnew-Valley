[System.Serializable]

//standard unity vector 3 cant be serialized and saved to a data file
public class Vector3Serializable 
{
    //for the coordinates
    public float x, y, z;

    //allows you to pass in the values
    public Vector3Serializable(float x, float y, float z)
    {
        //sets the fields to the passed in parameters
        this.x = x;
        this.y = y;
        this.z = z;
    }
    //empty constructor to create an instance with no parameters
    public Vector3Serializable()
    {

    }
}

//im guessing that with methods like this you can access either based on which type of vector3 you would like to make

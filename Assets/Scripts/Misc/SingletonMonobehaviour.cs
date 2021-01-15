
using UnityEngine;

public abstract class SingletonMonoBehavior<T> : MonoBehaviour where T : MonoBehaviour
{
    //t is for generic
    private static T instance;
    //static can be refrenced directly using the class name
    public static T Instance
    {
        get
        {
            return instance;
            // getter
        }

    }


    //wake up method
    protected virtual void Awake()
    {
        //runs when new game object initialized
        //protected means other children can access
        //virtual means children can over ride it
        if(instance == null)
        {
            instance = this as T;
            //if doesnt already exsist, give it what ever was passed in for T

        }
        else
        {
            Destroy(gameObject);
            //prevents duplication
        }
    }
         
   
     
   

}

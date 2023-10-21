
using System.Collections.Generic;
using UnityEngine;

//inherets from singleton monobehaviour
public class PoolManager : SingletonMonobehaviour<PoolManager>
{
    //pool dictionary with the key being a number and the que is for game objects
    //hold number of ques of game objects
   private Dictionary<int, Queue<GameObject>> poolDictionary = new Dictionary<int, Queue<GameObject>>();
   //populated in inspector with refrences of the prefab 
   [SerializeField] private Pool[] pool = null;
   //transform for the pool manager
   //keep objects together
   [SerializeField] private Transform objectPoolTransform = null;

   [System.Serializable]
   //pool struct with a size and a prefab type
   public struct Pool
   {
       public int poolSize;
       public GameObject prefab;
   }
   //loop through number of entries in the array and for each entry then we create a pool for them
   private void Start()
   {
       //Create object pools on start
       for(int i =0; i<pool.Length; i++)
       {
            CreatePool(pool[i].prefab, pool[i].poolSize);
       }
   }

   private void CreatePool(GameObject prefab, int poolSize)
   {
       //key for the dictionary
       int poolKey = prefab.GetInstanceID();
       //prefab name for the dictionary
       string prefabName = prefab.name; //get prefab name
    
    //anchor the pool object based on the prefab
       GameObject parentGameObject = new GameObject(prefabName + "Anchor"); // create parent gameobject to parent child objects to

//set parent to object pool transform
       parentGameObject.transform.SetParent(objectPoolTransform);

//create objects as part of the pool
//see if it doesnt already have a key
       if(!poolDictionary.ContainsKey(poolKey))
       {
           //add new entry with the pool key and create a new queue
            poolDictionary.Add(poolKey, new Queue<GameObject>());
//loop through number of iterations
            for(int i = 0; i<poolSize; i++)
            {
                //create a new object thru instantiating and achor to transform as a child
                GameObject newObject = Instantiate(prefab, parentGameObject.transform) as GameObject;
                //set inactive
                newObject.SetActive(false);
//put to end queu for that particular queu
                poolDictionary[poolKey].Enqueue(newObject);
            }
       }
   }

//call when we want to reuse an object from the pool
//pass in game object that we want to reuse
//set these to be the past parameters
   public GameObject ReuseObject(GameObject prefab, Vector3 position, Quaternion rotation){
       int poolKey = prefab.GetInstanceID();
//see if contains that key
       if(poolDictionary.ContainsKey(poolKey))
       {
           //Get object from pool queue
           GameObject objectToReuse = GetObjectFromPool(poolKey);
//reset all its information once we've found it
           ResetObject(position, rotation, objectToReuse, prefab);
//return the object
           return objectToReuse;
       }
       else
       {
           Debug.Log("No Object pool for "+prefab);
           return null;
       }
   }

   private GameObject GetObjectFromPool(int poolKey)
   {
       //access queue and then deque
       //dictionary contains a number of queues 
       //return a game object of that queue
       GameObject objectToReuse = poolDictionary[poolKey].Dequeue();
       //add that back to the queue currently dealing with 
       poolDictionary[poolKey].Enqueue(objectToReuse);

       //log to consol if object is currently active 
        if(objectToReuse.activeSelf == true)
        {
            //make sure none of them are still on
            objectToReuse.SetActive(false);
        }

        return objectToReuse;
   }

   //resets the object that is going to be reused and resets it to a new position and rotation
   //set back to prefab local scale

   private static void ResetObject(Vector3 position, Quaternion rotation, GameObject objectToReuse, GameObject prefab)
   {
       objectToReuse.transform.position = position;
       objectToReuse.transform.rotation = rotation;


       //objectToReuse.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
       objectToReuse.transform.localScale = prefab.transform.localScale;
   }
}


using UnityEngine;

[ExecuteAlways]
public class GenerateGUID : MonoBehaviour
{
    //run in the editor instead of just running in play mode
    [SerializeField]
    private string _gUID = "";

    //retrieve by its specific key

    public string GUID { get => _gUID; set => _gUID = value; }


    private void Awake()
    {
        //only populate in the editor
        //means you only want it to run in the editor, test if the game is playing before executing the function
        if (!Application.IsPlaying(gameObject))
        {
            //make sure the object has a gaurenteed unique id
            if(_gUID == "")
            {
                // assign GUID
                //generates a random unique 16 digit string that will be the summoning way for the item
                _gUID = System.Guid.NewGuid().ToString();
            }
        }
    }
}

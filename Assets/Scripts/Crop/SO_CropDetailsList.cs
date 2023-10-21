
using System.Collections.Generic;
using UnityEngine;

//default file name and then menu name
[CreateAssetMenu(fileName = "CropDetailsList", menuName = "Scriptable Objects/Crop/Crop Details List")]
public class SO_CropDetailsList : ScriptableObject
{
    [SerializeField]
    //list of the crop details we just wrote
    public List<CropDetails> cropDetails;

    public CropDetails GetCropDetails(int seedItemCode)
    {
        //find the seed item code on the item, return it when its been passed in
        return cropDetails.Find(x => x.seedItemCode == seedItemCode);
    }
}

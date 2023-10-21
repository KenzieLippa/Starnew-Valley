
using System.Collections.Generic;
using UnityEngine;

//directive/ atribute
[CreateAssetMenu(fileName = "so_ItemList", menuName = "Scriptable Objects/Item/Item List")]
//allows to be created through a menu

public class SO_ItemList : ScriptableObject
{
    [SerializeField]
    public List<ItemDetails> itemDetails;
    //give option to make an asset in the unity area to make an object based on this
}

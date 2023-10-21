
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//indicate which property attributes it relates to
[CustomPropertyDrawer(typeof(ItemCodeDescriptionAttribute))]

public class ItemCodeDescriptionDrawer : PropertyDrawer
{
    //retrieve from property and override method for property height, double the space to draw both in
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        //change the returned property to be double and fit all the drawn info
        return EditorGUI.GetPropertyHeight(property) * 2;
        //double the standard height
    }
    //next need on gui method which will actually draw the property
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //begin and end property
        //draws item code and description in the GUI

        EditorGUI.BeginProperty(position, label, property);

        if (property.propertyType == SerializedPropertyType.Integer)
        {
            //this checks to see if it is a serialized intiger from the list of values
            //detects changes or values in the editor
            //starts to check the changed values

            EditorGUI.BeginChangeCheck();
            //draw item code
            //position and then property
            //changed height to be doubled, now need to undo it
            var newValue = EditorGUI.IntField(new Rect(position.x, position.y, position.width, position.height / 2), label, property.intValue);
            //draw the item code as drawn by default in the editor, re-implimented the editor function
            //draw item descritpiton
            //drop this below the item code
            EditorGUI.LabelField(new Rect(position.x, position.y + (position.height / 2), position.width, position.height / 2), "Item Description", GetItemDescription(property.intValue));
            //the editor gui is probably that box down bellow



            //check if value has changed, then set to new value, can change it interactively
            if (EditorGUI.EndChangeCheck())
            {
                //checks to see if anything changed, then can set the property
                property.intValue = newValue;
            }
        }
   



        EditorGUI.EndProperty();
    }

    private string GetItemDescription(int itemCode)
    {
        //refrence the scriptable item list
        //search that list for an item code that matches whats being passed
        //if exsist then return it, if not then empty string
        SO_ItemList so_itemList;
        so_itemList = AssetDatabase.LoadAssetAtPath("Assets/Scriptable Object Assets/Item/so_ItemList.asset",typeof(SO_ItemList)) as SO_ItemList;
        //query files held in asset folder

        //can retrieve assests and load them
        List<ItemDetails> itemDetailsList = so_itemList.itemDetails;
        //from list retrieve one
        //for some reason these things are underlined like they are wrong and yet i know they are not

        ItemDetails itemDetail = itemDetailsList.Find(x => x.itemCode == itemCode);
        //try to find the item detail where this condition is true
        //then test if we've been able to retrieve something
        if(itemDetail != null)
        {
            return itemDetail.itemDescription;

        }
        else
        {
            return "";

        }


    }



}

using System.Collections.Generic;
using UnityEngine;

//sets option in asset menu
[CreateAssetMenu(fileName = "so_SceneRouteList", menuName = "Scriptable Objects/Scene/Scene Route List")]
public class SO_SceneRouteList : ScriptableObject
{
    //list os scene routes, asset created and populated in inspector
   public List<SceneRoute> sceneRouteList;
}

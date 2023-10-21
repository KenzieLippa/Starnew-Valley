using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//creates a menu option in the unity menu to create an object
//composed of an animation clip, a name, a part, the parts color, and the parts type, for this one its the carry variant

[CreateAssetMenu(fileName = "so_AnimationType", menuName = "Scriptable Objects/Animation/Animation Type")]
public class SO_AnimationType : ScriptableObject
{
    //red originally until you turn each into an enum
    //enums will store all possible animations in an array as an index value
    public AnimationClip animationClip;
    public AnimationName animationName;
    public CharacterPartAnimator characterPart;
    public PartVariantColor partVariantColor;
    public PartVariantType partVariantType;

}

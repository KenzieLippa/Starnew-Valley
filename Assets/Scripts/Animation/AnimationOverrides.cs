
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class AnimationOverrides : MonoBehaviour
{
    //can update in the inspector
    //drag player game object into this field and use to find child game objects
    [SerializeField] private GameObject character = null;

    //array of animation types to be updated
    //when creating these scrptable object assets drag them into the array
    [SerializeField] private SO_AnimationType[] soAnimationTypeArray = null;

    //build two dictionarys for refrences to these clips
    //the key is an animationclip so_animationtype
    private Dictionary<AnimationClip, SO_AnimationType> animationTypeDictionaryByAnimation;

    //composite attribute key is the string used for the search method
    private Dictionary<string, SO_AnimationType> animationTypeDictionaryByCompositeAttributeKey;

    // Start is called before the first frame update
    private void Start()
    {
        //Initialize animation type dictionary keyed by animation clip
        animationTypeDictionaryByAnimation = new Dictionary<AnimationClip, SO_AnimationType>();
        //populate with the values of animation
        foreach (SO_AnimationType item in soAnimationTypeArray)
        {
            //animationclip is the key so is the value
            animationTypeDictionaryByAnimation.Add(item.animationClip, item);
        }
        //Initialize animation type dictionary keyed by string
        animationTypeDictionaryByCompositeAttributeKey = new Dictionary<string, SO_AnimationType>();
        foreach(SO_AnimationType item in soAnimationTypeArray)
        {
            //populate with values from the SO array
            //build a different key to make a composite key
            //comes with the part, the color, the type and the name
            //these make it easier to reference
            string key = item.characterPart.ToString() + item.partVariantColor.ToString() + item.partVariantType.ToString() + item.animationName.ToString();
            animationTypeDictionaryByCompositeAttributeKey.Add(key, item);
        }
    }

    //actually swaps the animations
    //set in a list of character attributes based on what you want to switch
    //pass in only one attribute, but you can apply more overrides if you would like when the method is called
    public void ApplyCharacterCustomisationParameters(List<CharacterAttribute> characterAttributesList)
    {
        //Stopwatch s1 = Stopwatch.StartNew();

        //loop through all character attributes and set the animation override controller for each
        //if there are more attributes then search through more
        foreach(CharacterAttribute characterAttribute in characterAttributesList)
        {
            //local animator
            Animator currentAnimator = null;
            //keyvalue pair list with current and desired swap
            List<KeyValuePair<AnimationClip, AnimationClip>> animsKeyValuePairList = new List<KeyValuePair<AnimationClip, AnimationClip >> ();

            //we want to locate the character part animator
            string animatorSOAssetName = characterAttribute.characterPart.ToString();

            //find animators in scene that match SO animator type
            //find all children animators of the parent object
            Animator[] animatorsArray = character.GetComponentsInChildren<Animator>();

            //compare all to the particular name to see which matches
            foreach(Animator animator in animatorsArray)
            {
                if(animator.name == animatorSOAssetName)
                {
                    currentAnimator = animator;
                    break;
                }
            }
            // get base current animations animator
            //define the override with making a new object using the override controller class
            //can get a list of all animations they currently contain
            //AnimationClip stores this and can create a list based on it
            AnimatorOverrideController aoc = new AnimatorOverrideController(currentAnimator.runtimeAnimatorController);
            List<AnimationClip> animationsList = new List<AnimationClip>(aoc.animationClips);

            //process each clip in animations list
            foreach(AnimationClip animationClip in animationsList)
            {
                //find animation in dictionary
                //see if it is one that you would like to swap
                SO_AnimationType so_AnimationType;
                bool foundAnimation = animationTypeDictionaryByAnimation.TryGetValue(animationClip, out so_AnimationType);
                //if it has been found in the first dictionary then you may want to swap it
                if (foundAnimation)
                {
                    string key = characterAttribute.characterPart.ToString() + characterAttribute.partVariantColor.ToString() +
                        characterAttribute.partVariantType.ToString() + so_AnimationType.animationName.ToString();

                    //character part of this, all the things we want to switch to are asked for here
                    SO_AnimationType swapSO_AnimationType;
                    bool foundSwapAnimation = animationTypeDictionaryByCompositeAttributeKey.TryGetValue(key, out swapSO_AnimationType);

                    //want to see if this swap animation exsists
                    if (foundSwapAnimation)
                    {
                        //we have a swap we are interested in
                        AnimationClip swapAnimationClip = swapSO_AnimationType.animationClip;
                        //add both to the keyvalue pairs list
                        animsKeyValuePairList.Add(new KeyValuePair<AnimationClip, AnimationClip>(animationClip, swapAnimationClip));

                    }
                }
            }
            //Apply animation updates to animation override controller and then update the animator with the new controller
            aoc.ApplyOverrides(animsKeyValuePairList);

            //apply to the current animator controller and the runtime animator controller
            currentAnimator.runtimeAnimatorController = aoc;

            //work through this a few times if you need to
            //pass in a list of attributes defining what we want to switch to
            //use the assests that hold more info to see if we can swap
        }

    }
}

using System.Collections;
using UnityEngine;

//singleton game manager
public class VFXManager : SingletonMonobehaviour<VFXManager>
{
    //member variable used to initiate a 2 second pause in a coroutine
    private WaitForSeconds twoSeconds;
    //populate the game object in the inspector
   
    [SerializeField] private GameObject deciduousLeavesFallingPrefab = null;
    [SerializeField] private GameObject pineConesFallingPrefab = null;
    [SerializeField] private GameObject choppingTreeTrunkPrefab = null;
    [SerializeField] private GameObject breakingStonePrefab = null;
    [SerializeField] private GameObject reapingPrefab = null;



    protected override void Awake(){
        //populate member variable
        base.Awake();
        twoSeconds = new WaitForSeconds(2f);
    }
//unsubscribe to it in the on disable
    private void OnDisable() {
        EventHandler.HarvestActionEffectEvent -= displayHarvestActionEffect;

    }
    //subscribe to the harvest action effect

    private void OnEnable(){
        EventHandler.HarvestActionEffectEvent += displayHarvestActionEffect;
    }
    
    //takes game object and wait for seconds and then waits a certain length of time and sets the game object to false
    private IEnumerator DisableHarvestActionEffect(GameObject effectGameObject, WaitForSeconds secondsToWait){
        yield return secondsToWait;
        effectGameObject.SetActive(false);
    }
    //switch on whats passed in, only checking reaping right now
    private void displayHarvestActionEffect(Vector3 effectPosition, HarvestActionEffect harvestActionEffect){
        switch(harvestActionEffect)
        {
            case HarvestActionEffect.deciduousLeavesFalling:
            //pool manager reuse the object and pass in the effect position
                GameObject deciduousLeaveFalling = PoolManager.Instance.ReuseObject(deciduousLeavesFallingPrefab, effectPosition, Quaternion.identity);
                deciduousLeaveFalling.SetActive(true);
                StartCoroutine(DisableHarvestActionEffect(deciduousLeaveFalling, twoSeconds));
                break;

            case HarvestActionEffect.pineConesFalling:
                GameObject pineConesFalling = PoolManager.Instance.ReuseObject(pineConesFallingPrefab, effectPosition, Quaternion.identity);
                pineConesFalling.SetActive(true);
                StartCoroutine(DisableHarvestActionEffect(pineConesFalling, twoSeconds));
                break; 

            case HarvestActionEffect.choppingTreeTrunk:
            //goes to pool manager and reuses the object
            //sets active to true then sets active to true
                GameObject choppingTreeTrunk = PoolManager.Instance.ReuseObject(choppingTreeTrunkPrefab, effectPosition, Quaternion.identity);
                choppingTreeTrunk.SetActive(true);
                StartCoroutine(DisableHarvestActionEffect(choppingTreeTrunk, twoSeconds));
                break;

            case HarvestActionEffect.breakingStone:
            //retrieve from the game object
                GameObject breakingStone = PoolManager.Instance.ReuseObject(breakingStonePrefab, effectPosition, Quaternion.identity);
                breakingStone.SetActive(true);
                StartCoroutine(DisableHarvestActionEffect(breakingStone, twoSeconds));
                break;

            case HarvestActionEffect.reaping:
            //make use of pool manager and the reuse object method to reuse the reaping situation
                GameObject reaping = PoolManager.Instance.ReuseObject(reapingPrefab, effectPosition, Quaternion.identity);
                //set the effect to active (which hopefully was the error from before)
                reaping.SetActive(true);
                //disables the effect after a period of time, pass in the object to set to in active and the period of time
                StartCoroutine(DisableHarvestActionEffect(reaping, twoSeconds));
                break;
            case HarvestActionEffect.none:
                break;
            default:
                break;

        }
    }
}
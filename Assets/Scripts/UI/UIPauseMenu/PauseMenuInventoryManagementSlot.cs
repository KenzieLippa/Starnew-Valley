using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class PauseMenuInventoryManagementSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    //inventory slot image
    public Image inventoryManagementSlotImage;
    //text field to update item quantity
    public TextMeshProUGUI textMeshProUGUI;
    //greyed out image
    public GameObject greyedOutImageGO;
    //will be created later and then attached to a game object to be populated
    [SerializeField] private PauseMenuInventoryManagement inventoryManagement = null;
    //display the item description
    [SerializeField] private GameObject inventoryTextBoxPrefab = null;


    [HideInInspector] public ItemDetails itemDetails;
    [HideInInspector] public int itemQuantity;
    [SerializeField] private int slotNumber = 0;

    //private Vector3 startingPosition;
    //dragged item game object 
    public GameObject draggedItem;
    private Canvas parentCanvas;

    //get the parent canvas
    private void Awake()
    {
        parentCanvas = GetComponentInParent<Canvas>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        //check its not 0 then instantiate a dragged item
        if(itemQuantity != 0)
        {
            //instantiate game object as dragged item
            draggedItem = Instantiate(inventoryManagement.inventoryManagementDraggedItemPrefab, inventoryManagement.transform);

            //get image for dragged item
            Image draggedItemImage = draggedItem.GetComponentInChildren<Image>();
            draggedItemImage.sprite = inventoryManagementSlotImage.sprite;
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        //move game object as dragged item
        if(draggedItem != null)
        {
            //adjust the dragged item to be the same as the mouse 
            draggedItem.transform.position = Input.mousePosition;
        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        //destroy game object as dragged item
        if(draggedItem != null)
        {
            //destroy the dragged item
            Destroy(draggedItem);

            //get object drag is over
            //is it over a slot, use event data to figure this out
            //if this is true then we can drag it there
            if(eventData.pointerCurrentRaycast.gameObject != null && eventData.pointerCurrentRaycast.gameObject.GetComponent<PauseMenuInventoryManagementSlot>() != null)
            {
                //get the slot number where the drag is ended 
                int toSlotNumber = eventData.pointerCurrentRaycast.gameObject.GetComponent<PauseMenuInventoryManagementSlot>().slotNumber;

                //swap inventory items in inventory list
                //put the item in the box with method made already
                InventoryManager.Instance.SwapInventoryItems(InventoryLocation.player, slotNumber, toSlotNumber);

                //destroy inventory text box
                //for swapping reasons
                inventoryManagement.DestroyInventoryTextBoxGameobject();
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //populate text box with item details
        if(itemQuantity != 0)
        {
            //instantiate inventory text box
            inventoryManagement.inventoryTextBoxGameobject = Instantiate(inventoryTextBoxPrefab, transform.position, Quaternion.identity);
            inventoryManagement.inventoryTextBoxGameobject.transform.SetParent(parentCanvas.transform, false);

            UIInventoryTextBox inventoryTextBox = inventoryManagement.inventoryTextBoxGameobject.GetComponent<UIInventoryTextBox>();

            //set item type description
            string itemTypeDescription = InventoryManager.Instance.GetItemTypeDescription(itemDetails.itemType);

            //populate text box
            inventoryTextBox.SetTextboxText(itemDetails.itemDescription, itemTypeDescription, "", itemDetails.itemLongDescription, "","");

            //set textbox position
            if(slotNumber > 23)
            {
                //above or below inventory bar based on position
                inventoryManagement.inventoryTextBoxGameobject.GetComponent<RectTransform>().pivot = new Vector2(0.5f,0f);
                inventoryManagement.inventoryTextBoxGameobject.transform.position = new Vector3(transform.position.x, transform.position.y+50f, transform.position.z);

            }
            else
            {
                inventoryManagement.inventoryTextBoxGameobject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1f);
                inventoryManagement.inventoryTextBoxGameobject.transform.position = new Vector3(transform.position.x, transform.position.y - 50f,transform.position.z); 
            }
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        inventoryManagement.DestroyInventoryTextBoxGameobject();
    }
}

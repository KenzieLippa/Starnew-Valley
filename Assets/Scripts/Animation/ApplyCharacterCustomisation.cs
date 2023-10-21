
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class colorSwap
{
    //2 member variables
    public Color fromColor;
    public Color toColor;

//constructor to set based on what was passed in for instantiation
    public colorSwap(Color fromColor, Color toColor)
    {
        this.fromColor = fromColor;
        this.toColor = toColor;
    }
}

public class ApplyCharacterCustomisation : MonoBehaviour
{
   //input textures
   [Header("Base Textures")]
   [SerializeField] private Texture2D maleFarmerBaseTexture = null;
   //add in later, technically now only male textures available
   [SerializeField] private Texture2D femaleFarmerBaseTexture = null;
   //add new shirts in the editor
   //normally would have a window for that, for this game we will only have it in the editor tho
   [SerializeField] private Texture2D shirtsBaseTexture = null;

//take selection and write to another texture and tint to users color, write to the output texture
//read write enabled to a texture otherwise wont be able to access it
   [SerializeField] private Texture2D hairBaseTexture = null;
   //set female or male based on whats selected
   [SerializeField] private Texture2D hatsBaseTexture = null;
   [SerializeField] private Texture2D adornmentsBaseTexture = null;
   private Texture2D farmerBaseTexture;


   //created textures
   [Header("OutputBase Texture to be used for animation")]
   //used by the animations in the game
   [SerializeField] private Texture2D farmerBaseCustomised = null;
   //all of the shirts on it that fit in with the shirt positions from the base
   //merge on top of skin texture
   //output texture
   [SerializeField] private Texture2D hairCustomised = null;
   [SerializeField] private Texture2D hatsCustomised = null;
   private Texture2D farmerBaseShirtsUpdated;

   //applied in every direction to match the body sprites
   private Texture2D farmerBaseAdornmentsUpdated;
   //achieve the texture shown
   private Texture2D selectedShirt;
   private Texture2D selectedAdornment;

   //input field
   [Header("Select Shirt Style")]
   //range slider, only these two options for rn
   [Range(0,1)]
   [SerializeField] private int inputShirtStyleNo = 0;

   //select hair style
   [Header("Select Hair Style")]
   [Range(0,2)]
   [SerializeField] private int inputHairStyleNo = 0;

   [Header("Select Hat Style")]
   [Range(0,1)]
   [SerializeField] private int inputHatStyleNo = 0;

   //select adornment style
   [Header("Select Adornments Style")]
   [Range(0,2)]
   [SerializeField] private int inputAdornmentsStyleNo = 0;

   //set skin type
   [Header("Select Skin Type")]
   [Range(0, 3)]
   [SerializeField] private int inputSkinType = 0;



   //select sex but no sex exists yet
   [Header("Select Sex: 0=Male, 1=Female")]
   [Range(0,1)]
   [SerializeField] private int inputSex = 0;

   //select hair color
   [SerializeField] private Color inputHairColor = Color.black;

   //select trouser field
   //pupolate in editor and use this to pull up color picker by specifying
   //used to tint the trousers
   //apply over the grey sprites
   //GENIUS
   [SerializeField] private Color inputTrouserColor = Color.blue;

//facing direction
    private Facing[,] bodyFacingArray; 
    //also need the offset
    //hold in array in order to place correctly
    private Vector2Int[,] bodyShirtOffsetArray;
    private Vector2Int[,] bodyAdornmentsOffsetArray;

    //dimensions
    private int bodyRows = 21;
    
    private int bodyColumns = 6;
    private int farmerSpriteWidth = 16;
    private int farmerSpriteHeight = 32;

    private int shirtTextureWidth = 9;
    //all shirts on top of each other
    private int shirtTextureHeight = 36;
    private int shirtSpriteWidth = 9;
    private int shirtSpriteHeight = 9;
    //fit this many across
    private int shirtStylesInSpriteWidth = 16;

//three different hairstyles
//output texture
    private int hairTextureWidth = 16;
    private int hairTextureHeight = 96;
    //can fit 8 accross the texture
    private int hairStylesInSpriteWidth = 8;
//extract
    private int hatTextureWidth = 20;
    private int hatTextureHeight = 80;
    //can get 12 hats in the input texture, could fir more on the top rows tho
    private int hatStylesInSpriteWidth = 12;

    private int adornmentsTextureWidth = 16;
    private int adornmentsTextureHeight = 32;
    //number that can be fit
    private int adornmentsStylesInSpriteWidth = 8;
    //individual sprite height and width
    private int adornmentsSpriteWidth = 16;
    private int adornmentsSpriteHeight = 16;


    //when recoloring
    //change based on color swap per pixel
    private List<colorSwap> colorSwapList;

    //target colors for color replacement in the arms
    private Color32 armTargetColor1 = new Color32(77, 13, 13, 255);//darkest
    private Color32 armTargetColor2 = new Color32(138, 41, 41, 255); //next darkest
    private Color32 armTargetColor3 = new Color32(172, 50, 50, 255); //lightest

    //target skin color for replacement
    //correspond to the sprite sheet
    private Color32 skinTargetColor1 = new Color32(145, 117, 90, 255); // darkest
    private Color32 skinTargetColor2 = new Color32(204, 155, 108, 255); //next darkest
    private Color32 skinTargetColor3 = new Color32(207, 166, 128, 255); //next darkest
    private Color32 skinTargetColor4 = new Color32(238, 195, 154, 255); //lightest

    private void Awake()
    {
        //initialise color swap list
        colorSwapList = new List<colorSwap>();

        //proces color custimisation
        ProcessCustomisation();
    }
    private void ProcessCustomisation()
    {
        //calls series of methods
        //called in the order they are written
        //generate sprites and then merge with the default
        ProcessGender();
        ProcessShirt();
        ProcessArms();
        ProcessTrousers();
        ProcessHair();
        ProcessSkin();
        ProcessHat();
        ProcessAdornments();
        MergeCustomisations();
    }
    private void ProcessGender()
    {
        //set base spritesheet by gender
        if(inputSex == 0)
        {
            //set to whats in the editor under this information
            farmerBaseTexture = maleFarmerBaseTexture;
        }
        else if(inputSex == 1)
        {
            farmerBaseTexture = femaleFarmerBaseTexture;
        }
        //get base pixels
        //gets all base pixels and populates the array
        Color[] farmerBasePixels = farmerBaseTexture.GetPixels();

        //set changed base pixels
        farmerBaseCustomised.SetPixels(farmerBasePixels);
        //apply to the texture
        farmerBaseCustomised.Apply();
    }

    private void ProcessShirt()
    {
        //Initialize body facing shirt array
        //size of array is these two 
        //direction that the character is facing
        bodyFacingArray = new Facing[bodyColumns, bodyRows];

        //populate body facing shirt array
        PopulateBodyFacingArray();

        //initialise body shirt offset array
        bodyShirtOffsetArray = new Vector2Int[bodyColumns, bodyRows];

        //populate body shirt offset array
        PopulateBodyShirtOffsetArray();

        //create selected shirt texture
        AddShirtToTexture(inputShirtStyleNo);

        //apply shirt texture
        ApplyShirtTextureToBase();
    }
    private void ProcessArms()
    {
        //get arm pixels to recolor
        //get pixels from the sprite sheet for arms
        Color[] farmerPixelsToRecolor = farmerBaseTexture.GetPixels(0,0,288, farmerBaseTexture.height);

        //populate arm color swap list
        PopulateArmColorSwapList();

        //change arm colors
        ChangePixelColors(farmerPixelsToRecolor, colorSwapList);

        //set recolor pixels
        //were processed and changed
        farmerBaseCustomised.SetPixels(0,0,288, farmerBaseTexture.height, farmerPixelsToRecolor);

        //apply texture changes
        farmerBaseCustomised.Apply();
    }
    private void ProcessTrousers()
    {
        //get trouser pixels to recolor
        //look at farmer base texture and then look across
        Color[] farmerTrouserPixels = farmerBaseTexture.GetPixels(288, 0, 96, farmerBaseTexture.height);

        //change trouser color
        TintPixelColors(farmerTrouserPixels, inputTrouserColor);

        //set trouser pixels
        //same position that we got it from 
        farmerBaseCustomised.SetPixels(288, 0, 96, farmerBaseTexture.height, farmerTrouserPixels);

        //apply texture changes
        farmerBaseCustomised.Apply();
    }
    private void ProcessHair()
    {
        //create selected hair
        //extract based on whats in the sheet and add to the output texture
        AddHairToTexture(inputHairStyleNo);

        //get hair pixels to recolor
        Color[] farmerSelectedHairPixels = hairCustomised.GetPixels();

        //tint hair color pixels
        //pass in and pass in the input
        TintPixelColors(farmerSelectedHairPixels, inputHairColor);

//apply back to hair customised and apply
        hairCustomised.SetPixels(farmerSelectedHairPixels);
        hairCustomised.Apply();

    }

    private void ProcessSkin()
    {
        //get skin pixels to recolor
        //look at farmer base at 0,0 and then along the etire sheet to where the texture ends
        Color[] farmerPixelsToRecolor = farmerBaseCustomised.GetPixels(0,0,288, farmerBaseTexture.height);

        //populate skin color swap list
        PopulateSkinColorSwapList(inputSkinType);

        //change skin color
        //pass in pixels to recolor
        ChangePixelColors(farmerPixelsToRecolor, colorSwapList);

        //set recolored pixels
        farmerBaseCustomised.SetPixels(0,0,288, farmerBaseTexture.height, farmerPixelsToRecolor);

        //apply texture changes
        farmerBaseCustomised.Apply();
    }
    private void ProcessHat()
    {
        //create selected hat textures
        AddHatToTexture(inputHatStyleNo);
    }

    private void ProcessAdornments()
    {
        //initialise body adornments offset array
        //2d array
        bodyAdornmentsOffsetArray = new Vector2Int[bodyColumns, bodyRows];

        //populate body adornments offset array
        PopulateBodyAdornmentsOffsetArray();

        //create selected adornments texture
        AddAdornmentsToTexture(inputAdornmentsStyleNo);

        //create new adornments base texture
        //write out adronment in the correct position based on facing
        farmerBaseAdornmentsUpdated = new Texture2D(farmerBaseTexture.width, farmerBaseTexture.height);
        //not antialiased
        farmerBaseAdornmentsUpdated.filterMode = FilterMode.Point;

        //set adornments base texture to transparent
        SetTextureToTransparent(farmerBaseAdornmentsUpdated);
        ApplyAdornmentsTextureToBase();
    }

    private void MergeCustomisations()
    {
        //farmer shirt pixels
        //retrieved from the base and stored in the farmer shirt
        Color[] farmerShirtPixels = farmerBaseShirtsUpdated.GetPixels(0,0, bodyColumns*farmerSpriteWidth, farmerBaseTexture.height);
        //farmer trouser pixels
        Color[] farmerTrouserPixelsSelection = farmerBaseCustomised.GetPixels(288,0,96, farmerBaseTexture.height);

        //farmer adornment pixels
        //merge once gathered
        Color[] farmerAdornmentsPixels = farmerBaseAdornmentsUpdated.GetPixels(0, 0, bodyColumns* farmerSpriteWidth, farmerBaseTexture.height);
        //farmer body pixels
        Color[] farmerBodyPixels = farmerBaseCustomised.GetPixels(0,0, bodyColumns * farmerSpriteWidth, farmerBaseTexture.height);
//two lots of pixels and merges
//both trousers and shirt
        MergeColourArray(farmerBodyPixels, farmerTrouserPixelsSelection);
        MergeColourArray(farmerBodyPixels, farmerShirtPixels);
        MergeColourArray(farmerBodyPixels, farmerAdornmentsPixels);

        //paste merged pixels
        farmerBaseCustomised.SetPixels(0,0, bodyColumns*farmerSpriteWidth, farmerBaseTexture.height, farmerBodyPixels);

        //apply texture changes
        farmerBaseCustomised.Apply();
    }

    private void ApplyAdornmentsTextureToBase()
    {
        Color[] frontAdornmentsPixels;
        Color[] rightAdornmentsPixels;

        frontAdornmentsPixels = selectedAdornment.GetPixels(0, adornmentsSpriteHeight * 1, adornmentsSpriteWidth, adornmentsSpriteHeight);
        rightAdornmentsPixels = selectedAdornment.GetPixels(0, adornmentsSpriteHeight * 0, adornmentsSpriteWidth, adornmentsSpriteHeight);

        //loop through base textures and apply adornment pixels

        for(int x = 0; x < bodyColumns; x++)
        {
            for(int y = 0; y < bodyRows; y++)
            {
                int pixelX = x * farmerSpriteWidth;
                int pixelY = y * farmerSpriteHeight;

                if(bodyAdornmentsOffsetArray[x, y] != null)
                {
                    pixelX += bodyAdornmentsOffsetArray[x, y].x;
                    pixelY += bodyAdornmentsOffsetArray[x, y].y;

                }
                //switch on facing direction
                switch(bodyFacingArray[x, y])
                {
                    case Facing.none:
                        break;
                    case Facing.front:
                        // populate front adornment pixels
                        //for every direction and with displacement
                        farmerBaseAdornmentsUpdated.SetPixels(pixelX, pixelY, adornmentsSpriteWidth, adornmentsSpriteHeight, frontAdornmentsPixels);
                        break;
                    case Facing.right:
                        // populate right adornment pixels
                        farmerBaseAdornmentsUpdated.SetPixels(pixelX, pixelY, adornmentsSpriteWidth, adornmentsSpriteHeight, rightAdornmentsPixels);
                        break;
                    default:
                        break;

                    
                }
            }
        }
        // apply adornments texture pixels
        farmerBaseAdornmentsUpdated.Apply();
    }
    private void TintPixelColors(Color[] basePixelArray, Color tintColor)
    {
        //loop through pixels to tint
        for(int i = 0; i < basePixelArray.Length; i++)
        {
            //apply tint color by mult by all the tint colors
            basePixelArray[i].r = basePixelArray[i].r * tintColor.r;
            basePixelArray[i].g = basePixelArray[i].g * tintColor.g;
            basePixelArray[i].b = basePixelArray[i].b * tintColor.b;
            //pixel array comes back out
        }
    }
    private void PopulateArmColorSwapList()
    {
        //clear color swap list
        colorSwapList.Clear();

        //are replacement colors
        //colors are in those pixel locatoions
        colorSwapList.Add(new colorSwap(armTargetColor1, selectedShirt.GetPixel(0,7)));
        colorSwapList.Add(new colorSwap(armTargetColor2, selectedShirt.GetPixel(0,6)));
        colorSwapList.Add(new colorSwap(armTargetColor3, selectedShirt.GetPixel(0,5)));
    }
    //take in base array and color swaps
    //test if same color
    private void ChangePixelColors(Color[] baseArray, List<colorSwap> colorSwapList)
    {
        for(int i = 0; i < baseArray.Length; i++)
        {
            //loop through color swap list
            if(colorSwapList.Count > 0)
            {
                for(int j= 0; j < colorSwapList.Count; j++)
                {
                    if(isSameColor(baseArray[i], colorSwapList[j].fromColor))
                    {
                        baseArray[i] = colorSwapList[j].toColor;
                    }
                }
            }
        }
    }

    private void PopulateSkinColorSwapList(int skinType)
    {
        //clear color swap list
        colorSwapList.Clear();

        //skin replacement colors
        //switch on skin types
        //swap out colors depending on whats passed in, have the target skin color and then another color thats been passed in
        switch(skinType)
        {
            case 0:
                colorSwapList.Add(new colorSwap(skinTargetColor1, skinTargetColor1));
                colorSwapList.Add(new colorSwap(skinTargetColor2, skinTargetColor2));
                colorSwapList.Add(new colorSwap(skinTargetColor3, skinTargetColor3));
                colorSwapList.Add(new colorSwap(skinTargetColor4, skinTargetColor4));
                break;
            case 1:
                colorSwapList.Add(new colorSwap(skinTargetColor1, new Color32(187, 157, 128, 255)));
                colorSwapList.Add(new colorSwap(skinTargetColor2, new Color32(231, 187, 144, 255)));
                colorSwapList.Add(new colorSwap(skinTargetColor3, new Color32(221, 186, 154, 255)));
                colorSwapList.Add(new colorSwap(skinTargetColor4, new Color32(213, 189, 167, 255)));
                break;
            case 2:
                colorSwapList.Add(new colorSwap(skinTargetColor1, new Color32(105, 69, 2, 255)));
                colorSwapList.Add(new colorSwap(skinTargetColor2, new Color32(128, 87, 12, 255)));
                colorSwapList.Add(new colorSwap(skinTargetColor3, new Color32(145, 103, 167, 255)));
                colorSwapList.Add(new colorSwap(skinTargetColor4, new Color32(161, 114, 25, 255)));
                break;
            case 3:
                colorSwapList.Add(new colorSwap(skinTargetColor1, new Color32(151, 132, 0, 255)));
                colorSwapList.Add(new colorSwap(skinTargetColor2, new Color32(187, 166, 15, 255)));
                colorSwapList.Add(new colorSwap(skinTargetColor3, new Color32(209, 188, 39, 255)));
                colorSwapList.Add(new colorSwap(skinTargetColor4, new Color32(211, 199, 112, 255)));
                break;
            default:
                colorSwapList.Add(new colorSwap(skinTargetColor1, skinTargetColor1));
                colorSwapList.Add(new colorSwap(skinTargetColor2, skinTargetColor2));
                colorSwapList.Add(new colorSwap(skinTargetColor3, skinTargetColor3));
                colorSwapList.Add(new colorSwap(skinTargetColor4, skinTargetColor4));
                break;
        }
    }
    private bool isSameColor(Color color1, Color color2)
    {
        //compares rgb colors to see if theyre the same
        if((color1.r == color2.r) && (color1.g == color2.g) && (color1.b == color2.b) && (color1.a == color2.a))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private void MergeColourArray(Color[] baseArray, Color[] mergeArray)
    {
        //loop through in all base
        for(int i = 0; i < baseArray.Length; i++)
        {
            //if anything is visable in the merge array
            if(mergeArray[i].a > 0)
            {
                //merge array has color
                if(mergeArray[i].a >= 1)
                {
                    //fully replace
                    baseArray[i] = mergeArray[i];
                }
                else
                {
                    //interpolate colors
                    //if one is semi transparent
                    float alpha = mergeArray[i].a;

                    baseArray[i].r += (mergeArray[i].r - baseArray[i].r) * alpha;
                    baseArray[i].g += (mergeArray[i].g - baseArray[i].g) * alpha;
                    baseArray[i].b += (mergeArray[i].b - baseArray[i].b) * alpha;
                    baseArray[i].a += mergeArray[i].a;
                }
            }
        }
    }
    private void AddHairToTexture(int hairStyleNo)
    {
        //calculate coords for hair pixels
        //use the number to calculate the sprite number from the sprite sheet
        int y = (hairStyleNo / hairStylesInSpriteWidth) * hairTextureHeight;
        int x = (hairStyleNo % hairStylesInSpriteWidth) * hairTextureWidth;

        //get hair pixels
        //extract from get pixels
        Color[] hairPixels = hairBaseTexture.GetPixels(x,y, hairTextureWidth, hairTextureHeight);

        //apply selected hair pixels to texture
        hairCustomised.SetPixels(hairPixels);
        hairCustomised.Apply();
    }
    private void AddHatToTexture(int hatStyleNo){
        //calculate coords for hat pixels
        int y = (hatStyleNo / hatStylesInSpriteWidth) *  hatTextureHeight;
        //bottom left hand corner
        int x = (hatStyleNo % hatStylesInSpriteWidth) * hatTextureWidth;

        //get hat pixels
        Color[] hatPixels = hatsBaseTexture.GetPixels(x,y, hatTextureWidth, hairTextureHeight);

        //apply selected hat pixels to texture

        hatsCustomised.SetPixels(hatPixels);
        hatsCustomised.Apply();
    }
    private void AddAdornmentsToTexture(int adornmentsStyleNo)
    {
        //extract and add it to the new texture
        //create adornment texture
        selectedAdornment = new Texture2D(adornmentsTextureWidth, adornmentsTextureHeight);
        selectedAdornment.filterMode = FilterMode.Point;

        //calculate coords for adornment pixels
        int y = (adornmentsStyleNo / adornmentsStylesInSpriteWidth) * adornmentsTextureHeight;
        int x = (adornmentsStyleNo % adornmentsStylesInSpriteWidth) * adornmentsTextureWidth;

        //get adornments pixels
        Color[] adornmentsPixels = adornmentsBaseTexture.GetPixels(x, y, adornmentsTextureWidth, adornmentsTextureHeight);

        // apply selected adornments pixels to texture
        selectedAdornment.SetPixels(adornmentsPixels);
        selectedAdornment.Apply();
    }
    private void AddShirtToTexture(int shirtStyleNo)
    {
        //create shirt texture
        selectedShirt = new Texture2D(shirtTextureWidth, shirtTextureHeight);
        //no antialising in the new created shirt texture
        selectedShirt.filterMode = FilterMode.Point;

        //calculate coords for shirt pixels
        int y = (shirtStyleNo / shirtStylesInSpriteWidth)*shirtTextureHeight;
        //which column
        int x = (shirtStyleNo % shirtStylesInSpriteWidth)*shirtTextureWidth;

        //get shirts pixels
        //retrieve the pixels and add them to the variable
        Color[] shirtPixels = shirtsBaseTexture.GetPixels(x, y, shirtTextureWidth, shirtTextureHeight);

        //apply selected shirt pixels to texture
        selectedShirt.SetPixels(shirtPixels);
        selectedShirt.Apply();
    }
    private void ApplyShirtTextureToBase()
    {
        //create new shirt base textures
        farmerBaseShirtsUpdated = new Texture2D(farmerBaseTexture.width, farmerBaseTexture.height);
        //pure pixel texture
        farmerBaseShirtsUpdated.filterMode = FilterMode.Point;

        //set shirt base texture to transparent
        SetTextureToTransparent(farmerBaseShirtsUpdated);

        //create new arrays
        Color[] frontShirtPixels;
        Color[] backShirtPixels;
        Color[] rightShirtPixels;

//based on position
        frontShirtPixels = selectedShirt.GetPixels(0, shirtSpriteHeight * 3, shirtSpriteWidth, shirtSpriteHeight);
        backShirtPixels = selectedShirt.GetPixels(0, shirtSpriteHeight * 0, shirtSpriteWidth, shirtSpriteHeight);
        rightShirtPixels = selectedShirt.GetPixels(0, shirtSpriteHeight * 2, shirtSpriteWidth, shirtSpriteHeight);

        //loop through base texture and apply shirt pixels
        for(int x = 0; x < bodyColumns; x++)
        {
            for(int y = 0; y < bodyRows; y++)
            {
                //calculated using the height and width of the farmer sprite
                int pixelX = x * farmerSpriteWidth;
                int pixelY = y * farmerSpriteHeight;
//check if a value in it
                if(bodyShirtOffsetArray[x,y]  != null)
                {
                    if(bodyShirtOffsetArray[x,y].x == 99 && bodyShirtOffsetArray[x,y].y == 99) //do not populate with shirt
                       //dont put in character sprite
                        continue;
//adjust by offset
                    pixelX += bodyShirtOffsetArray[x,y].x;
                    pixelY += bodyShirtOffsetArray[x,y].y;
                    
                }
                //switch on facing direstions
                switch(bodyFacingArray[x,y])
                {
                    //blank areas and do nothing
                    case Facing.none:
                    break;

                    case Facing.front:
                        //populate front pixels
                        farmerBaseShirtsUpdated.SetPixels(pixelX, pixelY, shirtSpriteWidth, shirtSpriteHeight, frontShirtPixels);
                        break;

                    case Facing.back:
                        //populate back pixels
                        farmerBaseShirtsUpdated.SetPixels(pixelX, pixelY, shirtSpriteWidth, shirtSpriteHeight, backShirtPixels);
                        break;

                    case Facing.right:
                        //populate right pixels
                        farmerBaseShirtsUpdated.SetPixels(pixelX, pixelY, shirtSpriteWidth, shirtSpriteHeight, rightShirtPixels);
                        break;

                    default:
                    break;
                }
            }
        }
        //apply shirt texture pixels
        farmerBaseShirtsUpdated.Apply();
    }

    private void SetTextureToTransparent(Texture2D texture2D)
    {
        //fill texture with transparency
        //new array of colors based on hieght and width of texture, loop through and set to clear
        Color[] fill = new Color[texture2D.height * texture2D.width];
        for(int i = 0; i < fill.Length; i++)
        {
            fill[i] = Color.clear;
        }
        texture2D.SetPixels(fill);
    }
    private void PopulateBodyFacingArray()
    {
        bodyFacingArray[0, 0] = Facing.none;
        bodyFacingArray[1, 0] = Facing.none;
        bodyFacingArray[2, 0] = Facing.none;
        bodyFacingArray[3, 0] = Facing.none;
        bodyFacingArray[4, 0] = Facing.none;
        bodyFacingArray[5, 0] = Facing.none;

        bodyFacingArray[0, 1] = Facing.none;
        bodyFacingArray[1, 1] = Facing.none;
        bodyFacingArray[2, 1] = Facing.none;
        bodyFacingArray[3, 1] = Facing.none;
        bodyFacingArray[4, 1] = Facing.none;
        bodyFacingArray[5, 1] = Facing.none;

        bodyFacingArray[0, 2] = Facing.none;
        bodyFacingArray[1, 2] = Facing.none;
        bodyFacingArray[2, 2] = Facing.none;
        bodyFacingArray[3, 2] = Facing.none;
        bodyFacingArray[4, 2] = Facing.none;
        bodyFacingArray[5, 2] = Facing.none;

        bodyFacingArray[0, 3] = Facing.none;
        bodyFacingArray[1, 3] = Facing.none;
        bodyFacingArray[2, 3] = Facing.none;
        bodyFacingArray[3, 3] = Facing.none;
        bodyFacingArray[4, 3] = Facing.none;
        bodyFacingArray[5, 3] = Facing.none;

        bodyFacingArray[0, 4] = Facing.none;
        bodyFacingArray[1, 4] = Facing.none;
        bodyFacingArray[2, 4] = Facing.none;
        bodyFacingArray[3, 4] = Facing.none;
        bodyFacingArray[4, 4] = Facing.none;
        bodyFacingArray[5, 4] = Facing.none;

        bodyFacingArray[0, 5] = Facing.none;
        bodyFacingArray[1, 5] = Facing.none;
        bodyFacingArray[2, 5] = Facing.none;
        bodyFacingArray[3, 5] = Facing.none;
        bodyFacingArray[4, 5] = Facing.none;
        bodyFacingArray[5, 5] = Facing.none;

        bodyFacingArray[0, 6] = Facing.none;
        bodyFacingArray[1, 6] = Facing.none;
        bodyFacingArray[2, 6] = Facing.none;
        bodyFacingArray[3, 6] = Facing.none;
        bodyFacingArray[4, 6] = Facing.none;
        bodyFacingArray[5, 6] = Facing.none;
        
        bodyFacingArray[0, 7] = Facing.none;
        bodyFacingArray[1, 7] = Facing.none;
        bodyFacingArray[2, 7] = Facing.none;
        bodyFacingArray[3, 7] = Facing.none;
        bodyFacingArray[4, 7] = Facing.none;
        bodyFacingArray[5, 7] = Facing.none;

        bodyFacingArray[0, 8] = Facing.none;
        bodyFacingArray[1, 8] = Facing.none;
        bodyFacingArray[2, 8] = Facing.none;
        bodyFacingArray[3, 8] = Facing.none;
        bodyFacingArray[4, 8] = Facing.none;
        bodyFacingArray[5, 8] = Facing.none;

        bodyFacingArray[0, 9] = Facing.none;
        bodyFacingArray[1, 9] = Facing.none;
        bodyFacingArray[2, 9] = Facing.none;
        bodyFacingArray[3, 9] = Facing.none;
        bodyFacingArray[4, 9] = Facing.none;
        bodyFacingArray[5, 9] = Facing.none;

        bodyFacingArray[0, 10] = Facing.back;
        bodyFacingArray[1, 10] = Facing.back;
        bodyFacingArray[2, 10] = Facing.right;
        bodyFacingArray[3, 10] = Facing.right;
        bodyFacingArray[4, 10] = Facing.right;
        bodyFacingArray[5, 10] = Facing.right;

        bodyFacingArray[0, 11] = Facing.front;
        bodyFacingArray[1, 11] = Facing.front;
        bodyFacingArray[2, 11] = Facing.front;
        bodyFacingArray[3, 11] = Facing.front;
        bodyFacingArray[4, 11] = Facing.back;
        bodyFacingArray[5, 11] = Facing.back;

        bodyFacingArray[0, 12] = Facing.back;
        bodyFacingArray[1, 12] = Facing.back;
        bodyFacingArray[2, 12] = Facing.right;
        bodyFacingArray[3, 12] = Facing.right;
        bodyFacingArray[4, 12] = Facing.right;
        bodyFacingArray[5, 12] = Facing.right;

        bodyFacingArray[0, 13] = Facing.front;
        bodyFacingArray[1, 13] = Facing.front;
        bodyFacingArray[2, 13] = Facing.front;
        bodyFacingArray[3, 13] = Facing.front;
        bodyFacingArray[4, 13] = Facing.back;
        bodyFacingArray[5, 13] = Facing.back;

        bodyFacingArray[0, 14] = Facing.back;
        bodyFacingArray[1, 14] = Facing.back;
        bodyFacingArray[2, 14] = Facing.right;
        bodyFacingArray[3, 14] = Facing.right;
        bodyFacingArray[4, 14] = Facing.right;
        bodyFacingArray[5, 14] = Facing.right;

        bodyFacingArray[0, 15] = Facing.front;
        bodyFacingArray[1, 15] = Facing.front;
        bodyFacingArray[2, 15] = Facing.front;
        bodyFacingArray[3, 15] = Facing.front;
        bodyFacingArray[4, 15] = Facing.back;
        bodyFacingArray[5, 15] = Facing.back;

        bodyFacingArray[0, 16] = Facing.back;
        bodyFacingArray[1, 16] = Facing.back;
        bodyFacingArray[2, 16] = Facing.right;
        bodyFacingArray[3, 16] = Facing.right;
        bodyFacingArray[4, 16] = Facing.right;
        bodyFacingArray[5, 16] = Facing.right;

        bodyFacingArray[0, 17] = Facing.front;
        bodyFacingArray[1, 17] = Facing.front;
        bodyFacingArray[2, 17] = Facing.front;
        bodyFacingArray[3, 17] = Facing.front;
        bodyFacingArray[4, 17] = Facing.back;
        bodyFacingArray[5, 17] = Facing.back;

        bodyFacingArray[0, 18] = Facing.back;
        bodyFacingArray[1, 18] = Facing.back;
        bodyFacingArray[2, 18] = Facing.back;
        bodyFacingArray[3, 18] = Facing.right;
        bodyFacingArray[4, 18] = Facing.right;
        bodyFacingArray[5, 18] = Facing.right;

        bodyFacingArray[0, 19] = Facing.right;
        bodyFacingArray[1, 19] = Facing.right;
        bodyFacingArray[2, 19] = Facing.right;
        bodyFacingArray[3, 19] = Facing.front;
        bodyFacingArray[4, 19] = Facing.front;
        bodyFacingArray[5, 19] = Facing.front;

        bodyFacingArray[0, 20] = Facing.front;
        bodyFacingArray[1, 20] = Facing.front;
        bodyFacingArray[2, 20] = Facing.front;
        bodyFacingArray[3, 20] = Facing.back;
        bodyFacingArray[4, 20] = Facing.back;
        bodyFacingArray[5, 20] = Facing.back;
    
    }

    private void PopulateBodyAdornmentsOffsetArray()
    {
        bodyAdornmentsOffsetArray[0, 0] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[1, 0] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[2, 0] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[3, 0] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[4, 0] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[5, 0] = new Vector2Int(99, 99);

        bodyAdornmentsOffsetArray[0, 1] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[1, 1] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[2, 1] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[3, 1] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[4, 1] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[5, 1] = new Vector2Int(99, 99);

        bodyAdornmentsOffsetArray[0, 2] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[1, 2] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[2, 2] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[3, 2] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[4, 2] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[5, 2] = new Vector2Int(99, 99);

        bodyAdornmentsOffsetArray[0, 3] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[1, 3] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[2, 3] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[3, 3] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[4, 3] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[5, 3] = new Vector2Int(99, 99);

        bodyAdornmentsOffsetArray[0, 4] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[1, 4] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[2, 4] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[3, 4] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[4, 4] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[5, 4] = new Vector2Int(99, 99);

        bodyAdornmentsOffsetArray[0, 5] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[1, 5] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[2, 5] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[3, 5] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[4, 5] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[5, 5] = new Vector2Int(99, 99);

        bodyAdornmentsOffsetArray[0, 6] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[1, 6] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[2, 6] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[3, 6] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[4, 6] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[5, 6] = new Vector2Int(99, 99);

        bodyAdornmentsOffsetArray[0, 7] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[1, 7] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[2, 7] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[3, 7] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[4, 7] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[5, 7] = new Vector2Int(99, 99);

        bodyAdornmentsOffsetArray[0, 8] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[1, 8] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[2, 8] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[3, 8] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[4, 8] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[5, 8] = new Vector2Int(99, 99);

        bodyAdornmentsOffsetArray[0, 9] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[1, 9] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[2, 9] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[3, 9] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[4, 9] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[5, 9] = new Vector2Int(99, 99);

        bodyAdornmentsOffsetArray[0, 10] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[1, 10] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[2, 10] = new Vector2Int(0, 1 + 16);
        bodyAdornmentsOffsetArray[3, 10] = new Vector2Int(0, 2 + 16);
        bodyAdornmentsOffsetArray[4, 10] = new Vector2Int(0, 1 + 16);
        bodyAdornmentsOffsetArray[5, 10] = new Vector2Int(0, 0 + 16);

        bodyAdornmentsOffsetArray[0, 11] = new Vector2Int(0, 1 + 16);
        bodyAdornmentsOffsetArray[1, 11] = new Vector2Int(0, 2 + 16);
        bodyAdornmentsOffsetArray[2, 11] = new Vector2Int(0, 1 + 16);
        bodyAdornmentsOffsetArray[3, 11] = new Vector2Int(0, 0 + 16);
        bodyAdornmentsOffsetArray[4, 11] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[5, 11] = new Vector2Int(99, 99);

        bodyAdornmentsOffsetArray[0, 12] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[1, 12] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[2, 12] = new Vector2Int(0, 0 + 16);
        bodyAdornmentsOffsetArray[3, 12] = new Vector2Int(0, -1 + 16);
        bodyAdornmentsOffsetArray[4, 12] = new Vector2Int(0, -1 + 16);
        bodyAdornmentsOffsetArray[5, 12] = new Vector2Int(0, -1 + 16);

        bodyAdornmentsOffsetArray[0, 13] = new Vector2Int(0, 0 + 16);
        bodyAdornmentsOffsetArray[1, 13] = new Vector2Int(0, -1 + 16);
        bodyAdornmentsOffsetArray[2, 13] = new Vector2Int(1, -1 + 16);
        bodyAdornmentsOffsetArray[3, 13] = new Vector2Int(1, -1 + 16);
        bodyAdornmentsOffsetArray[4, 13] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[5, 13] = new Vector2Int(99, 99);

        bodyAdornmentsOffsetArray[0, 14] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[1, 14] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[2, 14] = new Vector2Int(0, -3 + 16);
        bodyAdornmentsOffsetArray[3, 14] = new Vector2Int(0, -5 + 16);
        bodyAdornmentsOffsetArray[4, 14] = new Vector2Int(0, -1 + 16);
        bodyAdornmentsOffsetArray[5, 14] = new Vector2Int(0, 1 + 16);

        bodyAdornmentsOffsetArray[0, 15] = new Vector2Int(0, -2 + 16);
        bodyAdornmentsOffsetArray[1, 15] = new Vector2Int(0, -5 + 16);
        bodyAdornmentsOffsetArray[2, 15] = new Vector2Int(0, -1 + 16);
        bodyAdornmentsOffsetArray[3, 15] = new Vector2Int(0, 2 + 16);
        bodyAdornmentsOffsetArray[4, 15] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[5, 15] = new Vector2Int(99, 99);

        bodyAdornmentsOffsetArray[0, 16] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[1, 16] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[2, 16] = new Vector2Int(0, -3 + 16);
        bodyAdornmentsOffsetArray[3, 16] = new Vector2Int(0, -2 + 16);
        bodyAdornmentsOffsetArray[4, 16] = new Vector2Int(0, -1 + 16);
        bodyAdornmentsOffsetArray[5, 16] = new Vector2Int(0, 0 + 16);

        bodyAdornmentsOffsetArray[0, 17] = new Vector2Int(0, -3 + 16);
        bodyAdornmentsOffsetArray[1, 17] = new Vector2Int(0, -2 + 16);
        bodyAdornmentsOffsetArray[2, 17] = new Vector2Int(0, -1 + 16);
        bodyAdornmentsOffsetArray[3, 17] = new Vector2Int(0, 0 + 16);
        bodyAdornmentsOffsetArray[4, 17] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[5, 17] = new Vector2Int(99, 99);

        bodyAdornmentsOffsetArray[0, 18] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[1, 18] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[2, 18] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[3, 18] = new Vector2Int(0, 0 + 16);
        bodyAdornmentsOffsetArray[4, 18] = new Vector2Int(0, -1 + 16);
        bodyAdornmentsOffsetArray[5, 18] = new Vector2Int(0, -1 + 16);

        bodyAdornmentsOffsetArray[0, 19] = new Vector2Int(0, 0 + 16);
        bodyAdornmentsOffsetArray[1, 19] = new Vector2Int(0, -1 + 16);
        bodyAdornmentsOffsetArray[2, 19] = new Vector2Int(0, -1 + 16);
        bodyAdornmentsOffsetArray[3, 19] = new Vector2Int(0, 0 + 16);
        bodyAdornmentsOffsetArray[4, 19] = new Vector2Int(0, -1 + 16);
        bodyAdornmentsOffsetArray[5, 19] = new Vector2Int(0, -1 + 16);

        bodyAdornmentsOffsetArray[0, 20] = new Vector2Int(0, 0 + 16);
        bodyAdornmentsOffsetArray[1, 20] = new Vector2Int(0, -1 + 16);
        bodyAdornmentsOffsetArray[2, 20] = new Vector2Int(0, -1 + 16);
        bodyAdornmentsOffsetArray[3, 20] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[4, 20] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[5, 20] = new Vector2Int(99, 99);
    }

    private void PopulateBodyShirtOffsetArray()
    {
        bodyShirtOffsetArray[0, 0] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[1, 0] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[2, 0] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[3, 0] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[4, 0] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[5, 0] = new Vector2Int(99, 99);

        bodyShirtOffsetArray[0, 1] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[1, 1] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[2, 1] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[3, 1] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[4, 1] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[5, 1] = new Vector2Int(99, 99);

        bodyShirtOffsetArray[0, 2] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[1, 2] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[2, 2] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[3, 2] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[4, 2] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[5, 2] = new Vector2Int(99, 99);

        bodyShirtOffsetArray[0, 3] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[1, 3] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[2, 3] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[3, 3] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[4, 3] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[5, 3] = new Vector2Int(99, 99);

        bodyShirtOffsetArray[0, 4] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[1, 4] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[2, 4] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[3, 4] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[4, 4] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[5, 4] = new Vector2Int(99, 99);

        bodyShirtOffsetArray[0, 5] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[1, 5] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[2, 5] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[3, 5] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[4, 5] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[5, 5] = new Vector2Int(99, 99);

        bodyShirtOffsetArray[0, 6] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[1, 6] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[2, 6] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[3, 6] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[4, 6] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[5, 6] = new Vector2Int(99, 99);

        bodyShirtOffsetArray[0, 7] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[1, 7] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[2, 7] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[3, 7] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[4, 7] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[5, 7] = new Vector2Int(99, 99);

        bodyShirtOffsetArray[0, 8] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[1, 8] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[2, 8] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[3, 8] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[4, 8] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[5, 8] = new Vector2Int(99, 99);

        bodyShirtOffsetArray[0, 9] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[1, 9] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[2, 9] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[3, 9] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[4, 9] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[5, 9] = new Vector2Int(99, 99);

        bodyShirtOffsetArray[0, 10] = new Vector2Int(4, 11);
        bodyShirtOffsetArray[1, 10] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[2, 10] = new Vector2Int(4, 11);
        bodyShirtOffsetArray[3, 10] = new Vector2Int(4, 12);
        bodyShirtOffsetArray[4, 10] = new Vector2Int(4, 11);
        bodyShirtOffsetArray[5, 10] = new Vector2Int(4, 10);

        bodyShirtOffsetArray[0, 11] = new Vector2Int(4, 11);
        bodyShirtOffsetArray[1, 11] = new Vector2Int(4, 12);
        bodyShirtOffsetArray[2, 11] = new Vector2Int(4, 11);
        bodyShirtOffsetArray[3, 11] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[4, 11] = new Vector2Int(4, 11);
        bodyShirtOffsetArray[5, 11] = new Vector2Int(4, 12);

        bodyShirtOffsetArray[0, 12] = new Vector2Int(3, 9);
        bodyShirtOffsetArray[1, 12] = new Vector2Int(3, 9);
        bodyShirtOffsetArray[2, 12] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[3, 12] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[4, 12] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[5, 12] = new Vector2Int(4, 9);

        bodyShirtOffsetArray[0, 13] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[1, 13] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[2, 13] = new Vector2Int(5, 9);
        bodyShirtOffsetArray[3, 13] = new Vector2Int(5, 9);
        bodyShirtOffsetArray[4, 13] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[5, 13] = new Vector2Int(4, 9);

        bodyShirtOffsetArray[0, 14] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[1, 14] = new Vector2Int(4, 12);
        bodyShirtOffsetArray[2, 14] = new Vector2Int(4, 7);
        bodyShirtOffsetArray[3, 14] = new Vector2Int(4, 5);
        bodyShirtOffsetArray[4, 14] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[5, 14] = new Vector2Int(4, 12);

        bodyShirtOffsetArray[0, 15] = new Vector2Int(4, 8);
        bodyShirtOffsetArray[1, 15] = new Vector2Int(4, 5);
        bodyShirtOffsetArray[2, 15] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[3, 15] = new Vector2Int(4, 12);
        bodyShirtOffsetArray[4, 15] = new Vector2Int(4, 8);
        bodyShirtOffsetArray[5, 15] = new Vector2Int(4, 5);

        bodyShirtOffsetArray[0, 16] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[1, 16] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[2, 16] = new Vector2Int(4, 7);
        bodyShirtOffsetArray[3, 16] = new Vector2Int(4, 8);
        bodyShirtOffsetArray[4, 16] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[5, 16] = new Vector2Int(4, 10);
        
        bodyShirtOffsetArray[0, 17] = new Vector2Int(4, 7);
        bodyShirtOffsetArray[1, 17] = new Vector2Int(4, 8);
        bodyShirtOffsetArray[2, 17] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[3, 17] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[4, 17] = new Vector2Int(4, 7);
        bodyShirtOffsetArray[5, 17] = new Vector2Int(4, 8);

        bodyShirtOffsetArray[0, 18] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[1, 18] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[2, 18] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[3, 18] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[4, 18] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[5, 18] = new Vector2Int(4, 9);

        bodyShirtOffsetArray[0, 19] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[1, 19] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[2, 19] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[3, 19] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[4, 19] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[5, 19] = new Vector2Int(4, 9);

        bodyShirtOffsetArray[0, 20] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[1, 20] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[2, 20] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[3, 20] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[4, 20] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[5, 20] = new Vector2Int(4, 9);


         
    }


}

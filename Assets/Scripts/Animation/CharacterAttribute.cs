[System.Serializable]

//creating a struct
public struct CharacterAttribute 
{
    //contains three fields to define which varient is being switched to
    //define the part, color, and type
    //switch all arms animations to hold -> part is arms, color is none, part is carry
    public CharacterPartAnimator characterPart;
    public PartVariantColor partVariantColor;
    public PartVariantType partVariantType;

    public CharacterAttribute(CharacterPartAnimator characterPart, PartVariantColor partVariantColor, PartVariantType partVariantType)
    {
        this.characterPart = characterPart;
        this.partVariantColor = partVariantColor;
        this.partVariantType = partVariantType;
    }
}

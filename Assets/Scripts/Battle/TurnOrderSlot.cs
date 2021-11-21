using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnOrderSlot : MonoBehaviour
{
    [SerializeField] private TMP_Text NameTmpText;
    [SerializeField] private Image BackgroundColorImage;
    [SerializeField] private Image CharPortraitImage;
    private static Sprite blankSprite = null;

    public void InitializeTurnOrderBox(string name, Color32 colorForBg, Sprite spriteToDisplay = null)
    {
        NameTmpText.text = name;
        BackgroundColorImage.color = colorForBg;
        if (spriteToDisplay != blankSprite)
        {
            CharPortraitImage.color = Color.white;
            CharPortraitImage.sprite = spriteToDisplay;
        }
        else
        {
            CharPortraitImage.sprite = blankSprite;
            CharPortraitImage.color = colorForBg;

        }
    }
}

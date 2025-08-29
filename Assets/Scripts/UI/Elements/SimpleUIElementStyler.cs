using UnityEngine;
using UnityEngine.UI;
using Text = TMPro.TextMeshProUGUI;

public class SimpleUIElementStyler : MonoBehaviour, IUIStyle
{
    [SerializeField] private Image panelImage, backgroundImage, specialBackgroundImage, inputImage, handleImage;
    [SerializeField] private Text primaryText, secondaryText;

    public void SetColors(UIStyleData style)
    {
        if (panelImage) panelImage.color = style.backgroundColor;
        if (backgroundImage) backgroundImage.color = style.backgroundColor;
        if (specialBackgroundImage) specialBackgroundImage.color = style.backgroundColor_highlight;
        if (inputImage) inputImage.color = style.backgroundColor_secondary;
        if (handleImage) handleImage.color = style.backgroundColor;
        if (primaryText) primaryText.color = style.textColor_primary;
        if (secondaryText) secondaryText.color = style.textColor_secondary;
    }

    public void SetStyle(UIStyleData style)
    {
        if (panelImage) panelImage.sprite = style.backgroundSprite;
        if (backgroundImage) backgroundImage.sprite = style.inputBackgroundSprite;
        if (specialBackgroundImage) specialBackgroundImage.sprite = style.inputBackgroundSprite;
        if (inputImage) inputImage.sprite = style.inputBackgroundSprite;
        if (handleImage) handleImage.sprite = style.handleSprite;
        if (primaryText) primaryText.font = style.font;
        if (secondaryText) secondaryText.font = style.font;
    }
}

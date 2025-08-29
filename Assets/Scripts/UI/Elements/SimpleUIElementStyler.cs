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
        if (panelImage) panelImage.type = style.isBackgroundTiled ? Image.Type.Tiled : Image.Type.Sliced;
        if (backgroundImage) backgroundImage.sprite = style.buttonSprite;
        if (backgroundImage) backgroundImage.type = style.isButtonTiled ? Image.Type.Tiled : Image.Type.Sliced;
        if (specialBackgroundImage) specialBackgroundImage.sprite = style.inputBackgroundSprite;
        if (specialBackgroundImage) specialBackgroundImage.type = style.isInputBackgroundTiled ? Image.Type.Tiled : Image.Type.Sliced;
        if (inputImage) inputImage.sprite = style.inputBackgroundSprite;
        if (inputImage) inputImage.type = style.isInputBackgroundTiled ? Image.Type.Tiled : Image.Type.Sliced;
        if (handleImage) handleImage.sprite = style.handleSprite;
        if (handleImage) handleImage.type = style.isInputBackgroundTiled ? Image.Type.Tiled : Image.Type.Sliced;
        if (primaryText) primaryText.font = style.font;
        if (secondaryText) secondaryText.font = style.font;
    }
}

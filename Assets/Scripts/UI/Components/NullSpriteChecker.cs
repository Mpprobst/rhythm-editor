using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NullSpriteChecker : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] LayoutElement element;

    private void Start()
    {
        // element may not be fully set up yet. especially in dropdowns
        Invoke("CheckSprite", 0.1f);    
    }

    public void CheckSprite()
    {
        gameObject.SetActive(image.sprite != null);

        if (element && image.sprite)
        {
            float ratio = image.sprite.textureRect.width / image.sprite.textureRect.height;
            element.preferredWidth = image.rectTransform.rect.height * ratio;
        }
    }
}

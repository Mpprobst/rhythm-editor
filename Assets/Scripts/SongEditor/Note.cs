using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Text = TMPro.TextMeshProUGUI;

public class Note : MonoBehaviour
{
    [SerializeField] protected Image image;
    [SerializeField] protected Sprite[] noteIcons;

    public void SetStyle(int iconIdx)
    {
        SetStyle(iconIdx, image.color);
    }

    public void SetStyle(Color color)
    {
        SetStyle(image.sprite, color);
    }

    public void SetStyle(int iconIdx, Color color)
    {
        Sprite sprite = null;
        if (noteIcons.Length > 0) sprite = noteIcons[iconIdx % noteIcons.Length];
        SetStyle(sprite, color);
    }

    public void SetStyle(Sprite sprite, Color color)
    {
        image.sprite = sprite;
        image.color = color;
    }
}

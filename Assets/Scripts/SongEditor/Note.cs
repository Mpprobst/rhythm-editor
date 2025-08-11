using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using Text = TMPro.TextMeshProUGUI;

public class Note : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] protected Image image;

    public UnityEvent<Note> onClick = new UnityEvent<Note>();

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            onClick.Invoke(this);
        }
    }

    public void SetStyle(Sprite sprite)
    {
        SetStyle(sprite, image.color);
    }

    public void SetStyle(Color color)
    {
        SetStyle(image.sprite, color);
    }

    public void SetStyle(Sprite sprite, Color color)
    {
        image.sprite = sprite;
        image.color = color;
    }
}

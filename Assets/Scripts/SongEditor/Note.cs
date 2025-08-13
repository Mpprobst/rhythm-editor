using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using Text = TMPro.TextMeshProUGUI;
using UnityEngine.InputSystem;

public class Note : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] protected Image image;

    public UnityEvent<Note> onClick = new UnityEvent<Note>();

    private int time;   // the time in ms this note would be played in the song

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

    public float GetPosition()
    {
        return image.rectTransform.anchoredPosition.x - image.rectTransform.rect.width * image.rectTransform.pivot.x;   // ensures we always get the top left. center is unreliable due to scaling
    }

    public virtual float GetNoteEndPosition()
    {
        return GetPosition() + image.rectTransform.rect.width;    // ensures we always get the top right. center is unreliable due to scaling
    }

    // relies on the track to tell it when its time is and the track's keybinding
    // returning an array because every note has a length and signals when it the note stops playing
    public virtual NoteData SaveNoteBegin(int time, string key)
    {
        return new NoteData(time, true, key, false);
    }

    public virtual NoteData SaveNoteEnd(int time, string key)
    {
        return new NoteData(time, false, key, false);

    }

    // TODO: a Note variant that can be dragged to spawn an ending note to indicate a long note
    // TODO: variant that marks a combo note

}

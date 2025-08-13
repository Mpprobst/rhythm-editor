using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEditor;
using System.Security.Cryptography;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Rendering;

public class Track : MonoBehaviour, IUIStyle, IPointerClickHandler
{
    [SerializeField] private RectTransform rxForm;
    [SerializeField] private GameObject notePrefab;
    [SerializeField] private Sprite[] noteSprites;

    // track popout options
    [HideInInspector] public UIElement_PopoutButton trackPopout;
    protected PopoutOption_Text trackNameOption;
    protected PopoutOption_Dropdown iconFileOption;
    protected PopoutOption_Action keybindOption, colorOption, deleteOption;

    public UnityEvent<Track> onTrackDestroy = new UnityEvent<Track>();

    // vars
    private float noteWidth = 10;
    private bool recordingInput;
    private string keybind = "";
    private string keyPath = "";    // a path that leads to the device and button that needs to be pressed. This is hopefully more flexible that I expect.
    private Color keyColor = Color.black;

    public string TrackName { get { return trackNameOption.GetValue(); } }
    public string Keybind { get { return keybind; } }
    public string KeyPath { get { return keyPath; } }
    public Color KeyColor { get { return keyColor; } }
    public int SpriteChosen { get { return iconFileOption.GetValue(); } }

    public List<Note> Notes { get { return notes; } }

    private List<Note> notes = new List<Note>();

    #region Settings

    public void SetPopout(UIElement_PopoutButton popoutButton)
    {
        trackPopout = popoutButton;
        trackNameOption = popoutButton.popout.GetOption<PopoutOption_Text>("Track Name");
        trackNameOption.onValueEnter.AddListener(SetTrackName);

        iconFileOption = popoutButton.popout.GetOption<PopoutOption_Dropdown>("Note Image");
        iconFileOption.onValueChanged.AddListener(SetNoteIcon);

        keybindOption = popoutButton.popout.GetOption<PopoutOption_Action>("Keybind");
        keybindOption.onActivate.AddListener(StartRecordingKeybind);

        colorOption = popoutButton.popout.GetOption<PopoutOption_Action>("Color");
        colorOption.onActivate.AddListener(StartRecordingColor);

        deleteOption = popoutButton.popout.GetOption<PopoutOption_Action>("Delete");
        deleteOption.onActivate.AddListener(TryDestroyTrack);

        InputSystem.onAnyButtonPress.Call(currentAction =>
        {
            if (recordingInput)
            {
                Debug.Log(currentAction.name);
                Debug.Log(currentAction.path);

                keybind = currentAction.name;
                keyPath = currentAction.path;
                keybindOption.SetName(keybind);
                SetTrackName(keybind);  // value here doesn't really matter since we just look at the name option anyway
                recordingInput = false;
            }
            else if (currentAction.path == keyPath)
            {
                Debug.Log("pressed custom " + keyPath);
            }
        });
    }

    public void Initialize(TrackData data, string key)
    {
        keybind = key;
        keyPath = data.keybindPath;
        keyColor = data.color;
        trackNameOption.SetValueNoNotify(data.name);
        SetTrackName(data.name);
        iconFileOption.SetValueNoNotify(data.spriteID);
    }

    private void SetTrackName(string val)
    {
        val = $"{trackNameOption.GetValue()} ({keybind}) ";
        trackPopout.SetName(val);
        trackPopout.popout.SetInfo(val);
    }

    private void SetNoteIcon(int option)
    {
        foreach (var note in notes)
        {
            note.SetStyle(iconFileOption.GetOptionImage(option));
        }
    }

    // TODO: prevent space and enter from activating the UI buttons
    private void StartRecordingKeybind()
    {
        recordingInput = true;
        keybindOption.SetName("...");
    }

    private void StartRecordingColor()
    {
        ColorPopout.Instance.onColorChange.AddListener(SetKeyColor);
        ColorPopout.Instance.onClose.AddListener(StopSettingKeyColor);
        ColorPopout.Instance.SetColorNoNotify(keyColor);
        ColorPopout.Instance.SetPositionRelative(trackPopout.GetComponent<RectTransform>());
        ColorPopout.Instance.Open();
    }

    private void SetKeyColor(Color val)
    {
        keyColor = val;
        trackPopout.baseColorBackground = keyColor;
        trackPopout.baseColorLabel = (val.r + val.g + val.b) < 1.5f ? Color.white : Color.black;

        foreach (var note in notes)
        {
            note.SetStyle(val);
        }
    }

    private void StopSettingKeyColor()
    {
        ColorPopout.Instance.onColorChange.RemoveListener(SetKeyColor);
        ColorPopout.Instance.onClose.RemoveListener(StopSettingKeyColor);
    }

    public void TryDestroyTrack()
    {
        MessageModal.ShowMessage("Are you sure?", "Are you sure you want to remove this track and all its contents?", "Yes", DestroyTrack, "No", null);       
    }

    public void DestroyTrack()
    {
        onTrackDestroy.Invoke(this);
        Destroy(gameObject);
        Destroy(trackPopout.popout.gameObject);
        Destroy(trackPopout.gameObject);
    }

    #endregion

    private void Update()
    {
        /*
        if (!string.IsNullOrEmpty(keyPath))
        {
            var input = InputSystem.FindControl(keyPath);
            if (input != null && input.IsPressed())
            {
                Debug.Log("custom path pressed " + keyPath);
            }
        }*/
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //Vector2 p = rxForm.InverseTransformPoint(eventData.position);
        float pos = eventData.position.x;
        pos -= rxForm.transform.position.x;
        pos /= Global.Instance.canvasScale;
        AddNote(pos);
    }

    public Note AddNote(float pos)
    {
        // check if we already have a note at this position
        for (int i = 0; i < notes.Count; i++)
        {
            if (Mathf.Abs(notes[i].transform.position.x - pos) < noteWidth / 2f)
            {
                Debug.Log("Too close to another note. not spawning");
                return null;
            }
        }

        // spawn a note
        GameObject noteObj = GameObject.Instantiate(notePrefab);
        RectTransform noteRxForm = noteObj.GetComponent<RectTransform>();
        //noteRxForm.position = new Vector2(pos, rxForm.position.y);
        noteObj.transform.SetParent(transform);
        noteObj.transform.localScale = Vector3.one;

        Vector2 relativePos = new Vector2(pos, 0);//noteRxForm.anchoredPosition;
        relativePos.x = Mathf.FloorToInt(relativePos.x / noteWidth) * noteWidth;
        relativePos.y = 0;

        // use track pivot and note pivot times track height;
        Vector2 noteSize = Vector2.Scale(noteRxForm.pivot - rxForm.pivot, new Vector2(noteWidth, rxForm.rect.height));
        relativePos += noteSize;
        noteRxForm.anchoredPosition = relativePos;
        //Debug.Log($"p = {pos} -> {relativePos.x - noteSize.x} -> {relativePos.x} ");

        //Debug.Log($"eventPos: {eventData.position} mouse pos {Mouse.current.position.value} anchor pos {relativePos}");
        noteRxForm.sizeDelta = new Vector2(noteWidth, rxForm.rect.height);
        Note note = noteObj.GetComponent<Note>();
        note.SetStyle(iconFileOption.GetOptionImage(iconFileOption.GetValue()), keyColor);
        note.onClick.AddListener(OnNoteRemoved);

        notes.Add(note);
        notes.Sort((x, y) => x.GetPosition().CompareTo(y.GetPosition()));

        return note;
    }

    public void OnNoteRemoved(Note note)
    {
        // find it in the list and 
        notes.Remove(note);
        Destroy(note.gameObject);
    }

    public void SetNoteWidth(float width)
    {
        foreach (var note in notes)
        {
            RectTransform noteRxForm = note.GetComponent<RectTransform>();
            // use track pivot and note pivot times track height;

            Vector2 relativePos = noteRxForm.anchoredPosition - Vector2.Scale(noteRxForm.pivot - rxForm.pivot, noteRxForm.rect.size); // position is set with this offset so we need to remove it pre-calc
            float notePos = relativePos.x / noteWidth;  // beat count this note would fall on
            relativePos.x = notePos * width;            // should still be on the same beat count, but uses new width to know how far from the start that is.
            relativePos.y = 0;

            noteRxForm.sizeDelta = new Vector2(width, rxForm.rect.height);

            // shouldn't need to account for the note size because these should be centered to the top left
            Vector2 noteSize = Vector2.Scale(noteRxForm.pivot - rxForm.pivot, new Vector2(noteRxForm.rect.width, rxForm.rect.height));
            relativePos += noteSize;
            noteRxForm.anchoredPosition = relativePos;
        }
        noteWidth = width;
    }


    public void SetColors(UIStyleData style)
    {
        throw new System.NotImplementedException();
    }

    public void SetStyle(UIStyleData style)
    {
        throw new System.NotImplementedException();
    }


    public List<NoteData> GetNoteData(float songLength)
    {
        List<NoteData> noteData = new List<NoteData>();

        for (int i = 0; i < notes.Count; i++)
        {
            // saving two notes where one is the event that the note ends
            // BUG: each quarter note is consistently 1 ms early (compounding) 
            // going to just live with this for now
            Note n = notes[i];
            int t = Mathf.FloorToInt(n.GetPosition() / rxForm.rect.width * songLength * 1000f);   // x1000 makes it ms
            noteData.Add(n.SaveNoteBegin(t, keybind));
            t = Mathf.FloorToInt(n.GetNoteEndPosition() / rxForm.rect.width * songLength * 1000f);   // x1000 makes it ms
            noteData.Add(n.SaveNoteEnd(t, keybind));
        }
        return noteData;    
    }

}

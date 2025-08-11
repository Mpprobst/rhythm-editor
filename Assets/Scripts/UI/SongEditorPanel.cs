using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Unity.VisualScripting;


public class SongEditorPanel : UILayout
{
    [Header("Tracks")]
    [SerializeField] private ScrollRect trackView;
    [SerializeField] private RectTransform sectionContainer;
    [SerializeField] private GameObject sectionPrefab;
    [SerializeField] private RectTransform trackHeaderContainer;    // headers show and control the track info
    [SerializeField] private GameObject trackHeaderPrefab;          // could probably be UIElement_PopoutButtons used to control the individual track info popups
    [SerializeField] private RectTransform trackContainer;
    [SerializeField] private GameObject trackPrefab;

    [Header("Notes")]
    [SerializeField] private float minNoteWidth = 10f;
    [SerializeField] private float defaultNoteWidth = 50f;
    [SerializeField] private int notePrecision = 4;

    [Header("Components")]
    [SerializeField] private AudioSource songSource;

    // UI Elements
    protected UIElement_Button addTrackButton;
    protected UIElement_PopoutButton songOptionsButton; // loads new song mp3s and sets bpm
    protected PopoutOption_File songOption;
    protected PopoutOption_Number bpmOption;
    protected List<UIElement_PopoutButton> trackButtons = new List<UIElement_PopoutButton>();

    // data
    private AudioClip loadedSong;
    private float BPM = 120;
    private float zoom = 1;
    private float minZoom, maxZoom;

    // cached values
    private int numNotes;
    private float beatWidth;

    private List<Track> tracks = new List<Track>();

    protected override void Awake()
    {
        base.Awake();
        // probably disable this while we don't have a song loaded
        addTrackButton = GetElement<UIElement_Button>("Add Track");
        addTrackButton.onActivate.AddListener(AddTrack);
        
        songOptionsButton = GetElement<UIElement_PopoutButton>("Song Settings");

        songOption = (PopoutOption_File)songOptionsButton.popout.GetOption<PopoutOption_File>("Song");
        songOption.onFileSelected.AddListener(SongSelected);

        bpmOption = (PopoutOption_Number)songOptionsButton.popout.GetOption<PopoutOption_Number>("BPM");
        bpmOption.onNumberChanged.AddListener(SetBPM);

        trackContainer.sizeDelta = new Vector2(0, 0);   // hides this before we have added anything

        minZoom = minNoteWidth / defaultNoteWidth;
        maxZoom = 2f;
    }

    protected void Update()
    {

        trackView.vertical = Keyboard.current.leftAltKey.isPressed;
        trackView.horizontal = !Keyboard.current.leftShiftKey.isPressed;

        if (Mouse.current.scroll.value.magnitude > 0 && Utils.IsMouseInRect(Mouse.current.position.value, trackView.viewport)) 
        {
            if (Keyboard.current.leftShiftKey.isPressed)
            {
                // zoom in and out
                zoom = Mathf.Clamp(zoom + Mouse.current.scroll.value.y / 1000f, minZoom, maxZoom);
                SetWidth();
            }
        }
    }

    public void AddTrack(bool val)
    {   
        // ensures track names and tracks line up. do this here because button height wasn't ready on awake
        trackContainer.sizeDelta = new Vector2(0, addTrackButton.GetComponent<RectTransform>().rect.height);
        
        UIElementData newTrackData = new UIElementData();
        newTrackData.elementType = UIElementType.POPOUT;
        newTrackData.elementPrefab = (GameObject)Resources.Load<Object>("UI/Elements/UIElement_PopoutButton");
        newTrackData.elementName = "New Track " + trackButtons.Count + 1;
        newTrackData.popoutPrefab = (GameObject)Resources.Load<Object>("UI/Popouts/PopoutBase");    // needs to be a specific Track popout because it needs its own class
        List<ElementInputOptionData> options = new List<ElementInputOptionData>();
        ElementInputOptionData trackNameOptionData = new ElementInputOptionData(OptionType.TEXT, "Track Name");
        ElementInputOptionData noteIconData = new ElementInputOptionData(OptionType.DROPDOWN, "Note Image", FileType.IMAGE);
        ElementInputOptionData keybindData = new ElementInputOptionData(OptionType.ACTION, "Keybind");
        options.Add(trackNameOptionData);
        options.Add(noteIconData);
        options.Add(keybindData);
        newTrackData.popoutOptions = options.ToArray();
        // default icon would be cool
        
        LayoutAlignment aligment = LayoutAlignment.CENTER;
        // TODO: could get alignment from screen pos
        UIElement_PopoutButton trackButton = (UIElement_PopoutButton)LayoutGenerator.SpawnElement(newTrackData, secondaryRibbon, this, aligment, ScreenSide.LEFT);
        trackButton.id = trackButtons.Count;
        trackButton.SetColors(activeStyle);
        trackButton.SetStyle(activeStyle);

        trackButtons.Add(trackButton as UIElement_PopoutButton);
        addTrackButton.transform.SetAsLastSibling();

        GameObject trackObj = Instantiate(trackPrefab, trackContainer);// TODO: connect this to the trackpopout class
        Track track = trackObj.GetComponent<Track>();
        track.SetPopout(trackButton);

        tracks.Add(track);

        SetWidth(); // adding a track is setting the container width to 0 for some reason. whatever. this is a pretty cheap operation
    }

    private void SongSelected(string pathToSong)
    {
        StartCoroutine(GetAudioFile(pathToSong));
    }

    // https://discussions.unity.com/t/load-audio-files-from-file/644803/12
    IEnumerator GetAudioFile(string url_voice)
    {
        WWW w = new WWW(url_voice);
        yield return w;

        loadedSong = w.GetAudioClip();
        float beatsPerSecond = BPM / 60f;   // ex if BPM is 120, I am doing 2 beats per second and 8 sixteenth notes per second.
        numNotes = Mathf.CeilToInt(loadedSong.length * beatsPerSecond);

        SetWidth();
    }

    // call this when zooming
    private void SetWidth()
    {
        beatWidth = Mathf.Max(defaultNoteWidth * zoom, minNoteWidth); 
        trackView.content.sizeDelta = new Vector2(numNotes * beatWidth, 0);  // height will be caucluated by layout components. No need to specify it here
        Debug.Log("set width to " + trackView.content.sizeDelta);
        // TODO: some kind of repeating texture that could set tiling so every measure is noted.
        // TODO: multiple textures based on the note precision 
        foreach (var track in tracks) {
            track.roundToVal = beatWidth;
        }
    }

    private void SetBPM(float bpm)
    {
        // get length of song and set the width of the tracks to something comprabable x the scale
        BPM = bpm;
        SetWidth();
    }

    private void AddNote(float trackPosition)
    {
        // use the beatWidth and the track width to know which beat this note was added to
        // might need this function on tracks individually
        int noteIndex = Mathf.RoundToInt(trackPosition / trackView.content.rect.width * numNotes);
    }

    // TODO: call with right click
    private void RemoveNote(float trackPosition)
    {
        
    }

}

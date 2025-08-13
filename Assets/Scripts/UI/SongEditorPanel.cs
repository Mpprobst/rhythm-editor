using DG.Tweening;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Newtonsoft.Json;

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
    [SerializeField] private Sprite[] noteSprites;

    [Header("Components")]
    [SerializeField] private AudioSource songSource;

    // UI Elements
    protected UIElement_Label songLabel;
    protected UIElement_Button addTrackButton;
    protected UIElement_PopoutButton songOptionsButton; // loads new song mp3s and sets bpm
    protected PopoutOption_Text songNameOption;
    protected PopoutOption_Action songSaveOption;
    protected PopoutOption_File songLoadOption;
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
        songLabel = GetElement<UIElement_Label>("Song and Instrument");

        addTrackButton = GetElement<UIElement_Button>("Add Track");
        addTrackButton.onActivate.AddListener(SpawnNewTrack);
        
        songOptionsButton = GetElement<UIElement_PopoutButton>("Song Settings");

        songNameOption = songOptionsButton.popout.GetOption<PopoutOption_Text>("Song Name");
        songNameOption.onValueEnter.AddListener(SetSongName);

        songSaveOption = songOptionsButton.popout.GetOption<PopoutOption_Action>("Save");
        songSaveOption.onActivate.AddListener(Save);

        songLoadOption = songOptionsButton.popout.GetOption<PopoutOption_File>("Load");
        songLoadOption.onFileSelected.AddListener(Load);

        songOption = songOptionsButton.popout.GetOption<PopoutOption_File>("Song");
        songOption.onFileSelected.AddListener(SongSelected);

        bpmOption = songOptionsButton.popout.GetOption<PopoutOption_Number>("BPM");
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

    private void SpawnNewTrack(bool val)
    {
        AddTrack();
    }

    public Track AddTrack()
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
        noteIconData.options = new DropdownOptionData[noteSprites.Length];
        for (int i = 0; i <  noteSprites.Length; i++)
            noteIconData.options[i] = new DropdownOptionData(i.ToString(), noteSprites[i]);

        ElementInputOptionData keybindData = new ElementInputOptionData(OptionType.ACTION, "Keybind");
        ElementInputOptionData colorData = new ElementInputOptionData(OptionType.ACTION, "Color");
        ElementInputOptionData deleteOptionData = new ElementInputOptionData(OptionType.ACTION, "Delete");

        options.Add(trackNameOptionData);
        options.Add(noteIconData);
        options.Add(keybindData);
        options.Add(colorData);
        options.Add(deleteOptionData);
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
        trackObj.GetComponent<RectTransform>().sizeDelta = new Vector2(0, addTrackButton.GetComponent<RectTransform>().rect.height);  // keeps the track the same height as its controlling button

        Track track = trackObj.GetComponent<Track>();
        track.SetPopout(trackButton);
        track.onTrackDestroy.AddListener(TrackDestroyed);
        tracks.Add(track);

        SetWidth(); // adding a track is setting the container width to 0 for some reason. whatever. this is a pretty cheap operation
        return track;
    }

    private void TrackDestroyed(Track track)
    {
        tracks.Remove(track);
    }

    private void SetSongName(string songName)
    {
        if (songLabel)
            songLabel.SetName(songName);
    }

    private void SongSelected(string pathToSong)
    {
        StartCoroutine(GetAudioFile(pathToSong, null));
    }

    // https://discussions.unity.com/t/load-audio-files-from-file/644803/12
    IEnumerator GetAudioFile(string url_voice, System.Action callback)
    {
        WWW w = new WWW(url_voice);
        yield return w;

        loadedSong = w.GetAudioClip();
        float beatsPerSecond = BPM / 60f;   // ex if BPM is 120, I am doing 2 beats per second and 8 sixteenth notes per second.
        numNotes = Mathf.CeilToInt(loadedSong.length * beatsPerSecond);

        SetWidth();

        if (callback != null) callback.Invoke();
    }

    // call this when zooming
    private void SetWidth()
    {
        beatWidth = Mathf.Max(defaultNoteWidth * zoom, minNoteWidth);
        
        // a layout group will make the indivudual tracks shorter than the actual song length so we must account for it
        Vector2 padding = Vector2.zero;
        LayoutGroup contentLayoutGroup = trackView.content.GetComponent<LayoutGroup>();
        if (contentLayoutGroup) padding = new Vector2(contentLayoutGroup.padding.left + contentLayoutGroup.padding.right, contentLayoutGroup.padding.top + contentLayoutGroup.padding.bottom);   // i know top right and bottom padding are 0, but if they weren't we can still account for that
        trackView.content.sizeDelta = new Vector2(numNotes * beatWidth, 0) + padding;  // height will be caucluated by layout components. No need to specify it here

        // TODO: some kind of repeating texture that could set tiling so every measure is noted.
        // TODO: multiple textures based on the note precision 
        foreach (var track in tracks) {
            track.SetNoteWidth(beatWidth);
        }
    }

    private void SetBPM(float bpm)
    {
        // get length of song and set the width of the tracks to something comprabable x the scale
        BPM = bpm;
        SetWidth();
    }

    private void Save()
    {
        if (loadedSong == null)
        {
            // TODO: a modal
            Debug.LogWarning("Cannot save when there is no song loaded!");
            return;
        }

        if (tracks.Count == 0)
        {
            Debug.LogWarning("Cannot sasve when there is no track data!");
            return;
        }

        // save loaded song
        SongData songData = new SongData();
        songData.bpm = BPM;
        songData.title = songNameOption.GetValue();
        songData.pathToSong = songOption.GetPathToFile();

        songData.trackData = new Dictionary<string, TrackData>();
        songData.notes = new List<NoteData>();
        for (int i =0; i < tracks.Count; i++)
        {
            var track = tracks[i];
            if (string.IsNullOrEmpty(track.Keybind) || string.IsNullOrEmpty(track.KeyPath))
            {
                Debug.LogWarning($"cannot save when {track.name} has no keybind!");
                return;
            }
            songData.trackData.Add(track.Keybind, new TrackData(track.TrackName, track.KeyPath, track.KeyColor, track.SpriteChosen));
            songData.notes.AddRange(track.GetNoteData(loadedSong.length));
        }

        // ensures all notes are sorted by time
        songData.notes.Sort((x, y) => x.time.CompareTo(y.time));

        string json = JsonConvert.SerializeObject(songData);
        string tracksDir = $"{Application.persistentDataPath}/tracks";
        if (!Directory.Exists(tracksDir))
            Directory.CreateDirectory(tracksDir);
        string trackPath = $"{tracksDir}/{songData.title}.json";

        if (File.Exists(trackPath))
        {
            Debug.LogWarning($"{trackPath} has been overwritten");
        }
        else
        {
            Debug.Log($"Saving {$"{tracksDir}/{songData.title}.json"}");
        }

        File.WriteAllText(trackPath, json);
    }

    private void Load(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogWarning("Cannot load track file. It doesn't exist");
            return;
        }
        Debug.Log($"Load song at:" + path);

        SongData songData = JsonConvert.DeserializeObject<SongData>(File.ReadAllText(path));

        if (!File.Exists(songData.pathToSong))
        {
            Debug.LogWarning("Track failed to load because path to song does not exist " + songData.pathToSong);
            return;
        }

        // destroy existing tracks
        foreach (var track in tracks)
        {
            track.onTrackDestroy.RemoveAllListeners();
            track.DestroyTrack();
        }
        tracks.Clear();

        SetSongName(songData.title);
        songNameOption.SetValueNoNotify(songData.title);
        BPM = songData.bpm;
        bpmOption.SetValueNoNotify(BPM);

        songOption.SetValueNoNotify(songData.pathToSong);
        StartCoroutine(GetAudioFile(songData.pathToSong, () => { 
            Dictionary<string, Track> trackDict = new Dictionary<string, Track>();
            foreach (var trackData in songData.trackData)
            {
                Track track = AddTrack();
                track.Initialize(trackData.Value, trackData.Key);
                trackDict.Add(trackData.Key, track);
            }

            for (int i = 0; i < songData.notes.Count; i++)
            {
                NoteData n = songData.notes[i];
                // p = t * w / len / 1000

                float p = Mathf.CeilToInt(n.time * numNotes * beatWidth / 1000f / loadedSong.length);
                // should maybe round up?
                Debug.Log($"t {n.time} -> {p}");
                if (n.noteOn)
                {
                    Note note = trackDict[n.key].AddNote(p);
                    // TODO: combo if it comes to that
                }
                else
                {
                    // the last started note on this track will be the one we end
                    if (trackDict[n.key].Notes.Count > 0)
                    {
                        Note lastNote = trackDict[n.key].Notes[trackDict[n.key].Notes.Count - 1];
                        // not sure what we do here, but if the position of this note is greater than th
                        float pdiff = p - lastNote.GetPosition();
                        if (pdiff > beatWidth)
                        {
                            Debug.Log("long notee!" + pdiff);
                        }
                    }
                }
            }

        }));
    }

}

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

    // track popout options
    [HideInInspector] public UIElement_PopoutButton trackPopout;
    protected PopoutOption_Text trackNameOption;
    protected PopoutOption_Dropdown iconFileOption;
    protected PopoutOption_Action keybindOption;  // todo: special where we listen for input. might need to be a different option type.
    // TODO: color option

    // vars
    public float roundToVal = 10;
    private bool recordingInput;
    private string keybind; // TODO: may need to store an InputSystem variable;
    private string keyPath;                    // I like the idea of saving paths to the input then just looking through active devices for the correct path.
                            // // maybe that can be done at set time instead of every frame

    private List<Note> notes = new List<Note>();

    #region Settings
    public void SetPopout(UIElement_PopoutButton popoutButton)
    {
        trackPopout = popoutButton;
        trackNameOption = (PopoutOption_Text)popoutButton.popout.GetOption<PopoutOption_Text>("Track Name");
        trackNameOption.onValueEnter.AddListener(SetTrackName);

        iconFileOption = (PopoutOption_Dropdown)popoutButton.popout.GetOption<PopoutOption_Dropdown>("Note Image");
        iconFileOption.onValueChanged.AddListener(SetNoteIcon);

        keybindOption = (PopoutOption_Action)popoutButton.popout.GetOption<PopoutOption_Action>("Keybind");
        keybindOption.onActivate.AddListener(StartRecordingKeybind);

        InputSystem.onAnyButtonPress.Call(currentAction =>
        {
            if (recordingInput)
            {
                Debug.Log(currentAction.name);
                Debug.Log(currentAction.path);
                
                keybind = currentAction.name;
                keyPath = currentAction.path;
                keybindOption.SetName(keybind);
                recordingInput = false;
            }
            else if (currentAction.path == keyPath)
            {
                Debug.Log("pressed custom " + keyPath);
            }
        });
    }

    private void SetTrackName(string val)
    {
        trackPopout.SetName(val);
        trackPopout.popout.SetInfo(val);
    }

    private void SetNoteIcon(int option)
    {
        foreach (var note in notes)
        {
            note.SetStyle(option);
        }
    }

    private void StartRecordingKeybind()
    {
        recordingInput = true;
        keybindOption.SetName("...");
    }

    private void SetKeybind(string key)
    {
        // not sure if I need to apply it to notes

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
        Vector2 localPos = Utils.RectCornerLocalPosition(rxForm, Vector2.up);
        Vector2 p = rxForm.InverseTransformPoint(eventData.position);
        // TODO: don't spawn if there's another note within round distance
        // TODO: use note to get element width? because it could be a long note
          
        // spawn a note
        GameObject noteObj = GameObject.Instantiate(notePrefab);
        RectTransform noteRxForm = noteObj.GetComponent<RectTransform>();
        noteRxForm.position = eventData.position;
        noteObj.transform.SetParent(transform);
        noteObj.transform.localScale = Vector3.one;

        Vector2 relativePos = noteRxForm.anchoredPosition;
        relativePos.x = Mathf.RoundToInt(relativePos.x / roundToVal) * roundToVal;
        relativePos.y = 0;

        // use track pivot and note pivot times track height;
        // track is (0,1) note is (0, 0) need to add 0,1
        Vector2 noteSize = Vector2.Scale(noteRxForm.pivot - rxForm.pivot, new Vector2(noteRxForm.rect.width, rxForm.rect.height));
        relativePos += noteSize;
        noteRxForm.anchoredPosition = relativePos;

        //Debug.Log($"eventPos: {eventData.position} mouse pos {Mouse.current.position.value} anchor pos {relativePos}");
        noteRxForm.sizeDelta = new Vector2(roundToVal, rxForm.rect.height);
        Note note = noteObj.GetComponent<Note>();
        note.SetStyle(iconFileOption.GetValue(), Color.white);

        // TODO: sort notes by x val
        notes.Add(noteObj.GetComponent<Note>());

        // apply the style to the note
        // set style from the popout
    }

    public void SetColors(UIStyleData style)
    {
        throw new System.NotImplementedException();
    }

    public void SetStyle(UIStyleData style)
    {
        throw new System.NotImplementedException();
    }

}

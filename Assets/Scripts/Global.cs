using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using Newtonsoft.Json;

// global runtime variables. 
// should not be accessed by editor scripts I think
public class Global : MonoBehaviour 
{
    public static Global Instance { get { return _instance; } }
    private static Global _instance;

    public Canvas mainCanvas;
    public float canvasScale { get { return mainCanvas.transform.localScale.x; } }

    public UILayout mainLayout;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        JsonConvert.DefaultSettings = () => new JsonSerializerSettings
        {
            Converters = { new ColorConverter() }
        };

        //StartCoroutine(ModalTest());

    }

    private IEnumerator ModalTest()
    {
        yield return new WaitForSeconds(1);
        MessageModal.ShowMessage("Test1", "this is a test on global start with no options");
        yield return new WaitForSeconds(1);
        MessageModal.ShowMessage("Test2", "Now with options", "Yes", () => { Debug.Log("Yes"); }, "No", () => { Debug.Log("No"); } );
        yield return new WaitForSeconds(1);
        MessageModal.ShowMessage("Test3", "Test the third has a really long message because sometimes the modal struggles to refresh its value due to the nested content size fitters. It has a second option name but no second option function so there should probably only be one button", "Yes", () => { Debug.Log("Yes"); }, "No" );
        yield return new WaitForSeconds(1);
        MessageModal.ShowMessage("Test4", "This should overwrite the first message", "Yes", null, "No", () => { Debug.Log("No"); });
    }
}

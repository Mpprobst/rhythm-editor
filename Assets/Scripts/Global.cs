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

    }
}

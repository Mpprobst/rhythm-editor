using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using Newtonsoft.Json.Linq;

public class SongData 
{
    public string title;
    public string artist;   // fun later
    public float bpm;
    public string pathToSong;
    public Dictionary<string, TrackData> trackData; // TODO: maybe this needs to be a dictionary of track data
    public List<NoteData> notes;    // TODO: technically this is just notes for one instrument for the song.

    // songs saved in a midi-inspired format
    // whenever a note comes on, we have a NOTE ON line
    // whenever a note is off, we have a NOTE OFF line
    // each line has a timestamp in ms
    // I can still do this in JSON because that will just be easier to parse
}

public class TrackData
{
    public string name;
    public string keybindPath;
    public Color color;
    public int spriteID;

    public TrackData(string n, string keyPath, Color c, int s_id)
    {
        name = n; 
        keybindPath = keyPath;    
        color = c;
        spriteID = s_id;
    }
}

public class NoteData
{
    public int time;  // time at which this event occurred in ms
    public bool noteOn; // whether the note just started or not
    public string key;  // key for the keybindPaths dictionary of the song. Will have a uniquely assigned name (probably just the descriptive name of the input key)
    public bool combo;  // can the note be combo'd with another? Like a hammer on.

    public NoteData(int t, bool on, string k, bool c = false)
    {
        time = t;
        noteOn = on;
        key = k;
        combo = c;
    }
}


public class ColorConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Color);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        JObject jsonObj = JObject.Load(reader);
        float r = 1;
        float g = 1;
        float b = 1;
        if (jsonObj["r"] != null)
            float.TryParse(jsonObj["r"].ToString(), out r);
        if (jsonObj["g"] != null)
            float.TryParse(jsonObj["g"].ToString(), out g);
        if (jsonObj["b"] != null)
            float.TryParse(jsonObj["b"].ToString(), out b);

        return  new Color(r, g, b);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        JObject jsonObj = new JObject();
        Color color = (Color)value;
        jsonObj.Add("r", JToken.FromObject(color.r));
        jsonObj.Add("g", JToken.FromObject(color.g));
        jsonObj.Add("b", JToken.FromObject(color.b));

        jsonObj.WriteTo(writer);
    }
} 

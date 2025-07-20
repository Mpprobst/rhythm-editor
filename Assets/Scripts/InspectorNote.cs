using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// this is just used to write a note in the inspector
/// </summary>
public class InspectorNote : MonoBehaviour
{
    [SerializeField][TextArea(5,10)] private string note;
}

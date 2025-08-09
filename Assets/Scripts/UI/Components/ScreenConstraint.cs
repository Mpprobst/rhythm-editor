using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

/// <summary>
/// Keeps this UI element on the screen whenever it is moved
/// </summary>
public class ScreenConstraint : MonoBehaviour
{
    [SerializeField] private RectTransform rxForm;

    [SerializeField] private Canvas parentCanvas;

    public void CheckPosition()
    {
        if (parentCanvas == null)
            parentCanvas = GetComponentInParent<Canvas>();

        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        if (parentCanvas)
            screenSize = parentCanvas.renderingDisplaySize;

        if (rxForm == null)
            rxForm = GetComponent<RectTransform>();

        // use my position and size
        Vector2[] corners = Utils.GetRectCorners(rxForm);

        // if any of these are outside the screen, find out by how much and move it
        Vector3 offset = Vector3.zero;
        for (int i = 0; i < corners.Length; i++)
        {
            if (corners[i].x < 0 && offset.x == 0)
                offset += new Vector3(-corners[i].x, 0, 0);
            else if (corners[i].x > screenSize.x && offset.x == 0)
                offset -= new Vector3(corners[i].x - screenSize.x, 0, 0);
            else if (corners[i].y < 0 && offset.y == 0)
                offset += new Vector3(0, -corners[i].y, 0);
            else if (corners[i].y > screenSize.y && offset.y == 0)
                offset -= new Vector3(0, corners[i].y - screenSize.y, 0);

        }
        print($"pos {rxForm.position} + {offset} screen {screenSize} x({corners[0].x}..{corners[3].x}) y({corners[0].y}..{corners[1].y})");
        rxForm.position += offset;
    }
}

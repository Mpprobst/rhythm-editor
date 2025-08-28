using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class RectStretchTest : MonoBehaviour, IUIStyle
{

    public RectTransform rectTransform;
    public Vector2 size;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            Utils.StretchRectEdge(rectTransform, new Vector2(size.x, size.y), Utils.AnchorPreset.LEFT);
        }
        else if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            Utils.StretchRectEdge(rectTransform, new Vector2(size.x, size.y), Utils.AnchorPreset.RIGHT);
        }
        else if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            Utils.StretchRectEdge(rectTransform, new Vector2(size.y, size.x), Utils.AnchorPreset.TOP);
        }
        else if (Keyboard.current.digit4Key.wasPressedThisFrame)
        {
            Utils.StretchRectEdge(rectTransform, new Vector2(size.x, size.y), Utils.AnchorPreset.BOTTOM);
        }
    }



    public Vector2 MouseToRectPosition(Vector2 mousePos)
    {
        Vector3 pos = mousePos;                     // screen position
        pos -= rectTransform.transform.position;    // global position is from bottom left of screen
        Vector3 scale = Utils.GetOverallScale(rectTransform);
        pos = Vector3.Scale(pos, scale);            // compounding parent gameobjcet scale affects position
        return pos;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        Vector2 pos = MouseToRectPosition(eventData.position);
        rectTransform.anchoredPosition = pos;
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

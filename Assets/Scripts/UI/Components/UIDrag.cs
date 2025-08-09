using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class UIDrag : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Tooltip("The whoe item that will be dragged")][SerializeField] private RectTransform rxForm;
    Canvas parentCanvas;

    private bool isDragging = false;
    private Vector2 dragOffset = Vector2.zero;

    public void OnPointerDown(PointerEventData eventData)
    {
        // start following the mouse
        isDragging = true;
        Vector2 pos = new Vector2(rxForm.position.x, rxForm.position.y);    // rxForm.anchoredPosition
        dragOffset = pos - eventData.position;  
        if (parentCanvas == null)
            parentCanvas = rxForm.GetComponentInParent<Canvas>();   
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // stop tracking the mouse
        isDragging = false;
        if (rxForm.GetComponent<ScreenConstraint>())
            rxForm.GetComponent<ScreenConstraint>().CheckPosition();
    }

    

    // Update is called once per frame
    void Update()
    {
        if (isDragging)
        {
            // get mouse position, set pos to that plus drag offset
            Vector2 mousePos = Mouse.current.position.value;
            Vector2 screenSize = new Vector2(Screen.width, Screen.height);
            Vector2 screenRatio = new Vector2( parentCanvas.renderingDisplaySize.x / screenSize.x,  parentCanvas.renderingDisplaySize.y / screenSize.y);
            mousePos = Vector2.Scale(mousePos, screenRatio);
            // scale the mousePos to the screen size because 
            rxForm.position = mousePos + dragOffset;    // using position instead of anchored position because that won't follow the mouse directly
        }
    }
}

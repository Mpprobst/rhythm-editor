using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UILayoutReorder : UIDrag
{

    private RectTransform placeholderRect;
    private LayoutElement layoutElement;
    private Vector2 constrainAxis;
    private LayoutGroup parentGroup;
    private List<Transform> siblings = new List<Transform>();

    // create a temporary layout element so this can ignore its layout group (maybe I can temporarily remove it from parent instead?)
    // then position the dummy in the list where this will go so we can see the separation in realtime
    // may want this to trigger some event so we can tell the controlling classes that the order is different now

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        parentGroup = rxForm.parent.GetComponent<LayoutGroup>();
        if (parentGroup == null)
        {
            Debug.LogWarning("Cannot use the UILayoutReorderer on an element that does not have a Layout Group in its immediate parent " + name);
            return;
        }
        if (parentGroup.GetType() == typeof(HorizontalLayoutGroup))
        {
            constrainAxis = Vector2.right;
            if (parentGroup.childAlignment.ToString().Contains("Right"))
                constrainAxis *= -1;
            if ((parentGroup as HorizontalLayoutGroup).reverseArrangement)
                constrainAxis *= -1;
        }
        else if (parentGroup.GetType() == typeof(VerticalLayoutGroup))
        {
            constrainAxis = Vector2.up;
            if (parentGroup.childAlignment.ToString().Contains("Upper"))
                constrainAxis *= -1;
            if ((parentGroup as VerticalLayoutGroup).reverseArrangement)
                constrainAxis *= -1;
        }
        else constrainAxis = Vector2.one;   // might be a grid? not explicitly supporting it but it could work?
        
        siblings = Utils.GetImmediateChildren(parentGroup.transform);

        layoutElement = gameObject.AddComponent<LayoutElement>();
        layoutElement.ignoreLayout = true;
        GameObject placeHolder = new GameObject("placeholder", typeof(RectTransform));
        placeholderRect = placeHolder.GetComponent<RectTransform>();
        placeholderRect.SetParent(parentGroup.transform);
        // use a layout element with preferred dims to force the element to always be the size of the dragged rect
        // just setting the size is not a guarantee on some layout groups
        LayoutElement placeholderElement = placeholderRect.AddComponent<LayoutElement>();
        placeholderElement.preferredHeight = rxForm.rect.height;
        placeholderElement.preferredWidth = rxForm.rect.width;
        placeholderElement.flexibleHeight = 0;
        placeholderElement.flexibleWidth = 0;
        placeholderRect.sizeDelta = rxForm.sizeDelta;
        placeholderRect.anchoredPosition = rxForm.anchoredPosition;
        placeholderRect.SetSiblingIndex(rxForm.GetSiblingIndex());

        rxForm.SetAsLastSibling();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        rxForm.SetSiblingIndex(placeholderRect.GetSiblingIndex());
        Destroy(layoutElement);
        Destroy(placeholderRect.gameObject);
        parentGroup = null;
        siblings.Clear();
    }

    protected override void Update()
    {
        base.Update();
        // reorder the children
        if (isDragging)
        {
            for (int i = 0; i < siblings.Count; i++)
            {
                if (siblings[i].GetComponent<UIReorderIgnore>()) continue;

                if (siblings[i] != rxForm.transform && siblings[i].transform != placeholderRect.transform)
                {

                    float sibDist = Utils.Vector2Sum(Vector2.Scale(siblings[i].GetComponent<RectTransform>().anchoredPosition, constrainAxis));
                    float dragDist = Utils.Vector2Sum(Vector2.Scale(rxForm.anchoredPosition, constrainAxis));
                    Debug.Log($"{i} - {dragDist - sibDist}");
                    // if the dragged element is definitively further away from this index then it should go there.
                    // Doing this at the first sibling found because we want to insert at the inflection where its futher than a sibling but closer than the rest
                    if (dragDist - sibDist > 0) 
                    {
                        // we have found our new spot
                        //placeholderRect.SetSiblingIndex(i);
                        // TODO: figure this out with debugs to know the right move here
                        //return;
                        // the layout group should handle positioning from here
                    }
                }
            }
        }
    }
}

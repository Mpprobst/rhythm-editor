using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Drawing;

public static class LayoutGenerator 
{
    public static void CreateLayout(LayoutData layoutData, ref UILayout layout, Transform parent = null)
    {
        if (layout == null)
        {
            // create a new gameobject. spawwn the base layoOut prefab
            UnityEngine. Object layoutPrefab = layoutData.layoutPrefab;
            if (layoutPrefab == null) layoutPrefab = Resources.Load<UnityEngine.Object>("UI/Layouts/LayoutBase");
            GameObject layoutObj = (GameObject)PrefabUtility.InstantiatePrefab(layoutPrefab, parent);
            layout = layoutObj.GetComponent<UILayout>();
        }

        List<UIElement_Popout> existingPopouts = layout.GetComponentsInChildren<UIElement_Popout>().ToList();

        SpawnLayoutGroup(layoutData.primaryGroupData, layout.primaryRibbon, layout, ref existingPopouts);
        SpawnLayoutGroup(layoutData.secondaryGroupData, layout.secondaryRibbon, layout, ref existingPopouts);

        layout.SetStyles(layoutData.defaultPalette);
        layout.SetElementColors();
        layout.SetElementStyle();

        // spawn the sublayout
        if (layout.window)
        {
            UILayout sublayout = layout.window.GetComponentInChildren<UILayout>();
            if (layoutData.subLayout != null)
            {
                CreateLayout(layoutData.subLayout, ref sublayout, layout.window);
                EditorUtil.SaveObjectAsPrefab(layout.gameObject, "Assets/Resources/UI/Layouts");    
            }
        }
    }

    private static void SpawnLayoutGroup(LayoutGroupData[] groupData, Transform ribbon, UILayout layout, ref List<UIElement_Popout> existingPopouts)
    {
        List<UIElement> existingElements = ribbon.GetComponentsInChildren<UIElement>().ToList();
        ScreenSide screenSide = ScreenSide.LEFT;    // figure this out by screen position
        RectTransform ribbonRect = ribbon.GetComponent<RectTransform>();
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        // TODO: involve the canvas scaler
        Canvas canvas = layout.GetComponentInParent<Canvas>();
        if (canvas)
            screenSize = canvas.renderingDisplaySize;   // because this is how transforms determine their position

        // whichever axis is closer to an extent we should use
        Vector2 posDiff = new Vector2(ribbonRect.position.x, ribbonRect.position.y) - screenSize/2f;    // using center of screen because negative values clearly indicate our quadrant of screen
        posDiff = new Vector2(posDiff.x / screenSize.x, posDiff.y / screenSize.y);  // scales so our comparison between axes is fair
        if (Mathf.Abs(posDiff.x) < Mathf.Abs(posDiff.y))
        {
            // we are closer to a top or bottom
            if (posDiff.y < 0) screenSide = ScreenSide.BOTTOM;
            else screenSide = ScreenSide.TOP;
        }
        else 
        {
            if (posDiff.x > 0)    
                screenSide = ScreenSide.RIGHT;
        }

        // spawn all the things
        for (int i = 0; i < groupData.Length; i++)
        {
            LayoutAlignment alignment = groupData[i].alignment;
            UIButtonGroup[] buttonGroups = ribbon.GetComponentsInChildren<UIButtonGroup>();
            Transform container = ribbon;
            if (buttonGroups.Length > 0)
            {
                int groupIdx = Mathf.Min(buttonGroups.Length - 1, (int)alignment);
                container = ribbon.GetChild(groupIdx).transform;
            }
            UIButtonGroup buttonGroup = container.GetComponent<UIButtonGroup>();
            if (buttonGroup)
                buttonGroup.allowMultiple = groupData[i].allowMultipleActiveButtons;
            
            // try to get this button already
            for (int j = 0; j < groupData[i].buttonData.Length; j++)
            {
                UIElementData elementData = groupData[i].buttonData[j];
                Type elementType = Type.GetType($"UIElement_{Utils.ToHumanReadable(elementData.elementType)}");
                UIElement element = GetChildElement(container, elementData.elementName, elementType);

                if (element == null)
                {
                    // spawn the prefab
                    GameObject elementObj = (GameObject)PrefabUtility.InstantiatePrefab(elementData.elementPrefab, container);
                    element = elementObj.GetComponent<UIElement>();
                }

                if (existingElements.Contains(element))
                    existingElements.Remove(element);

                // update the element
                element.SetInfo(elementData, alignment, screenSide);
                element.transform.SetSiblingIndex(i);
                element.name = $"{elementType.Name}_{elementData.elementName}";

                // if a popout, we need to find it or spawn it
                if (elementData.elementType == UIElementType.POPOUT)
                {
                    UIElement_PopoutButton popoutButton = element as UIElement_PopoutButton;
                    SpawnPopout(elementData, layout.popoutContainer, ref popoutButton.popout);
                    if (existingPopouts.Contains(popoutButton.popout))
                        existingPopouts.Remove(popoutButton.popout);
                    popoutButton.popout.onClose = new UnityEngine.Events.UnityEvent();
                    //popoutButton.popout.onClose.AddListener(delegate { popoutButton.SetActiveNoNotify(false); });   // no notify because the popout already knows it is being closed

                    RectTransform popoutRXForm = popoutButton.popout.GetComponent<RectTransform>();
                    Vector2 anchorPivot = Utils.GetAnchorFromAlignment(screenSide, alignment);

                    popoutRXForm.SetParent(popoutButton.transform);
                    popoutRXForm.anchorMin = anchorPivot;
                    popoutRXForm.anchorMax = anchorPivot;
                    popoutRXForm.pivot = anchorPivot;

                    // this is getting pretty close, might need to force the conetent size fitter to update first
                    RectTransform buttonRect = popoutButton.GetComponent<RectTransform>();
                    Vector2 popoutPos = Vector2.Scale(buttonRect.rect.size, buttonRect.pivot);// + popoutRXForm.rect.size / 2f;

                    popoutRXForm.anchoredPosition = popoutPos;

                    popoutRXForm.SetParent(layout.popoutContainer);
                    popoutRXForm.localScale = Vector3.one;
                }
            }

            // delete old popouts
            foreach (var popout in existingPopouts)
                if (popout)
                    GameObject.DestroyImmediate(popout.gameObject);

            // delete buttons that exist but were not found
            foreach (var existing in existingElements)
                if (existing)
                    GameObject.DestroyImmediate(existing.gameObject);

        }
    }

    private static void SpawnPopout(UIElementData data, Transform popoutContainer, ref UIElement_Popout popout)
    {
        if (popout == null)
        {
            if (data.popoutPrefab == null)
            {
                Debug.LogError($"could not spawn popout for {data.elementName} because its popout prefab was not set");
                return;
            }
            GameObject popoutObj = (GameObject)PrefabUtility.InstantiatePrefab(data.popoutPrefab, popoutContainer);
            popout = popoutObj.GetComponent<UIElement_Popout>();
            
            if (popout == null)
            {
                Debug.LogError($"could not spawn popout for {data.elementName} because its popout prefab did not inherit from UIElement_Popout");
                GameObject.DestroyImmediate(popoutObj);
                return;
            }
        }

        List<PopoutOption> existingOptions = popout.content.GetComponentsInChildren<PopoutOption>().ToList();
        popout.SetInfo(data.elementName);
        popout.name = $"{data.elementName}_Popout";

        for (int i = 0; i < data.popoutOptions.Length; i++)
        {
            Type optionType = Type.GetType($"PopoutOption_{Utils.ToHumanReadable(data.popoutOptions[i].optionType)}");
            PopoutOption option = popout.GetOption(data.popoutOptions[i].optionName, optionType);
            SpawnOption(data.popoutOptions[i], ref option, popout.content);

            option.transform.SetSiblingIndex(i);
            option.name = $"{optionType.Name}_{data.popoutOptions[i].optionName}";

            if (existingOptions.Contains(option))
                existingOptions.Remove(option);
        }

        foreach (var existing in existingOptions)
            GameObject.DestroyImmediate(existing);

        LayoutRebuilder.ForceRebuildLayoutImmediate(popout.content.GetComponent<RectTransform>());
        EditorUtil.SaveObjectAsPrefab(popout.gameObject, "Assets/Resources/UI/Popouts");
    }

    private static void SpawnOption(ElementInputOptionData optionData, ref PopoutOption option, Transform parent)
    {        
        if (option == null)
        {
            GameObject optionObj = (GameObject)PrefabUtility.InstantiatePrefab(optionData.optionPrefab, parent);
            option = optionObj.GetComponent<PopoutOption>();
            if (option == null)
            {
                Debug.LogError($"could not spawn {optionData.optionName} because it did not derive from PopoutOption");
                GameObject.DestroyImmediate(optionObj);
                return;
            }
        }
        option.SetInfo(optionData);
        option.SetOptionHeight();
    }

    private static UIElement GetChildElement(Transform transform, string name, Type type)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            UIElement element = transform.GetChild(i).GetComponent<UIElement>();
            if (element && element.GetType() == type && element.Name.Contains(name))
                return element;
        }
        return null;
    }
}
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Drawing;
using System.Xml.Linq;

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
        if (layout.primaryRibbon && layout.primaryRibbon.TryGetComponent<UIRibbon>(out UIRibbon uiRibbon))
            uiRibbon.isPrimary = true;
        SpawnLayoutGroup(layoutData.secondaryGroupData, layout.secondaryRibbon, layout, ref existingPopouts);
        if (layout.secondaryRibbon && layout.secondaryRibbon.TryGetComponent<UIRibbon>(out UIRibbon uiRibbon2))
            uiRibbon2.isPrimary = true;

        layout.SetStyles(layoutData.defaultPalette);
        layout.SetElementColors();
        layout.SetElementStyle();

        // spawn the sublayout
        if (layout.window)
        {
            UILayout sublayout = layout.window.GetComponentInChildren<UILayout>();
            if (layoutData.subLayout != null)
            {
                var tempStyle = layoutData.subLayout.defaultPalette;
                layoutData.subLayout.defaultPalette = layoutData.defaultPalette;    // trickle the style down
                CreateLayout(layoutData.subLayout, ref sublayout, layout.window);
                EditorUtil.SaveObjectAsPrefab(layout.gameObject, "Assets/Resources/UI/Layouts");
                layoutData.subLayout.defaultPalette = tempStyle;    // restore so developer isn't confused 
            }
        }
    }

    private static void SpawnLayoutGroup(LayoutGroupData[] groupData, Transform ribbon, UILayout layout, ref List<UIElement_Popout> existingPopouts)
    {
        List<UIElement> existingElements = ribbon.GetComponentsInChildren<UIElement>().ToList();
        RectTransform ribbonRect = ribbon.GetComponent<RectTransform>();
        ScreenSide screenSide = Utils.ScreenSideOfElement(ribbonRect);
        // keeps all children equal width
        UIButtonGroup[] buttonGroups = ribbon.GetComponentsInChildren<UIButtonGroup>();
        if (buttonGroups.Length > 0)
        {
            Vector2 size = ribbon.GetComponent<RectTransform>().rect.size / ribbon.childCount - Vector2.one * 20f;  // 20 is a safe buffer
            for (int i = 0; i < ribbon.childCount; i++)
            {
                ribbon.GetChild(i).GetComponent<RectTransform>().sizeDelta = size;
            }
        }

        

        // spawn all the things
        for (int i = 0; i < groupData.Length; i++)
        {
            LayoutAlignment alignment = groupData[i].alignment;
            Transform container = ribbon;
            if (buttonGroups.Length > 0)
            {
                int groupIdx = Mathf.Min(buttonGroups.Length - 1, (int)alignment);
                //Debug.Log($"{container.name} has groupidx {groupIdx}");
                container = buttonGroups[groupIdx].transform;
            }
            UIButtonGroup buttonGroup = container.GetComponent<UIButtonGroup>();
            if (buttonGroup)
                buttonGroup.allowMultiple = groupData[i].allowMultipleActiveButtons;
            
            // try to get this button already
            for (int j = 0; j < groupData[i].buttonData.Length; j++)
            {
                UIElementData elementData = groupData[i].buttonData[j];
                UIElement element = SpawnElement(elementData, container, layout, alignment, screenSide);
                
                if (existingElements.Contains(element))
                    existingElements.Remove(element);

                if (elementData.elementType == UIElementType.POPOUT)
                    if (existingPopouts.Contains((element as UIElement_PopoutButton).popout))
                        existingPopouts.Remove((element as UIElement_PopoutButton).popout);
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

    public static UIElement SpawnElement(UIElementData elementData, Transform container, UILayout layout, LayoutAlignment alignment, ScreenSide screenSide)
    {
        Type elementType = Type.GetType($"UIElement_{Utils.ToHumanReadable(elementData.elementType)}");
        UIElement element = layout.GetElement(elementData.elementName, elementType, container);

        if (element == null)
        {
            // spawn the prefab
            GameObject elementObj = (GameObject)PrefabUtility.InstantiatePrefab(elementData.elementPrefab, container);
            element = elementObj.GetComponent<UIElement>();
        }

        // update the element
        element.SetInfo(elementData, alignment, screenSide);
        element.transform.SetAsLastSibling();
        element.name = $"{elementType.Name}_{elementData.elementName}";

        // if a popout, we need to find it or spawn it
        if (elementData.elementType == UIElementType.POPOUT)
        {
            UIElement_PopoutButton popoutButton = element as UIElement_PopoutButton;
            SpawnPopout(elementData, layout.popoutContainer, ref popoutButton.popout);

            popoutButton.popout.onClose = new UnityEngine.Events.UnityEvent();
            //popoutButton.popout.onClose.AddListener(delegate { popoutButton.SetActiveNoNotify(false); });   // no notify because the popout already knows it is being closed

            popoutButton.SetPopoutPos();
        }
        return element;
    }

    public static void SpawnPopout(UIElementData data, Transform popoutContainer, ref UIElement_Popout popout)
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
}
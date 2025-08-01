using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LayoutData", menuName = "ScriptableObjects/UI/LayoutData", order = 0)]
public class LayoutData : ScriptableObject
{
    public LayoutGroupData[] primaryGroupData;
    public LayoutGroupData[] secondaryGroupData;

    public LayoutData subLayout;
    public GameObject layoutPrefab;

    public UIStyleData defaultPalette;
}


public enum LayoutAlignment { LEFT, CENTER, RIGHT };
public enum ScreenSide { LEFT, RIGHT, TOP, BOTTOM };    // Determined by the primary or secondary group

[Serializable]
public class LayoutGroupData
{
    public string groupName;
    public LayoutAlignment alignment;    
    public UIElementData[] buttonData;
    public bool allowMultipleActiveButtons;

}

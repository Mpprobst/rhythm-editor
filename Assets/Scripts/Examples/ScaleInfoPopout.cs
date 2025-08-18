using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleInfoPopout : UIElement_Popout
{
    private PopoutOption_Text positionText, anchoredPositionText, sizeText, sizeDeltaText, anchorMinText, anchorMaxText, pivotText;

    protected override void Start()
    {
        base.Start();

        positionText = GetOption<PopoutOption_Text>("Position");
        anchoredPositionText = GetOption<PopoutOption_Text>("Anchored Position");
        sizeText = GetOption<PopoutOption_Text>("Size");
        sizeDeltaText = GetOption<PopoutOption_Text>("Size Delta");
        anchorMinText = GetOption<PopoutOption_Text>("Anchor Min");
        anchorMaxText = GetOption<PopoutOption_Text>("Anchor Max");
        pivotText = GetOption<PopoutOption_Text>("Pivot");
    }


}

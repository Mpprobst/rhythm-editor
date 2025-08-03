using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleFileBrowser;

public class PopoutOption_File : PopoutOption_Action
{
    // display a button that opens a file browser when clicked
    // based on file type, a differnt preview icon can be shown
    public FileType fileType;
    [Tooltip("")] public Sprite searchSprite;

    public override void SetInfo(ElementInputOptionData info)
    {
        base.SetInfo(info);
        // if no icon, just show a search sprite
    }

    protected override void Awake()
    {
        base.Awake();
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Images", ".jpg", ".png"), new FileBrowser.Filter("Text Files", ".txt", ".pdf"));
        FileBrowser.AddQuickLink("Default Assets", Application.persistentDataPath, null);
    }

    // when an item is selected, the Action icon is replaced with the proper content
    // if the item is not an image, we just display the name of the file
    protected override void OnClick()
    {
        // open the file browser
        // show files from the active directory?
        // would be nice to have some default assets from Streaming Assets I think
        base.OnClick();
        FileBrowser.ShowLoadDialog( ( paths ) => { Debug.Log( "Selected: " + paths[0] ); },
        						   () => { Debug.Log( "Canceled" ); },
        						   FileBrowser.PickMode.Folders, false, null, null, "Select Folder", "Select" );
    }


    public override void SetColors(UIStyleData style)
    {
        base.SetColors(style);
    }

    public override void SetStyle(UIStyleData style)
    {
        base.SetStyle(style);
    }

}

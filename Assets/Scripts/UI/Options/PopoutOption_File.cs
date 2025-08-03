using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleFileBrowser;
using Unity.VisualScripting;

public class PopoutOption_File : PopoutOption_Action
{
    [SerializeField] protected RawImage loadedImage;
    // display a button that opens a file browser when clicked
    // based on file type, a differnt preview icon can be shown
    public FileType fileType;

    public override void SetInfo(ElementInputOptionData info)
    {
        fileType = info.fileType;
        info.icon = null;
        info.description = "Please select a " + Utils.ToHumanReadable(fileType);
        base.SetInfo(info);
        label.gameObject.SetActive(true);
        icon.sprite = Resources.Load<Sprite>($"Sprites/{fileType.ToString()}");
        icon.gameObject.SetActive(true);
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
        FileBrowser.ShowLoadDialog(FileSelected,
        						   () => { Debug.Log( "Canceled" ); },
        						   FileBrowser.PickMode.FilesAndFolders, false, null, null, "Select Folder", "Select" );
    }

    private void FileSelected(string[] paths)
    {
        if (fileType == FileType.IMAGE)
        {
            Texture2D tex = new Texture2D(1, 1);
            tex.LoadImage(File.ReadAllBytes(paths[0]));
            float w = tex.width;
            float h = tex.height;
            float aspect = w / h;
            AspectRatioFitter arFitter = icon.GetComponent<AspectRatioFitter>();    
            if (arFitter)
            {
                AspectRatioFitter.AspectMode aspectMode = AspectRatioFitter.AspectMode.WidthControlsHeight;
                if (aspect > 1)
                {
                    aspect = 1f / aspect;
                    aspectMode = AspectRatioFitter.AspectMode.HeightControlsWidth;
                }
                arFitter.aspectMode = aspectMode;
                arFitter.aspectRatio = aspect;
            }

            icon.gameObject.SetActive(false);
            loadedImage.gameObject.SetActive(true);
            loadedImage.texture = tex;
            
            description.text = Path.GetFileName(paths[0]);
        }
        else
        {
            icon.gameObject.SetActive(true);
            loadedImage.gameObject.SetActive(false);
        }

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

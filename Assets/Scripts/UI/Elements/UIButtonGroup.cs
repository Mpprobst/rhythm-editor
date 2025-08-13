using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// only allows one of each element to be active at once
// TODO: allow buttons to be dragged and reordered
public class UIButtonGroup : MonoBehaviour
{
    public bool allowMultiple = false;
    protected UIElement_Button[] buttons;

    // Start is called before the first frame update
    void Start()
    {
        buttons = GetComponentsInChildren<UIElement_Button>();
        for(int i = 0; i <  buttons.Length; i++)
        {
            buttons[i].id = i;
            buttons[i].onActivate.AddListener(ButtonSelect);
        }
    }

    private void ButtonSelect(bool val)
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

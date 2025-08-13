using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Text = TMPro.TextMeshProUGUI;
using DG.Tweening;

public class MessageModal : MonoBehaviour, IUIStyle
{
    private static List<MessageModal> messages = new List<MessageModal>();
    private static int maxModals = 3;

    [SerializeField] private Image backroundImage;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Text headerText, messageText;
    [SerializeField] private Button button1, button2;
    [SerializeField] private Text buttonText1, buttonText2;

    private Action option1Callback, option2Callback;
    private int modalIdx = 0;
    private bool isShowing;

    public static void ShowMessage(string title, string message, string option1 = "OK", Action callback1 = null, string option2 ="", Action callback2 = null)
    {
        MessageModal modal = GetModal();

        modal.headerText.text = title;
        modal.messageText.text = message;

        if (!string.IsNullOrEmpty(option1))
        {
            modal.buttonText1.text = option1;
            modal.button1.onClick.AddListener(modal.Option1Pressed);
            modal.option1Callback = callback1;
        }
        else modal.button1.gameObject.SetActive(false);

        if (!string.IsNullOrEmpty(option2))
        {
            modal.buttonText2.text = option2;
            modal.button2.onClick.AddListener(modal.Option2Pressed);
            modal.option2Callback = callback2;
        }
        else modal.button2.gameObject.SetActive(false);

        modal.Open();
    }

    private static MessageModal GetModal()
    {
        // return an available message
        for (int i = 0; i < messages.Count; i++)
        {
            if (!messages[i].isShowing)
                return messages[i]; 
        }

        // if we get here, there are no avaiable 
        if (messages.Count < maxModals)
        {
            var prefab = Resources.Load<GameObject>("UI/MessageModal");
            var go = Instantiate(prefab, Vector3.zero, Quaternion.identity, Global.Instance.mainCanvas.transform);
            MessageModal modal = go.GetComponent<MessageModal>();
            messages.Add(modal);
            modal.SetColors(Global.Instance.mainLayout.Style);
            modal.SetStyle(Global.Instance.mainLayout.Style);
            return modal;
        }

        // at this point, there are no more available modals so we can just erase the first one then move it to the back of the queue
        MessageModal oldest = messages[0];
        messages.RemoveAt(0);
        messages.Add(oldest);
        return oldest;
    }

    private void Option1Pressed()
    {
        if (option1Callback != null)
            option1Callback.Invoke();
        Close();
    }

    private void Option2Pressed()
    {
        if (option2Callback != null)
            option2Callback.Invoke();
        Close();
    }

    private void Open()
    {
        isShowing = true;
        transform.SetAsLastSibling();   // puts this in front
        backroundImage.rectTransform.anchoredPosition = Vector2.zero;
        backroundImage.transform.localScale = new Vector3(0, 1, 1);
        backroundImage.transform.DOScaleX(1, 0.2f);
        canvasGroup.DOFade(1, 0.2f);
    }

    private void Close()
    {
        isShowing = false;

        canvasGroup.DOFade(0, 0.2f);
        backroundImage.transform.localScale = new Vector3(1, 1, 1);
        backroundImage.transform.DOScaleX(0, 0.2f);
    }

    public void SetColors(UIStyleData style)
    {
        backroundImage.color = style.backgroundColor;
        headerText.color = style.textColor_primary;
        messageText.color = style.textColor_secondary;
        button1.image.color = style.backgroundColor_highlight;
        button2.image.color = style.backgroundColor_secondary;
        buttonText1.color = style.iconColor;
        buttonText2.color = style.textColor_secondary;
    }

    public void SetStyle(UIStyleData style)
    {
        backroundImage.sprite = style.backgroundSprite;
        headerText.font = style.font;
        messageText.font = style.font;
        button1.image.sprite = style.buttonSprite;
        button2.image.sprite = style.buttonSprite;
        buttonText1.font = style.font;
        buttonText2.font = style.font;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Text = TMPro.TextMeshProUGUI;

public class RectTransformLogger : MonoBehaviour
{
    [SerializeField] private RectTransform target;

    [Header("output texts")]
    [SerializeField] private Text posText;
    [SerializeField] private Text anchorPosText, sizeText, sizeDeltaText, anchorMinText, anchorMaxText, pivotText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (target)
        {
            posText.text =$"{target.position}";
            anchorPosText.text =$"{target.anchoredPosition}";
            sizeText.text =$"{target.rect.size}";
            sizeDeltaText.text =$"{target.sizeDelta}";
            anchorMinText.text =$"{target.anchorMin}";
            anchorMaxText.text =$"{target.anchorMax}";
            pivotText.text =$"{target.pivot}";
            Debug.Log($"{name} pos: {target.position}, pivot {target.pivot} anchored: {target.anchoredPosition}");
            Debug.Log($"{name} sizedelta: {target.sizeDelta}, anchorMinMax ({target.anchorMin},{target.anchorMax}) overall size: {target.rect.size}");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ButtonAnimation : MonoBehaviour
{
    [SerializeField] private Button button;

    Sequence pulseSequence;

    public void Pop()
    {
        // sequence 
        var sequence = DOTween.Sequence();
        sequence.Append(button.transform.DOScale(1.1f, 0.1f));
        sequence.Append(button.transform.DOScale(1f, 0.2f));
        sequence.Play();
    }

    public void Pulse()
    {
        // dgtween loop
        if (pulseSequence != null)
        {
            pulseSequence.Kill();
            pulseSequence =  null;
            return;
        }
        pulseSequence = DOTween.Sequence();
        pulseSequence.Append(button.transform.DOScale(1.1f, 0.25f));
        pulseSequence.Append(button.transform.DOScale(1f, 0.25f));
        pulseSequence.SetLoops(-1, LoopType.Restart);
        pulseSequence.Play();
    }

    private Color RandomColor()
    {
        return new Color(
            Random.Range(0f, 1f),
            Random.Range(0f, 1f),
            Random.Range(0f, 1f));
    }

    public void ColorChange()
    {
        button.image.DOColor(RandomColor(), 1f);
    }


}

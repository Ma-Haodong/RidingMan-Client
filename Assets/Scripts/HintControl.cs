using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
//Popover prompt control class
[RequireComponent(typeof(CanvasGroup))]
public class HintControl : Singleton<HintControl>
{
    private Sequence showSequence;
    private CanvasGroup thisCanvasGroup;
    [SerializeField]
    private Transform frameT;
    [SerializeField]
    private Text messageText;
    [SerializeField]
    private Button confirmButton;
    private UnityAction onConfirmed;
    // Start is called before the first frame update
    void Start()
    {
        showSequence = DOTween.Sequence().SetAutoKill(false).Pause();
        thisCanvasGroup = this.GetComponent<CanvasGroup>();
        thisCanvasGroup.alpha = 0f;
        thisCanvasGroup.blocksRaycasts = false;
        showSequence.Join(thisCanvasGroup.DOFade(1f, 0.3f));
        frameT.localScale = Vector3.zero;
        showSequence.Join(frameT.DOScale(1f, 0.3f));
        showSequence.onStepComplete += () =>
        {
            if (showSequence.isBackwards)
            {
                thisCanvasGroup.blocksRaycasts = false;
            }
        };

        confirmButton.onClick.AddListener(() =>
        {
            Hide();
            onConfirmed?.Invoke();
        });
    }
    public void Show(string message, UnityAction onConfirmed = null)
    {
        thisCanvasGroup.blocksRaycasts = true;
        showSequence.PlayForward();
        messageText.text = message;
        this.onConfirmed = onConfirmed;
    }
    public void Hide()
    {
        showSequence.PlayBackwards();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
//Page control basic class, control page animation, show and hide
[RequireComponent(typeof(CanvasGroup))]
public class PageControl<T> : Singleton<T> where T : MonoBehaviour
{
    protected CanvasGroup thisCanvasGroup;
    public bool IsActive
    {
        get
        {
            return thisCanvasGroup.alpha == 1f;
        }
    }
    protected Sequence showSequence;
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        float scaleFactor = this.GetComponentInParent<Canvas>().scaleFactor;
        this.transform.localPosition += new Vector3(Screen.width / scaleFactor, 0f);
        thisCanvasGroup = this.GetComponent<CanvasGroup>();
        thisCanvasGroup.alpha = 0f;
        showSequence = DOTween.Sequence().SetAutoKill(false).Pause();
        showSequence.Join(this.transform.DOLocalMoveX(this.transform.localPosition.x - Screen.width / scaleFactor, 0.3f));
        showSequence.Join(thisCanvasGroup.DOFade(1f, 0.3f));
    }
    public virtual void Show()
    {
        showSequence.PlayForward();
    }
    public virtual void Hide()
    {
        showSequence.PlayBackwards();
    }
}
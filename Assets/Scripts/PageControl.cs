using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class PageControl<T> : Singleton<T> where T : MonoBehaviour
{
    protected CanvasGroup thisCanvasGroup;
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        thisCanvasGroup = this.GetComponent<CanvasGroup>();
    }
    public virtual void Show()
    {
        thisCanvasGroup.alpha = 1f;
        thisCanvasGroup.blocksRaycasts = true;
    }
    public virtual void Hide()
    {
        thisCanvasGroup.alpha = 0f;
        thisCanvasGroup.blocksRaycasts = false;
    }
}
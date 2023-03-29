using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
//Sport ready control class
public class ReadyControl : MonoBehaviour
{
    private Sequence readySequence;
    [SerializeField]
    private Text tipsText;
    [SerializeField]
    private Transform infoT;
    private UnityEvent onEnded = new UnityEvent();
    public UnityEvent OnEnded
    {
        get
        {
            return onEnded;
        }
    }
    // Start is called before the first frame updatesss
    void Start()
    {
        //Controls countdown animation sequences and related events
        readySequence = DOTween.Sequence().SetAutoKill(false).Pause();
        readySequence.AppendInterval(0.3f);
        tipsText.transform.localScale = Vector3.zero;
        for (int i = 3; i > 0; i--)
        {
            int num = i;
            readySequence.AppendCallback(() => tipsText.text = num.ToString());
            readySequence.Append(tipsText.transform.DOScale(2f, 0.4f));
            readySequence.AppendInterval(0.2f);
            readySequence.Append(tipsText.transform.DOScale(0f, 0.4f));
        }
        readySequence.AppendCallback(() => tipsText.text = "Go");
        readySequence.Append(tipsText.transform.DOScale(2f, 0.4f));
        readySequence.AppendInterval(0.2f);
        readySequence.Append(tipsText.transform.DOScale(0f, 0.4f));
        readySequence.Append(this.transform.DOLocalMoveY(this.transform.localPosition.y - Screen.height / this.GetComponentInParent<Canvas>().scaleFactor, 0.3f));
        readySequence.Append(infoT.DOLocalMoveY(400f, 0.3f));
        readySequence.AppendCallback(() => this.GetComponent<AudioSource>().Play());
        readySequence.AppendCallback(() => onEnded.Invoke());
    }
    public void Show()
    {
        infoT.localPosition = Vector3.zero;
        readySequence.Restart();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
//Sport information control class
public class InfoControl : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField]
    private Button shareLocationButton;
    [SerializeField]
    private Button stopShareLocationButton;
    [SerializeField]
    private Text topMileageText;
    [SerializeField]
    private GameObject fullG;
    [SerializeField]
    private Text centerMileageText;
    [SerializeField]
    private Text speedText;
    [SerializeField]
    private Text timeText;
    [SerializeField]
    private Text calorieText;
    [SerializeField]
    private Button pauseButton;
    [SerializeField]
    private Button startButton;
    [SerializeField]
    private Button stopButton;
    private Tweener gotoTweener;
    // Start is called before the first frame update
    void Start()
    {
        shareLocationButton.onClick.AddListener(() =>
        {
            shareLocationButton.gameObject.SetActive(false);
            stopShareLocationButton.gameObject.SetActive(true);
        });

        stopShareLocationButton.onClick.AddListener(() =>
        {
            stopShareLocationButton.gameObject.SetActive(false);
            shareLocationButton.gameObject.SetActive(true);
        });

        pauseButton.onClick.AddListener(() =>
        {
            pauseButton.gameObject.SetActive(false);
            startButton.gameObject.SetActive(true);
            stopButton.gameObject.SetActive(true);
            SportControl.Instance.PauseSport();
        });

        startButton.onClick.AddListener(() =>
        {
            pauseButton.gameObject.SetActive(true);
            startButton.gameObject.SetActive(false);
            stopButton.gameObject.SetActive(false);
            SportControl.Instance.StartSport();
        });

        stopButton.onClick.AddListener(() =>
        {
            SportControl.Instance.GetComponentInChildren<AudioSource>().Stop();
            SportControl.Instance.Hide();
        });
    }
    //Pointer Down Event
    public void OnPointerDown(PointerEventData eventData)
    {
        if (gotoTweener != null)
        {
            gotoTweener.Kill();
        }
    }
    //Pointer Up Event
    public void OnPointerUp(PointerEventData eventData)
    {
        gotoTweener = this.transform.DOLocalMoveY(this.transform.localPosition.y < 750f ? 400f : 1500f, 0.3f);
        gotoTweener.onUpdate += () =>
        {
            if (this.transform.localPosition.y < 600f)
            {
                topMileageText.gameObject.SetActive(true);
                fullG.SetActive(false);
            }
            else
            {
                topMileageText.gameObject.SetActive(false);
                fullG.SetActive(true);
            }
        };
    }
    //Drag Event
    public void OnDrag(PointerEventData eventData)
    {
        this.transform.localPosition += new Vector3(0f, eventData.delta.y);

        if (this.transform.localPosition.y < 400f)
        {
            this.transform.localPosition = new Vector3(0f, 400f);
        }
        else if (this.transform.localPosition.y > 1500f)
        {
            this.transform.localPosition = new Vector3(0, 1500f);
        }

        if (this.transform.localPosition.y < 600f)
        {
            topMileageText.gameObject.SetActive(true);
            fullG.SetActive(false);
        }
        else
        {
            topMileageText.gameObject.SetActive(false);
            fullG.SetActive(true);
        }
    }
    public void Show()
    {
        shareLocationButton.gameObject.SetActive(true);
        stopShareLocationButton.gameObject.SetActive(false);
        topMileageText.gameObject.SetActive(true);
        fullG.SetActive(false);
        pauseButton.gameObject.SetActive(true);
        startButton.gameObject.SetActive(false);
        stopButton.gameObject.SetActive(false);
    }
    public void UpdateInfo(double distance, double time)
    {
        topMileageText.text = distance.ToString("f2");
        centerMileageText.text = distance.ToString("f2");
        double speed = distance / (time / 3600);
        speedText.text = $"{(double.IsInfinity(speed) ? "0.0" : speed.ToString("f1"))}km/h";
        timeText.text = TimeSpan.FromSeconds(time).ToString(@"hh\:mm\:ss");
        calorieText.text = $"{(distance * 20f).ToString("f1")}k";
    }
}

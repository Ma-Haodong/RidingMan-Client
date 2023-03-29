using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using BestHTTP;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
//sport control class
public class SportControl : PageControl<SportControl>
{
    [SerializeField]
    private OnlineMaps onlineMaps;
    private RawImage onlineMapsRawImage;
    private OnlineMapsMarker selfMarker;
    private List<Vector2> pathPointsList = new List<Vector2>();
    [SerializeField]
    private Button locateButton;
    [SerializeField]
    private InfoControl infoControl;
    [SerializeField]
    private ReadyControl readyControl;
    private bool isRunning;
    private float updatePosTime;
    private Vector2 lastPos;
    private double distance;
    private DateTime startTime;
    private double time;
    // Start is called before the first frame update
    void Start()
    {
        onlineMapsRawImage = onlineMaps.GetComponent<RawImage>();
        onlineMapsRawImage.enabled = false;
        showSequence.onStepComplete += () =>
        {
            onlineMapsRawImage.enabled = !showSequence.isBackwards;
        };

        //Create map markers and path elements
        selfMarker = OnlineMapsMarkerManager.CreateItem(Vector2.zero);
        OnlineMapsDrawingElementManager.AddItem(new OnlineMapsDrawingLine(pathPointsList, Color.green, 5));
        onlineMaps.GetComponent<RectTransform>().sizeDelta = Vector2.one * Screen.height / this.GetComponentInParent<Canvas>().scaleFactor;

        locateButton.onClick.AddListener(() =>
        {
            onlineMaps.position = lastPos;
        });

        readyControl.OnEnded.AddListener(() =>
        {
            lastPos = Vector2.zero;
            distance = 0d;
            startTime = DateTime.Now;
            time = 0d;
            infoControl.UpdateInfo(distance, time);
            isRunning = true;
            Input.location.Start();
        });
    }
    // Update is called once per frame
    void Update()
    {
        //After location is enabled, update the current user marker location and redraw the path
        if (Input.location.isEnabledByUser && Input.location.status == LocationServiceStatus.Running)
        {
            updatePosTime += Time.deltaTime;
            if (updatePosTime > 1f)
            {
                updatePosTime = 0f;
                LocationInfo lastData = Input.location.lastData;
                Vector2 currentPos = new Vector2(lastData.longitude, lastData.latitude);
                selfMarker.position = currentPos;
                if (currentPos != lastPos)
                {
                    pathPointsList.Add(currentPos);
                    onlineMaps.Redraw();
                }
                if (lastPos == Vector2.zero)
                {
                    onlineMaps.position = currentPos;
                }
                else if (isRunning)
                {
                    //Cumulative distance
                    distance += DistanceBetweenPoints(lastPos, currentPos);
                }
                lastPos = currentPos;
            }
        }

        if (isRunning)
        {
            //Update time and info data
            time = DateTime.Now.Subtract(startTime).TotalSeconds;
            infoControl.UpdateInfo(distance, time);
        }
    }
    public void Show(byte[] iconImageData)
    {
        base.Show();
        if (iconImageData != null)
        {
            //Initialize the icon for the marker
            Texture2D iconTexture2D = new Texture2D(0, 0);
            iconTexture2D.LoadImage(iconImageData);
            selfMarker.texture = iconTexture2D;
            selfMarker.Init();
        }
        pathPointsList.Clear();
        onlineMaps.Redraw();
        infoControl.Show();
        readyControl.Show();
    }
    public override void Hide()
    {
        base.Hide();
        Input.location.Stop();
    }
    //Calculate the distance between two geographic coordinates
    private double DistanceBetweenPoints(Vector2 point1, Vector2 point2)
    {
        double Deg2Rad = Math.PI / 180d;
        double scfY = Math.Sin(point1.y * Deg2Rad);
        double sctY = Math.Sin(point2.y * Deg2Rad);
        double ccfY = Math.Cos(point1.y * Deg2Rad);
        double cctY = Math.Cos(point2.y * Deg2Rad);
        double cX = Math.Cos((point1.x - point2.x) * Deg2Rad);
        double R = 6371d;
        double sizeX1 = Math.Abs(R * Math.Acos(scfY * scfY + ccfY * ccfY * cX));
        double sizeX2 = Math.Abs(R * Math.Acos(sctY * sctY + cctY * cctY * cX));
        double sizeX = (sizeX1 + sizeX2) / 2.0;
        double sizeY = R * Math.Acos(scfY * sctY + ccfY * cctY);
        if (double.IsNaN(sizeX)) sizeX = 0;
        if (double.IsNaN(sizeY)) sizeY = 0;
        return Math.Sqrt(sizeX * sizeX + sizeY * sizeY);
    }
    public void StartSport()
    {
        isRunning = true;
        lastPos = Vector2.zero;
        startTime = DateTime.Now.AddSeconds(-time);
    }
    public void PauseSport()
    {
        isRunning = false;
    }
}

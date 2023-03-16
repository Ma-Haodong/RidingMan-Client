using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;

public class MileageCounterControl : MonoBehaviour
{
    [SerializeField]
    private Text debugText;
    [SerializeField]
    private Text distanceText;
    [SerializeField]
    private Button startButton;
    [SerializeField]
    private Button stopButton;
    private float updateTime;
    private Vector2 lastPos;
    private double distance;
    // Start is called before the first frame update
    void Start()
    {
        startButton.onClick.AddListener(() =>
        {
            startButton.gameObject.SetActive(false);
            stopButton.gameObject.SetActive(true);
            Permission.RequestUserPermissions(new[] { Permission.CoarseLocation, Permission.FineLocation });
            Input.location.Start(1f, 1f);
        });

        stopButton.onClick.AddListener(() =>
        {
            stopButton.gameObject.SetActive(false);
            startButton.gameObject.SetActive(true);
            distance = 0d;
            Permission.RequestUserPermissions(new[] { Permission.CoarseLocation, Permission.FineLocation });
            Input.location.Stop();
        });
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.location.isEnabledByUser && Input.location.status == LocationServiceStatus.Running)
        {
            updateTime += Time.deltaTime;
            if (updateTime > 1f)
            {
                updateTime = 0f;
                LocationInfo lastData = Input.location.lastData;
                if (lastPos != Vector2.zero)
                {
                    distance += DistanceBetweenPoints(lastPos, new Vector2(lastData.longitude, lastData.latitude));
                }
                lastPos = new Vector2(lastData.longitude, lastData.latitude);
                debugText.text = $"Longitude: {lastData.longitude.ToString("f6")} Latitude: {lastData.latitude.ToString("f6")}";
            }
        }
        distanceText.text = $"{distance.ToString("f0")}m";
    }
    private double DistanceBetweenPoints(Vector2 point1, Vector2 point2)
    {
        double Deg2Rad = Math.PI / 180d;
        double scfY = Math.Sin(point1.y * Deg2Rad);
        double sctY = Math.Sin(point2.y * Deg2Rad);
        double ccfY = Math.Cos(point1.y * Deg2Rad);
        double cctY = Math.Cos(point2.y * Deg2Rad);
        double cX = Math.Cos((point1.x - point2.x) * Deg2Rad);
        double R = 6371000d;
        double sizeX1 = Math.Abs(R * Math.Acos(scfY * scfY + ccfY * ccfY * cX));
        double sizeX2 = Math.Abs(R * Math.Acos(sctY * sctY + cctY * cctY * cX));
        double sizeX = (sizeX1 + sizeX2) / 2.0;
        double sizeY = R * Math.Acos(scfY * sctY + ccfY * cctY);
        if (double.IsNaN(sizeX)) sizeX = 0;
        if (double.IsNaN(sizeY)) sizeY = 0;
        return Math.Sqrt(sizeX * sizeX + sizeY * sizeY);
    }
}

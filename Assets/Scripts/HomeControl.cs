using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using BestHTTP;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
//Main interface control class
public class HomeControl : Singleton<HomeControl>
{
    [SerializeField]
    private Transform logoT;
    [SerializeField]
    private Text nameText;
    [SerializeField]
    private Button iconButton;
    private byte[] iconImageData;
    [SerializeField]
    private Button startButton;
    // Start is called before the first frame update
    void Start()
    {
        logoT.DOLocalRotate(new Vector3(0f, 0f, 20f), 5f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);

        iconButton.onClick.AddListener(() =>
        {
            LoginControl.Instance.Show();
        });

        startButton.onClick.AddListener(() =>
        {
            SportControl.Instance.Show(iconImageData);
        });
    }
    public void Show(string name)
    {
        nameText.text = name;
        //Http request to get icon
        HTTPRequest request = new HTTPRequest(new Uri($"{Global.HttpHost}/getIcon"), HTTPMethods.Get);
        request.AddField("id", Global.UserId);
        request.Callback += (request, response) =>
        {
            if (response.Data.LongLength > 0)
            {
                //Here, the original data of the picture needs to be stored to prevent later permission problems. The plug-in cannot read Texture2D
                iconImageData = response.Data;
                Texture2D iconTexture2D = response.DataAsTexture2D;
                iconButton.image.sprite = Sprite.Create(iconTexture2D, new Rect(0f, 0f, iconTexture2D.width, iconTexture2D.height), Vector2.one * 0.5f);
            }
        };
        request.Send();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BestHTTP;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        HTTPRequest request = new HTTPRequest(new Uri($"{Global.HttpHost}/Account/Login"), HTTPMethods.Post);
        request.AddField("LoginId", "123");
        request.AddField("Password", "123");
        request.Callback += (request, response) =>
        {
            print(response.DataAsText);
            JToken result = JsonConvert.DeserializeObject<JToken>(response.DataAsText);
        };
        request.Send();
    }

    // Update is called once per frame
    void Update()
    {

    }
}

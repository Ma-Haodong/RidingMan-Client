using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BestHTTP;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class LoginControl : PageControl<LoginControl>
{
    [SerializeField]
    private Button returnButton;
    [SerializeField]
    private InputField nameInputField;
    [SerializeField]
    private InputField passwordInputField;
    [SerializeField]
    private Button loginButton;
    [SerializeField]
    private Button registerButton;
    // Start is called before the first frame update
    void Start()
    {
        returnButton.onClick.AddListener(() =>
        {
            this.Hide();
        });

        loginButton.onClick.AddListener(() =>
        {
            if (string.IsNullOrEmpty(nameInputField.text))
            {
                HintControl.Instance.Show("The name cannot be empty!");
                return;
            }
            else if (string.IsNullOrEmpty(passwordInputField.text))
            {
                HintControl.Instance.Show("The password cannot be empty!");
                return;
            }
            //Log in with the user name and password
            HTTPRequest request = new HTTPRequest(new Uri($"{Global.HttpHost}/login"), HTTPMethods.Post);
            request.AddField("name", nameInputField.text);
            request.AddField("password", passwordInputField.text);
            request.Callback += (request, response) =>
            {
                print(response.DataAsText);
                JToken result = JsonConvert.DeserializeObject<JToken>(response.DataAsText);
                if (result.Value<bool>("success"))
                {
                    Global.UserId = result.Value<string>("id");
                    this.Hide();
                    HomeControl.Instance.Show(result.Value<string>("name"));
                }
                else
                {
                    HintControl.Instance.Show(result.Value<string>("message"));
                }
            };
            request.Send();
        });

        registerButton.onClick.AddListener(() =>
        {
            RegisterControl.Instance.Show();
        });
    }
    public override void Show()
    {
        base.Show();
        nameInputField.text = string.Empty;
        passwordInputField.text = string.Empty;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BestHTTP;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
//Registration control class
public class RegisterControl : PageControl<RegisterControl>
{
    [SerializeField]
    private Button returnButton;
    [SerializeField]
    private Button selectIconButton;
    private Sprite defaultIconSprite;
    [SerializeField]
    private InputField nameInputField;
    [SerializeField]
    private InputField passwordInputField;
    [SerializeField]
    private InputField confirmPasswordInputField;
    [SerializeField]
    private Button registerButton;
    // Start is called before the first frame update
    void Start()
    {
        returnButton.onClick.AddListener(() =>
        {
            this.Hide();
        });

        selectIconButton.onClick.AddListener(() =>
        {
            NativeGallery.GetImageFromGallery(imagePath =>
            {
                if (!string.IsNullOrEmpty(imagePath))
                {
                    SelectIconControl.Instance.Show(NativeGallery.LoadImageAtPath(imagePath));
                }
            });
        });
        defaultIconSprite = selectIconButton.image.sprite;

        registerButton.onClick.AddListener(() =>
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
            else if (string.IsNullOrEmpty(confirmPasswordInputField.text))
            {
                HintControl.Instance.Show("The confirm password cannot be empty!");
                return;
            }
            else if (passwordInputField.text != confirmPasswordInputField.text)
            {
                HintControl.Instance.Show("The two passwords are different!");
                return;
            }
            //The user submits an icon, username and password to register an account
            HTTPRequest request = new HTTPRequest(new Uri($"{Global.HttpHost}/register"), HTTPMethods.Post);
            request.AddBinaryData("icon", selectIconButton.image.sprite.texture.EncodeToPNG());
            request.AddField("name", nameInputField.text);
            request.AddField("password", passwordInputField.text);
            request.Callback += (request, response) =>
            {
                print(response.DataAsText);
                JToken result = JsonConvert.DeserializeObject<JToken>(response.DataAsText);
                if (result.Value<bool>("success"))
                {
                    this.Hide();
                }
                else
                {
                    HintControl.Instance.Show(result.Value<string>("message"));
                }
            };
            request.Send();
        });
    }
    public override void Show()
    {
        base.Show();
        selectIconButton.image.sprite = defaultIconSprite;
        nameInputField.text = string.Empty;
        passwordInputField.text = string.Empty;
        confirmPasswordInputField.text = string.Empty;
    }
    //After selecting the icon, you can send back the icon Texture2D data
    public void Show(Texture2D iconTexture2D)
    {
        selectIconButton.image.sprite = Sprite.Create(iconTexture2D, new Rect(0f, 0f, iconTexture2D.width, iconTexture2D.height), Vector2.one * 0.5f);
    }
}

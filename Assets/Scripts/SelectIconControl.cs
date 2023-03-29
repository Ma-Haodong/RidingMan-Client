using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//Select icon control class
public class SelectIconControl : PageControl<SelectIconControl>
{
    private RectTransform thisRectT;
    [SerializeField]
    private RawImage iconRawImage;
    private RectTransform iconRectT;
    private float aspectRatio;
    private Vector2 touch0LastPos;
    private Vector2 touch1LastPos;
    [SerializeField]
    private Button returnButton;
    [SerializeField]
    private Button confirmButton;
    [SerializeField]
    private Texture2D circleTexture2D;
    protected override void Awake()
    {
        base.Awake();
        thisRectT = this.GetComponent<RectTransform>();
        iconRectT = iconRawImage.GetComponent<RectTransform>();

        returnButton.onClick.AddListener(() =>
        {
            this.Hide();
        });

        confirmButton.onClick.AddListener(() =>
        {
            StartCoroutine(CreateIcon());
        });
    }
    // Update is called once per frame
    void Update()
    {
        if (!IsActive)
        {
            return;
        }

        if (Input.touchCount == 1)//Single finger control picture movement
        {
            Touch touch0 = Input.GetTouch(0);
            if (touch0.phase == TouchPhase.Moved)
            {
                iconRectT.localPosition += new Vector3(touch0.deltaPosition.x, touch0.deltaPosition.y);
            }
            touch0LastPos = touch0.position;
        }
        else if (Input.touchCount == 2)//Double finger control picture zoom
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            if (touch0.phase == TouchPhase.Began)
            {
                touch0LastPos = touch0.position;
            }
            if (touch1.phase == TouchPhase.Began)
            {
                touch1LastPos = touch1.position;
            }
            if (touch0.phase == TouchPhase.Moved)
            {
                float lastDis = Vector2.Distance(touch0LastPos, touch1LastPos);
                float currentDis = Vector2.Distance(touch0.position, touch1.position);
                float heightDelta = currentDis - lastDis;
                iconRectT.sizeDelta += new Vector2(heightDelta * aspectRatio, heightDelta);
                touch0LastPos = touch0.position;
                touch1LastPos = touch1.position;
            }
            else if (touch1.phase == TouchPhase.Moved)
            {
                float lastDis = Vector2.Distance(touch0LastPos, touch1LastPos);
                float currentDis = Vector2.Distance(touch0.position, touch1.position);
                float heightDelta = currentDis - lastDis;
                iconRectT.sizeDelta += new Vector2(heightDelta * aspectRatio, heightDelta);
                touch0LastPos = touch0.position;
                touch1LastPos = touch1.position;
            }
        }
        //Limit picture size
        float minSize = 800f * iconRawImage.canvas.scaleFactor;
        if (iconRectT.rect.width >= iconRectT.rect.height)
        {
            if (iconRectT.rect.height < minSize)
            {
                iconRectT.sizeDelta = new Vector2(minSize * aspectRatio, minSize);
            }
        }
        else
        {
            if (iconRectT.rect.width < minSize)
            {
                iconRectT.sizeDelta = new Vector2(minSize, minSize / aspectRatio);
            }
        }
        //Limit picture position
        float halfMinSize = minSize / 2;
        if (iconRectT.localPosition.x - iconRectT.rect.width / 2 > -halfMinSize)
        {
            iconRectT.localPosition = new Vector3(iconRectT.rect.width / 2 - halfMinSize, iconRectT.localPosition.y);
        }
        else if (iconRectT.localPosition.x + iconRectT.rect.width / 2 < halfMinSize)
        {
            iconRectT.localPosition = new Vector3(halfMinSize - iconRectT.rect.width / 2, iconRectT.localPosition.y);
        }
        if (iconRectT.localPosition.y - iconRectT.rect.height / 2 > -halfMinSize)
        {
            iconRectT.localPosition = new Vector3(iconRectT.localPosition.x, iconRectT.rect.height / 2 - halfMinSize);
        }
        else if (iconRectT.localPosition.y + iconRectT.rect.height / 2 < halfMinSize)
        {
            iconRectT.localPosition = new Vector3(iconRectT.localPosition.x, halfMinSize - iconRectT.rect.height / 2);
        }
    }
    public void Show(Texture2D imageTexture2D)
    {
        base.Show();
        if (iconRawImage.texture != null)
        {
            DestroyImmediate(iconRawImage.texture);
        }
        iconRawImage.gameObject.SetActive(true);
        iconRawImage.texture = imageTexture2D;
        iconRectT.localPosition = Vector3.zero;
        iconRectT.sizeDelta = new Vector2(imageTexture2D.width, imageTexture2D.height);
        aspectRatio = (float)imageTexture2D.width / imageTexture2D.height;
    }
    //Create icon by taking screenshots and sampling masks
    private IEnumerator CreateIcon()
    {
        yield return new WaitForEndOfFrame();
        int rectSize = (int)(800f * iconRawImage.canvas.scaleFactor * iconRawImage.canvas.scaleFactor);
        int x = (int)((thisRectT.rect.width - 800f * iconRawImage.canvas.scaleFactor) / 2 * iconRawImage.canvas.scaleFactor);
        int y = (int)((thisRectT.rect.height - 800f * iconRawImage.canvas.scaleFactor) / 2 * iconRawImage.canvas.scaleFactor);
        Texture2D texture2D = new Texture2D(rectSize, rectSize, TextureFormat.RGBA32, false);
        texture2D.ReadPixels(new Rect(x, y, rectSize, rectSize), 0, 0);
        texture2D.Apply();
        List<Color> colors = new List<Color>();
        for (int i = 0; i < circleTexture2D.height; i++)
        {
            float v = (float)i / circleTexture2D.height;
            for (int n = 0; n < circleTexture2D.width; n++)
            {
                float u = (float)n / circleTexture2D.width;
                Color color = texture2D.GetPixelBilinear(u, v);
                Color circleColor = circleTexture2D.GetPixelBilinear(u, v);
                color.a = circleColor.a;
                colors.Add(color);
            }
        }
        Texture2D iconTexture2D = new Texture2D(circleTexture2D.width, circleTexture2D.height, TextureFormat.RGBA32, false);
        iconTexture2D.SetPixels(colors.ToArray());
        iconTexture2D.Apply();
        iconRawImage.gameObject.SetActive(false);
        this.Hide();
        RegisterControl.Instance.Show(iconTexture2D);
    }
}

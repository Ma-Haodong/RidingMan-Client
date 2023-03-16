using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class HintControl : PageControl<HintControl>
{
    [SerializeField]
    private Text messageText;
    [SerializeField]
    private Button confirmButton;
    [SerializeField]
    private Button cancelButton;
    private UnityAction onConfirmed;
    // Start is called before the first frame update
    void Start()
    {
        confirmButton.onClick.AddListener(() =>
        {
            Hide();
            onConfirmed?.Invoke();
        });

        cancelButton.onClick.AddListener(() =>
        {
            Hide();
        });
    }
    public void Show(string message, UnityAction onConfirmed = null)
    {
        base.Show();
        messageText.text = message;
        this.onConfirmed = onConfirmed;
        cancelButton.gameObject.SetActive(onConfirmed != null);
    }
}

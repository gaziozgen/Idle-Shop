using FateGames.Core;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PersonalBubble : FateMonoBehaviour
{
    [SerializeField] private Canvas canvas = null;
    [SerializeField] private GameObject textBubble = null;
    [SerializeField] private TextMeshProUGUI tmproText = null;
    [SerializeField] private GameObject loadingBubble = null;
    [SerializeField] private Image loadingBar = null;

    private bool loading = false;
    private float loadingDuration = 1;
    private bool textShowing = false;

    public void UpdateByManager(Vector3 rotation)
    {
        canvas.transform.eulerAngles = rotation;

        if (loading)
        {
            float nextAmount = loadingBar.fillAmount + Time.deltaTime / loadingDuration;
            if (nextAmount > 1) nextAmount = nextAmount % 1;
            loadingBar.fillAmount = nextAmount;
        }
    }

    public void StartLoading(float duration)
    {
        if (!canvas.enabled) canvas.enabled = true;

        if (textShowing)
        {
            textShowing = false;
            textBubble.SetActive(false);
        }

        loading = true;
        loadingBubble.gameObject.SetActive(true);
        loadingBar.fillAmount = 0;
        loadingDuration = duration;
    }

    public void SetText(string text)
    {
        if (!canvas.enabled) canvas.enabled = true;

        if (loading)
        {
            loadingBubble.gameObject.SetActive(false);
            loading = false;
        }

        textShowing = true;
        textBubble.SetActive(true);
        tmproText.text = text;
    }

    public void Close()
    {
        loading = false;
        textShowing = false;
        loadingBubble.gameObject.SetActive(false);
        textBubble.SetActive(false);
        canvas.enabled = false;
    }

    private void Awake()
    {
        PersonalBubbleManager.Instance.AddBubble(this);
    }
}

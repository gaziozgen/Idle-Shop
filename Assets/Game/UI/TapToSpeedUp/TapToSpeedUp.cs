using FateGames.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using DG.Tweening;
using TMPro;
using System;

public class TapToSpeedUp : FateMonoBehaviour
{
    public static TapToSpeedUp Instance;

    [SerializeField] private Canvas canvas;
    [SerializeField] private TextMeshProUGUI multiplierText;
    [SerializeField] private GameObject multiplierArea;
    [SerializeField] private GameObject InfoText;
    [SerializeField] private Animator animator;
    [SerializeField] private float impactDuration = 2f;
    [SerializeField] private float changeSpeed = 2f;
    [SerializeField] private float waitingDuration = 4;
    [SerializeField] private float[] multipliers = new float[3];
    [SerializeField] private GameStateVariable gameState = null;


    private float[] tapExpiryTimes = { -1, -1, -1 };
    private float infoTextShowTime = -1;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) { TapCheck(); }

        if (gameState.Value != GameState.PAUSED)
        {
            int tapCount = TapCount();
            Time.timeScale = Mathf.MoveTowards(Time.timeScale, multipliers[tapCount], Time.deltaTime * changeSpeed);
            if (Time.timeScale <= 1 && tapCount == 0)
            {
                if (multiplierArea.activeSelf) multiplierArea.SetActive(false);
            }
            else
            {
                if (!multiplierArea.activeSelf) multiplierArea.SetActive(true);
                multiplierText.text = "x" + Time.timeScale.ToString("F1");
            }



            if (Time.timeScale == 1 && Time.time > infoTextShowTime && !InfoText.activeSelf)
            {
                InfoText.SetActive(true);
                Animate();
            }
        }
    }

    public void Hide()
    {
        canvas.enabled = false;
    }

    public void Show()
    {
        canvas.enabled = true;
    }

    private void TapCheck() { if (gameState.Value != GameState.PAUSED && (!EventSystem.current.IsPointerOverGameObject() || EventSystem.current.currentSelectedGameObject == null)) Tap(); }


    public void Tap()
    {
        GameManager.Instance.PlayHaptic();

        float oldestExpiryTime = float.MaxValue;
        int oldestExpiryTimeIndex = -1;

        for (int i = 0; i < tapExpiryTimes.Length; i++)
        {
            if (tapExpiryTimes[i] < oldestExpiryTime)
            {
                oldestExpiryTime = tapExpiryTimes[i];
                oldestExpiryTimeIndex = i;
            }
        }
        tapExpiryTimes[oldestExpiryTimeIndex] = Time.time + impactDuration;

        if (InfoText.activeSelf) InfoText.SetActive(false);
        infoTextShowTime = Time.time + waitingDuration;
    }

    private int TapCount()
    {
        int total = 0;
        for (int i = 0; i < tapExpiryTimes.Length; i++)
        {
            if (tapExpiryTimes[i] > Time.time) total++;
        }
        return total;
    }

    private void Animate()
    {
        animator.SetTrigger("Bounce");
    }

}

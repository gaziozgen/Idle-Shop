using FateGames.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using DG.Tweening;

public class TapToSpeedUp : FateMonoBehaviour
{
    public static TapToSpeedUp Instance;

    [SerializeField] private Canvas canvas;
    [SerializeField] private Animator animator;
    //[SerializeField] private FloatReference targetVariable;
    //[SerializeField] GameEvent onTappedToSpeedUp;
    [SerializeField] private float impactDuration = 1.5f;
    [SerializeField] private float waitingDuration = 4;
    [SerializeField] private float multiplier = 2;
    [SerializeField] private GameStateVariable gameState = null;

    private bool isGameStarted = false;

    private Tween speedUpTween = null, countdownTween = null;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) { TapCheck(); }
    }

    private void TapCheck() { if (isGameStarted && gameState.Value != GameState.PAUSED && (!EventSystem.current.IsPointerOverGameObject() || EventSystem.current.currentSelectedGameObject == null)) Tap(); }

    public void OnGameStarted()
    {
        Debug.Log("OnGameStarted", this);
        isGameStarted = true;
        StartCountdown();
    }
    public void OnGameEnded()
    {
        isGameStarted = false;
        CancelCountdown();
        canvas.enabled = false;
    }

    public void Tap()
    {
        GameManager.Instance.PlayHaptic();
        //onTappedToSpeedUp.Raise();
        canvas.enabled = false;
        SpeedUp();
        StartCountdown();
    }
    public void SpeedUp()
    {
        CancelSpeedUp();
        Time.timeScale = multiplier;
        speedUpTween = DOTween.To(() => Time.timeScale, (float x) => { if (gameState.Value != GameState.PAUSED) Time.timeScale = x; }, 1, impactDuration).OnComplete(() => speedUpTween = null);
    }
    public void CancelSpeedUp()
    {
        if (speedUpTween == null) return;
        speedUpTween.Kill(true);
        speedUpTween = null;
    }
    public void StartCountdown()
    {
        CancelCountdown();
        countdownTween = DOVirtual.DelayedCall(waitingDuration, Show).OnComplete(() => countdownTween = null);
    }
    public void CancelCountdown()
    {
        if (countdownTween == null) return;
        countdownTween.Kill(true);
        countdownTween = null;
    }

    public void Hide()
    {
        canvas.enabled = false;
    }

    public void Show()
    {
        canvas.enabled = true;
        Animate();
    }
    public void Animate()
    {
        animator.SetTrigger("Bounce");
    }

}

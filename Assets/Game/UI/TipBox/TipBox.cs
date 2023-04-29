using DG.Tweening;
using FateGames.Core;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TipBox : MonoBehaviour
{
    public static TipBox Instance;

    [SerializeField] private Canvas buttonCanvas = null;
    [SerializeField] private Canvas panelCanvas = null;
    [SerializeField] private Transform tipBoxParent = null;
    [SerializeField] private Transform tipBox = null;
    [SerializeField] private Transform panel = null;
    [SerializeField] protected TextMeshProUGUI tipAmountText = null;

    private int currentAmount = 0;
    private bool boxAnimationPlaying = false;


    private void Awake()
    {
        Instance = this;
        tipBoxParent.localScale = Vector3.zero;
    }

    public void AddTip(int amount)
    {
        if (currentAmount == 0) tipBoxParent.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        currentAmount += amount;

        if (!boxAnimationPlaying)
        {
            boxAnimationPlaying = true;
            tipBox.DORotate(Vector3.forward * 30, 0.2f).SetLoops(2, LoopType.Yoyo).OnComplete(() =>
            {
                boxAnimationPlaying = false;
            });
        }
    }


    public void OpenPanel()
    {
        GameManager.Instance.PauseGame();
        panelCanvas.enabled = true;
        buttonCanvas.enabled = false;
        tipAmountText.text = currentAmount.ToString();

        tipBoxParent.localScale = Vector3.zero;

        /*panel.localScale = Vector3.zero;
        panel.DOScale(1, 0.5f).SetEase(Ease.OutBack);*/
    }

    public void Get()
    {
        UIMoney.Instance.AddFromTip(currentAmount);
        ClosePanel();
    }

    public void Double()
    {
        UIMoney.Instance.AddFromTip(currentAmount * 2);
        ClosePanel();
    }

    private void ClosePanel()
    {
        panelCanvas.enabled = false;
        buttonCanvas.enabled = true;
        GameManager.Instance.ResumeGame();
        currentAmount = 0;
    }
}

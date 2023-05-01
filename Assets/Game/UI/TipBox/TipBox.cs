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
    [SerializeField] protected TextMeshProUGUI tipAmountText = null;

    private int currentAmount = 0;


    private void Awake()
    {
        Instance = this;
        tipBoxParent.localScale = Vector3.zero;
    }

    public void AddTip(int amount)
    {
        if (currentAmount == 0) tipBoxParent.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        currentAmount += amount;
    }

    public void OpenPanel()
    {
        GameManager.Instance.PlayHaptic();
        GameManager.Instance.PauseGame();
        UpgradeButtonsController.Instance.Hide();
        TapToSpeedUp.Instance.Hide();
        panelCanvas.enabled = true;
        buttonCanvas.enabled = false;
        tipAmountText.text = currentAmount.ToString();

        tipBoxParent.localScale = Vector3.zero;
    }

    public void Get()
    {
        UIMoney.Instance.AddFromTip(currentAmount);
        ClosePanel();
    }

    public void Double()
    {
        SDKManager.Instance.ShowRewardedAd(Get, () =>
        {
            UIMoney.Instance.AddFromTip(currentAmount * 2);
            ClosePanel();
        });
        
    }

    private void ClosePanel()
    {
        GameManager.Instance.PlayHaptic();
        panelCanvas.enabled = false;
        buttonCanvas.enabled = true;
        GameManager.Instance.ResumeGame();
        TapToSpeedUp.Instance.Show();
        currentAmount = 0;
    }
}

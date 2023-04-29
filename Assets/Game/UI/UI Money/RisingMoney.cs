using DG.Tweening;
using FateGames.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class RisingMoney : FateMonoBehaviour, IPooledObject
{
    [SerializeField] private TextMeshProUGUI textMeshProUGUI;

    public void SetMoney(int money)
    {
        textMeshProUGUI.text = "$ " + money.ToString();
    }

    public Action Release { get; set; }

    public void OnObjectSpawn()
    {
        transform.DOMoveY(6, 1f).SetEase(Ease.OutSine).OnComplete(() =>
        {
            Release();
        });
    }

    public void OnRelease()
    {
        transform.DOKill();
    }
}

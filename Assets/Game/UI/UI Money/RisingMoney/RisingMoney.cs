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
    [SerializeField] private SoundEntity moneySound = null;

    public void SetMoney(int money)
    {
        textMeshProUGUI.text = "$ " + money.ToString();
    }

    public Action Release { get; set; }

    public void OnObjectSpawn()
    {
        GameManager.Instance.PlaySound(moneySound, transform.position);
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

using DG.Tweening;
using FateGames.Core;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DataEditController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyText = null;
    [SerializeField] private UIMoney UIMoney = null;

    public void SetMoney()
    {
        UIMoney.SetMoneyFromDevMode(moneyText.text);
    }

    public void RestartGame()
    {
        SaveManager.DeletePlayerData();
        DOTween.KillAll();
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

}

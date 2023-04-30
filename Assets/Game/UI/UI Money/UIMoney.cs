using DG.Tweening;
using FateGames.Core;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class UIMoney : FateMonoBehaviour
{
    static public UIMoney Instance;

    [SerializeField] private TextMeshProUGUI tmpro = null;
    [SerializeField] private Transform background = null;
    [SerializeField] private float effectDuration = 0.2f;
    [SerializeField] private int coffeeMoney = 10;
    [SerializeField] private GameObject risingMoneyPrefab = null;
    [SerializeField] private SaveDataVariable saveData = null;
    [SerializeField] private UnityEvent onMoneyChanged = null;

    private FateObjectPool<RisingMoney> pool;

    private void Awake()
    {
        pool = new FateObjectPool<RisingMoney>(risingMoneyPrefab, true, 50, 100);
        Instance = this;
    }

    private void Start()
    {
        UpdateText();
    }

    public void Add(int coffeeCount, Vector3 wordPosition, bool tip)
    {
        if (tip) TipBox.Instance.AddTip((int)(coffeeCount * coffeeMoney * Random.Range(0.2f, 0.6f)));
        saveData.Value.Money += coffeeMoney * coffeeCount;
        UpdateText();
        RisingMoneyEffect(wordPosition, coffeeMoney * coffeeCount);
    }

    public void AddFromTip(int amount)
    {
        saveData.Value.Money += amount;
        UpdateText();
    }

    public void Spend(int amount)
    {
        saveData.Value.Money -= amount;
        UpdateText();
    }

    private void RisingMoneyEffect(Vector3 wordPosition, int value)
    {
        pool.Get(wordPosition).SetMoney(value);
    }

    private void UpdateText()
    {
        tmpro.text = saveData.Value.Money.ToString();
        background.DOKill();
        background.DOScale(1.2f, effectDuration / 2).SetLoops(2, LoopType.Yoyo);
        onMoneyChanged.Invoke();
    }

}

public partial class SaveData
{
    public int Money = 0;
}

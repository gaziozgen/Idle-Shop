using DG.Tweening;
using FateGames.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ShopManager : FateMonoBehaviour
{
    public TableManager TableManager = null;
    public CoffeeMachineManager CoffeeMachineManager = null;
    public Reception Reception = null;
    public Trash Trash = null;
    public DriveThru DriveThru = null;
    public CustomerManager CustomerManager = null;
    public WaiterManager WaiterManager = null;
    public WaiterSpeedUpgradeController WaiterSpeedUpgradeController = null;
    [SerializeField] private SoundEntity ambientSound = null;

    static public ShopManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (ambientSound != null) GameManager.Instance.PlaySound(ambientSound);
    }

    public void RequestWaiterToServe(Table table)
    {
        WaiterManager.AddMission((Waiter waiter) =>
        {
            GoToTable mission = new GoToTable();
            waiter.SetMissionAndCoroutine(StartCoroutine(mission.SetMission(waiter, table)), mission, "GoToTable");
        });
    }

    public void RequestWaiterToClean(Table table)
    {
        WaiterManager.AddMission((Waiter waiter) =>
        {
            CleanTable mission = new CleanTable();
            waiter.SetMissionAndCoroutine(StartCoroutine(mission.SetMission(waiter, table)), mission, "CleanTable");
        });
    }
}

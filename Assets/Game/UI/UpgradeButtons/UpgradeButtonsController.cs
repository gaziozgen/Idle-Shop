using FateGames.Core;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UpgradeButtonsController : FateMonoBehaviour
{
    public static UpgradeButtonsController Instance;

    [SerializeField] private SaveDataVariable saveData;

    [SerializeField] private int buyWaiterBaseCost = 0;
    [SerializeField] private int buyWaiterCostIncreaseAmount = 0;

    [SerializeField] private int buyTableBaseCost = 0;
    [SerializeField] private int buyTableCostIncreaseAmount = 0;

    [SerializeField] private int buyMachineBaseCost = 0;
    [SerializeField] private int buyMachineCostIncreaseAmount = 0;

    [SerializeField] private int mergeBaseCost = 0;
    [SerializeField] private int mergeCostIncreaseAmount = 0;

    [SerializeField] private int speedUpBaseCost = 0;
    [SerializeField] private int speedUpCostIncreaseAmount = 0;

    [SerializeField] private TextMeshProUGUI mergeCost = null;
    [SerializeField] private TextMeshProUGUI waiterCost = null;
    [SerializeField] private TextMeshProUGUI speedUpCost = null;
    [SerializeField] private TextMeshProUGUI seatCost = null;
    [SerializeField] private TextMeshProUGUI coffeeMachineCost = null;

    [SerializeField] private Button mergeButton = null;
    [SerializeField] private Button waiterButton = null;
    [SerializeField] private Button speedUpButton = null;
    [SerializeField] private Button seatButton = null;
    [SerializeField] private Button coffeeMachineButton = null;

    [SerializeField] private GameObject nextMapButton = null;
    [SerializeField] private GameObject watchAddForWaiterButton = null;


    [SerializeField] private SoundEntity clickSound = null;
    [SerializeField] private UnityEvent onButtonClicked;


    private WaiterManager waiterManager = null;
    private TableManager tableManager = null;
    private CoffeeMachineManager coffeeMachineManager = null;
    private WaiterSpeedUpgradeController waiterSpeedController = null;

    private void Awake()
    {
        Instance = this;
        waiterManager = ShopManager.Instance.WaiterManager;
        tableManager = ShopManager.Instance.TableManager;
        coffeeMachineManager = ShopManager.Instance.CoffeeMachineManager;
        waiterSpeedController = ShopManager.Instance.WaiterSpeedUpgradeController;
        UpdateAllButtons();
    }

    public void Hide()
    {
        mergeButton.gameObject.SetActive(false);
        waiterButton.gameObject.SetActive(false);
        speedUpButton.gameObject.SetActive(false);
        seatButton.gameObject.SetActive(false);
        coffeeMachineButton.gameObject.SetActive(false);
    }

    public void OpenNextMapButton()
    {
        nextMapButton.SetActive(true);
    }

    private int watchedAddCount = 0;
    public void WatchAddForWaiter()
    {
        SDKManager.Instance.ShowRewardedAd(() => { }, () =>
        {
            watchedAddCount++;
            BuyWaiter(false);

            /*if (watchedAddCount % 3 == 0)
            {
                BuyWaiter();
                BuyWaiter();
            }*/
        });
    }

    public void UpdateAllButtons()
    {
        UpdateMergeButton();
        UpdateBuyWaiterButton();
        UpdateSpeedUpButton();
        UpdateBuySeatButton();
        UpdateBuyCoffeeMachineButton();
    }

    public void BuyWaiter(bool spendMoney)
    {
        OnButtonClicked();
        if (spendMoney) UIMoney.Instance.Spend(buyWaiterBaseCost + saveData.Value.soldierBuyLevel * buyWaiterCostIncreaseAmount);
        waiterManager.AddWaiter(1, Vector3.zero);
        UpdateAllButtons();
    }

    public void BuySeat()
    {
        OnButtonClicked();
        UIMoney.Instance.Spend(buyTableBaseCost + saveData.Value.SeatCount * buyTableCostIncreaseAmount);
        tableManager.UnlockNextTable();
        UpdateAllButtons();
    }

    public void BuyCoffeeMachine()
    {
        OnButtonClicked();
        UIMoney.Instance.Spend(buyMachineBaseCost + saveData.Value.CoffeeMachineCount * buyMachineCostIncreaseAmount);
        coffeeMachineManager.UnlockNextMachine();
        UpdateAllButtons();
    }

    public void SpeedUp()
    {
        OnButtonClicked();
        UIMoney.Instance.Spend(speedUpBaseCost + saveData.Value.SpeedLevel * speedUpCostIncreaseAmount);
        waiterSpeedController.LevelUp();
        UpdateAllButtons();
    }

    public void Merge()
    {
        OnButtonClicked();
        UIMoney.Instance.Spend(mergeBaseCost + saveData.Value.soldierMergeLevel * mergeCostIncreaseAmount);
        StartCoroutine(waiterManager.Merge());
        UpdateAllButtons();
    }

    private void OnButtonClicked()
    {
        GameManager.Instance.PlaySound(clickSound);
        GameManager.Instance.PlayHaptic();
        onButtonClicked.Invoke();
    }

    public void UpdateMergeButton()
    {
        int cost = mergeBaseCost + saveData.Value.soldierMergeLevel * mergeCostIncreaseAmount;
        if (!waiterManager.IsMergeAvaliable())
        {
            mergeButton.gameObject.SetActive(false);
        }
        else if (saveData.Value.Money < cost)
        {
            mergeButton.gameObject.SetActive(true);
            mergeButton.interactable = false;
            mergeCost.text = cost.ToString();
        }
        else
        {
            mergeButton.gameObject.SetActive(true);
            mergeButton.interactable = true;
            mergeCost.text = cost.ToString();
        }
    }

    private void UpdateBuyWaiterButton()
    {
        if (watchAddForWaiterButton.activeSelf) watchAddForWaiterButton.SetActive(false);
        waiterButton.gameObject.SetActive(true);
        int cost = buyWaiterBaseCost + saveData.Value.soldierBuyLevel * buyWaiterCostIncreaseAmount;
        if (!waiterManager.IsBuyAvaliable())
        {
            waiterButton.interactable = false;
            waiterCost.text = "Max";
        }
        else if (saveData.Value.Money < cost)
        {
            waiterButton.interactable = false;
            waiterCost.text = cost.ToString();
            watchAddForWaiterButton.SetActive(true);
        }
        else
        {
            waiterButton.interactable = true;
            waiterCost.text = cost.ToString();
        }
        if (cost == 0) waiterCost.text = "Free";
    }

    private void UpdateSpeedUpButton()
    {
        speedUpButton.gameObject.SetActive(true);
        int cost = speedUpBaseCost + saveData.Value.SpeedLevel * speedUpCostIncreaseAmount;
        if (!waiterSpeedController.IsUpgradeAvaliable())
        {
            speedUpButton.interactable = false;
            speedUpCost.text = "Max";
        }
        else if (saveData.Value.Money < cost)
        {
            speedUpButton.interactable = false;
            speedUpCost.text = cost.ToString();
        }
        else
        {
            speedUpButton.interactable = true;
            speedUpCost.text = cost.ToString();
        }
    }

    private void UpdateBuySeatButton()
    {
        seatButton.gameObject.SetActive(true);
        int cost = buyTableBaseCost + saveData.Value.SeatCount * buyTableCostIncreaseAmount;
        if (!tableManager.IsUnlockAvaliable())
        {
            seatButton.interactable = false;
            seatCost.text = "Max";
        }
        else if (saveData.Value.Money < cost)
        {
            seatButton.interactable = false;
            seatCost.text = cost.ToString();
        }
        else
        {
            seatButton.interactable = true;
            seatCost.text = cost.ToString();
        }
        if (cost == 0) seatCost.text = "Free";
    }

    private void UpdateBuyCoffeeMachineButton()
    {
        if (tableManager.HowMuchMachineCanBeOpenNow() > saveData.Value.CoffeeMachineCount && coffeeMachineManager.IsUnlockAvaliable())
        {
            coffeeMachineButton.gameObject.SetActive(true);
            int cost = buyMachineBaseCost + saveData.Value.CoffeeMachineCount * buyMachineCostIncreaseAmount;
            if (saveData.Value.Money < cost)
            {
                coffeeMachineButton.interactable = false;
                coffeeMachineCost.text = cost.ToString();
            }
            else
            {
                coffeeMachineButton.interactable = true;
                coffeeMachineCost.text = cost.ToString();
            }
            if (cost == 0) coffeeMachineCost.text = "Free";
        }
        else coffeeMachineButton.gameObject.SetActive(false);

    }
}

using FateGames.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TableManager : MonoBehaviour
{
    [SerializeField] private List<Table> tables = new List<Table>();
    [SerializeField] private GameObject secondPlace = null;
    [SerializeField] private GameObject secondPlaceLock = null;
    [SerializeField] private int secondPlaceUnlockLevel = 0;
    [SerializeField] private int driveThruUnlockLevel = 12;
    [SerializeField] private SaveDataVariable saveData;

    private bool secondPlaceUnlocked = false;

    private Table currentTable = null;

    private void Start()
    {
        FastOpen(saveData.Value.SeatCount);
    }

    private void FastOpen(int level)
    {
        for (int i = 0; i < tables.Count; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (level == 0) return;
                tables[i].Unlock(false);
                level--;
            }
            if (saveData.Value.SeatCount >= secondPlaceUnlockLevel && !secondPlaceUnlocked)
            {
                secondPlaceUnlocked = true;
                FreeIdleCameraController.Instance.UpgradeClamp();
                FreeWaiterArea.Instance.UpgradePositions();
                UpgradeButtonsController.Instance.OpenNextMapButton();
                secondPlace.SetActive(true);
                secondPlaceLock.SetActive(false);
            }
        }
    }

    public Seat GetEmptySeatIfAny()
    {
        if (currentTable)
        {
            Seat seat = currentTable.GetSeatToSit();
            if (seat) return seat;
        }

        List<Table> tablesHasAtLeastOneEmptySeat = new List<Table>();
        for (int i = 0; i < tables.Count; i++)
        {
            if (tables[i].UnlockedSeatCount != 0 && tables[i].HasAtLeastOneEmptySeat())
                tablesHasAtLeastOneEmptySeat.Add(tables[i]);
        }

        if (tablesHasAtLeastOneEmptySeat.Count > 0)
        {
            currentTable = tablesHasAtLeastOneEmptySeat[Random.Range(0, tablesHasAtLeastOneEmptySeat.Count)];
            return currentTable.GetSeatToSit();
        }
        else
            return null;
    }

    public bool IsUnlockAvaliable()
    {
        if (tables[tables.Count - 1].UnlockedSeatCount == 4) return false;
        else return true;
    }

    public void UnlockNextTable()
    {
        for (int i = 0; i < tables.Count; i++)
        {
            if (tables[i].UnlockedSeatCount < 4)
            {
                saveData.Value.SeatCount++;

                tables[i].Unlock(true);
                FreeIdleCameraController.Instance.Focus(tables[i].transform.position, 0.5f);

                if (saveData.Value.SeatCount >= secondPlaceUnlockLevel && !secondPlaceUnlocked)
                {
                    secondPlaceUnlocked = true;
                    FreeIdleCameraController.Instance.UpgradeClamp();
                    FreeWaiterArea.Instance.UpgradePositions();
                    UpgradeButtonsController.Instance.OpenNextMapButton();
                    secondPlace.SetActive(true);
                    secondPlaceLock.SetActive(false);
                }
                if (saveData.Value.SeatCount >= driveThruUnlockLevel && !saveData.Value.DriveThruUnlocked)
                {
                    ShopManager.Instance.DriveThru.Unlock();
                    FreeIdleCameraController.Instance.Focus(new Vector3(-10, 0, 10), 0.5f);
                }
                return;
            }
        }
        Debug.LogError("no table left");
    }

    public int HowMuchMachineCanBeOpenNow()
    {
        int ratio = tables.Count * 4 / ShopManager.Instance.CoffeeMachineManager.MaxCapacity();
        return saveData.Value.SeatCount / ratio + 1;
    }
}

public partial class SaveData
{
    public int SeatCount = 0;
}

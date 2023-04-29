using FateGames.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class CoffeeMachineManager : MonoBehaviour
{
    [SerializeField] protected SaveDataVariable saveData;
    [SerializeField] private List<CoffeeMachine> machines = new List<CoffeeMachine>();

    public WaitUntil WaitUntilThereIsACoffeeMachine;

    private void Awake()
    {
        WaitUntilThereIsACoffeeMachine = new WaitUntil(() => saveData.Value.CoffeeMachineCount > 0);
        FastOpen(saveData.Value.CoffeeMachineCount);
    }

    private void FastOpen(int level)
    {
        for (int i = 0; i < machines.Count; i++)
        {
            if (level == 0) return;
            machines[i].Unlock();
            level--;
        }
    }

    public CoffeeMachine GetCoffeeMachineWithShortestQueue()
    {
        int min = int.MaxValue;
        CoffeeMachine machine = null;

        CoffeeMachine currentMachine;
        for (int i = 0; i < machines.Count; i++)
        {
            currentMachine = machines[i];
            if (currentMachine.Unlocked && currentMachine.QueueLength() < min)
            {
                machine = currentMachine;
                min = machine.QueueLength();
            }
        }
        return machine;
    }

    public bool IsUnlockAvaliable()
    {
        if (machines[machines.Count - 1].Unlocked) return false;
        else return true;
    }

    public void UnlockNextMachine()
    {
        for (int i = 0; i < machines.Count; i++)
        {
            if (!machines[i].Unlocked)
            {
                saveData.Value.CoffeeMachineCount++;
                machines[i].Unlock();
                FreeIdleCameraController.Instance.Focus(machines[i].transform.position, 0.5f);
                return;
            }
        }

        Debug.LogError("no coffee machine left");
    }

    public int MaxCapacity()
    {
        return machines.Count;
    }
}

public partial class SaveData
{
    public int CoffeeMachineCount = 0;
}

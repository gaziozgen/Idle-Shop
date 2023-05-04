using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ServeToTakeAway : Mission<Waiter>
{
    private CoffeeMachine machine = null;
    private DriveThru driveThru = null;

    public IEnumerator SetMission(Waiter waiter, DriveThru driveThru)
    {
        person = waiter;
        this.driveThru = driveThru;
        yield return DoMission();
    }

    public override IEnumerator DoMission()
    {
        int order = Mathf.Min(person.Capacity, driveThru.CoffeeShortage);
        driveThru.PromiseForCoffee(order);
        person.SetOrder(order);

        machine = ShopManager.Instance.CoffeeMachineManager.GetCoffeeMachineWithShortestQueue();

        machine.JoinQueue(person);
        yield return person.WaitUntilOrderTaken;
        machine.Dequeue();

        person.SetOrder(0);

        driveThru.JoinQueue(person);
        yield return person.WaitUntilCoffeeStackFinished;
        driveThru.Dequeue();

        person.SetMissionAndCoroutine(null, null, "null");
        ShopManager.Instance.WaiterManager.MissionDone(person);
    }

    public override void HandleStopMission()
    {
        //person.MissionList.Add("HandleStopMission Start");
        if (person.Order > 0) // sipari�i �retmeye gidiyor veya al�yor
        {
            //person.MissionList.Add("sipari�i �retmeye gidiyor veya al�yorken yok oldu");
            machine.RemoveFromQueue(person);
            driveThru.CancelCoffeePromise(person.Order);

        }
        else if (person.Order == 0 && person.CoffeeStackLength > 0) // yolda veya kahve da��t�yor
        {
            //person.MissionList.Add("yolda veya kahve da��t�yorken yok oldu");
            driveThru.RemoveFromQueue(person);
            driveThru.CancelCoffeePromise(person.CoffeeStackLength);
        }

        //table.RemoveWaiterFromServers(person);
        person.CleanCoffeeStack();
        person.SetMissionAndCoroutine(null, null, "null");
        //person.MissionList.Add("HandleStopMission End");
    }
}

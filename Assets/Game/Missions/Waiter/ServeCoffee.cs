using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ServeCoffee : Mission<Waiter>
{
    private Table table = null;
    private CoffeeMachine machine = null;

    public IEnumerator SetMission(Waiter waiter, Table table)
    {
        this.table = table;
        person = waiter;
        yield return DoMission();
    }

    public override IEnumerator DoMission()
    {
        table.RegisterToServingWaiters(person);

        machine = ShopManager.Instance.CoffeeMachineManager.GetCoffeeMachineWithShortestQueue();
        if (machine == null)
        {
            yield return ShopManager.Instance.CoffeeMachineManager.WaitUntilThereIsACoffeeMachine;
            machine = ShopManager.Instance.CoffeeMachineManager.GetCoffeeMachineWithShortestQueue();
        }

        machine.JoinQueue(person);
        yield return person.WaitUntilOrderTaken;
        machine.Dequeue();

        person.SetOrder(0);

        table.JoinQueue(person);
        yield return person.WaitUntilCoffeeStackFinished;
        table.Dequeue();


        person.SetMissionAndCoroutine(null, null, "null");
        table.StartOrderMission(person);
    }

    public override void HandleStopMission()
    {
        //person.MissionList.Add("HandleStopMission Start");
        if (person.Order > 0) // sipariþi üretmeye gidiyor veya alýyor
        {
            //person.MissionList.Add("sipariþi üretmeye gidiyor veya alýyorken yok oldu");
            table.CancelCoffeePromise(person.Order);
            machine.RemoveFromQueue(person);
            
        }
        else if (person.Order == 0 && person.CoffeeStackLength > 0) // yolda veya kahve daðýtýyor
        {
            //person.MissionList.Add("yolda veya kahve daðýtýyorken yok oldu");
            table.CancelCoffeePromise(person.CoffeeStackLength);
            table.RemoveFromQueue(person);
        }

        table.RemoveWaiterFromServers(person);
        person.CleanCoffeeStack();
        person.SetMissionAndCoroutine(null, null, "null");
        //person.MissionList.Add("HandleStopMission End");
    }
}

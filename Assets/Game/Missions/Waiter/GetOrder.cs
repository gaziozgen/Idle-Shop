using System.Collections;
using UnityEngine;

public class GetOrder : Mission<Waiter>
{
    private Table table = null;

    public IEnumerator SetMission(Waiter waiter, Table table)
    {
        this.table = table;
        person = waiter;
        yield return new WaitForSeconds(0.1f);
        yield return DoMission();
    }

    public override IEnumerator DoMission()
    {
        table.RegisterToServingWaiters(person);

        if (table.HasUnorderedCustomer())
        {
            person.OpenProcessAnimation();
            yield return person.OrderTakeDuration;
            person.CloseProcessAnimation();
            table.GetOrderOfCurrentCustomers();
        }

        person.SetMissionAndCoroutine(null, null, "null");
        int shortage = table.TotalCoffeeNeed() - table.PromisedCoffees;
        if (shortage > 0)
        {
            int order = Mathf.Min(shortage, person.Capacity);
            person.SetOrder(order);
            table.PromiseForCoffee(order);
            table.StartServeMission(person);
            table.CallHelpIfNeeded();
        }
        else
        {
            table.CompleteWaiterInServers(person);
            ShopManager.Instance.WaiterManager.MissionDone(person);
        }
    }

    public override void HandleStopMission()
    {
        //person.MissionList.Add("HandleStopMission Start");
        //person.MissionList.Add("yeni müþteri sipariþi alýrken yok oldu");
        person.CloseProcessAnimation();
        person.SetMissionAndCoroutine(null, null, "null");
        table.RemoveWaiterFromServers(person);
        //person.MissionList.Add("HandleStopMission End");
    }
}

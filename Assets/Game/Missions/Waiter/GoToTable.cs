using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToTable : Mission<Waiter>
{
    private Table table = null;

    public IEnumerator SetMission(Waiter waiter, Table table)
    {
        this.table = table;
        person = waiter;
        yield return DoMission();
    }

    public override IEnumerator DoMission()
    {
        person.SetDestination(table.WaiterInteractionPoint.position);
        yield return person.WaitUntilReached;

        person.TurnTo(table.WaiterInteractionPoint.eulerAngles.y);

        person.SetMissionAndCoroutine(null, null);
        int totalNeed = table.TotalCoffeeNeed();
        if (totalNeed > 0) 
            table.StartServeMission(person);
        else
        {
            table.CloseWaiterRequest();
            ShopManager.Instance.WaiterManager.MissionDone(person);
        }
    }

    public override void HandleStopMission()
    {
        ShopManager.Instance.RequestWaiterToServe(table);
        person.SetMissionAndCoroutine(null, null);
    }
}

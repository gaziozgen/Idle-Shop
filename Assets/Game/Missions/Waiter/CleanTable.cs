using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FateGames.Core;

public class CleanTable : Mission<Waiter>
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
        while (table.GarbageCount > 0)
        {
            yield return person.TrashCollectDuration;
            person.AddTrashToStack(table.GiveGarbage());
        }

        table.CleanTable();
        ShopManager.Instance.Trash.JoinQueue(person);
        yield return person.WaitUntilCoffeeStackFinished;

        person.SetMissionAndCoroutine(null, null, "null");
        ShopManager.Instance.Trash.Dequeue();
        ShopManager.Instance.WaiterManager.MissionDone(person);
    }

    public override void HandleStopMission()
    {
        //person.MissionList.Add("HandleStopMission Start");
        if (table.GarbageCount > 0)
        {
            //person.MissionList.Add("msaayý temizlemeye gidiyor veya çöpleri topoluyorken yok oldu");
            ShopManager.Instance.RequestWaiterToClean(table);
        }
        else
        {
            //person.MissionList.Add("çöpleri kutuya taþýrken yok oldu");
            ShopManager.Instance.Trash.RemoveFromQueue(person);

        }

        person.CleanCoffeeStack();
        person.SetMissionAndCoroutine(null, null, "null");
        //person.MissionList.Add("HandleStopMission End");
    }
}

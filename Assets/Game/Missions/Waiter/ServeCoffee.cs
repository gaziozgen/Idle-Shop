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
        if (table.HasUnorderedCustomer())
        {
            person.OpenProcessAnimation();
            yield return person.OrderTakeDuration;
            person.CloseProcessAnimation();
            table.GetOrderOfCurrentCustomers();
        }

        person.SetOrder(Mathf.Min(table.TotalCoffeeNeed(), person.Capacity));

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
        person.SetDestination(table.WaiterInteractionPoint.position);
        yield return person.WaitUntilReached;
        person.TurnTo(table.WaiterInteractionPoint.eulerAngles.y);

        List<Seat> fullSeats = table.GetFullSeats();
        for (int i = 0; 0 < person.CoffeeStackLength; i++)
        {
            Seat seat = fullSeats[i % fullSeats.Count];
            if (seat.Customer && seat.Customer.CoffeeNeed > 0)
            {
                seat.PutCoffee(person.ServeCoffee());
                yield return person.CoffeeServeDuration;
            }
            else
                fullSeats.Remove(seat);
        }

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
        if (person.Order == 0 && person.CoffeeStackLength == 0) // daha sipariþi almamýþ
            person.CloseProcessAnimation();
        else if (person.Order > 0) // sipariþ üretimini tamamlayamamýþ
            machine.RemoveFromQueue(person);

        if (table.TotalCoffeeNeed() > 0)
            ShopManager.Instance.RequestWaiterToServe(table);
        else
            table.CloseWaiterRequest();


        person.CleanCoffeeStack();
        person.SetMissionAndCoroutine(null, null);
    }
}

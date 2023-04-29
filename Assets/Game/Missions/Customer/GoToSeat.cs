using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToSeat : Mission<Customer>
{
    private Seat seat;

    public IEnumerator SetMission(Customer customer, Seat seat)
    {
        this.seat = seat;
        person = customer;
        yield return DoMission();
    }

    public override IEnumerator DoMission()
    {
        person.SetSeat(seat);
        person.SetDestination(seat.InteractionPoint.position);
        yield return person.WaitUntilReached;
        person.TurnTo(seat.InteractionPoint.eulerAngles.y);
        seat.Sit(person);
        person.StartWaiting();
    }

    public override void HandleStopMission()
    {
        throw new System.NotImplementedException();
    }
}

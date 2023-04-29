using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : Mission<Customer>
{

    public IEnumerator SetMission(Customer customer)
    {
        person = customer;
        yield return DoMission();
    }

    public override IEnumerator DoMission()
    {
        person.SetDestination(ShopManager.Instance.CustomerManager.GetRandomExitPoint());
        yield return person.WaitUntilReached;
        person.Release();
    }

    public override void HandleStopMission()
    {
        throw new System.NotImplementedException();
    }
}

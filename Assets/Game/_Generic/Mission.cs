using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Mission<T> where T : Person
{
    protected T person;

    public bool IsEmpty()
    {
        return person == null;
    }

    public abstract IEnumerator DoMission();
    public abstract void HandleStopMission();

    /*protected void Finish()
    {
        person = null;
    }*/

}



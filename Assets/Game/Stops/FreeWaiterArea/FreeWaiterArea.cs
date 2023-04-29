using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeWaiterArea : MonoBehaviour
{
    [SerializeField] private PersonQueue<Waiter> waiterQueue = null;
    [SerializeField] private Transform queueStart;
    [SerializeField] private float queueSpacing;
    [SerializeField] private float turnRotation = 0;

    private void Awake()
    {
        waiterQueue = new PersonQueue<Waiter>(queueStart, queueSpacing, (Waiter waiter) =>
        {
            return StartCoroutine(LookAndWait(waiter));
        }, (Coroutine routine) => StopCoroutine(routine));
    }

    public void JoinQueue(Waiter waiter)
    {
        waiterQueue.Enqueue(waiter);
    }

    public void RemoveFromArea(Waiter waiter)
    {
        waiterQueue.RemoveImmediate(waiter);
    }

    public Waiter Dequeue()
    {
        return waiterQueue.Dequeue();
    }

    private IEnumerator LookAndWait(Waiter waiter)
    {
        if (waiterQueue.Contains(waiter))
        {
            yield return waiter.WaitUntilReached;
            waiter.TurnTo(turnRotation);
        }
    }
}

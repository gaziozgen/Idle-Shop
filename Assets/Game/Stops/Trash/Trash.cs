using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames.Core;
using DG.Tweening;

public class Trash : FateMonoBehaviour
{
    [SerializeField] private Transform interactionPoint = null;
    [SerializeField] private float queueSpacing = 2f;
    [SerializeField] private int maxQueueLength = 5;
    [SerializeField] private Transform trashTarget = null;

    private PersonQueue<Waiter> waiterQueue;
    private WaitForSeconds trashThrowDuration = new(0.1f);

    private void Awake()
    {
        waiterQueue = new PersonQueue<Waiter>(interactionPoint, queueSpacing, maxQueueLength, (Waiter waiter) =>
        {
            return StartCoroutine(ThrowTrashes(waiter));
        }, (Coroutine routine) => StopCoroutine(routine));
    }

    public void JoinQueue(Waiter waiter)
    {
        waiterQueue.Enqueue(waiter);
    }

    public void Dequeue()
    {
        waiterQueue.Dequeue();
    }

    public void RemoveFromQueue(Waiter waiter)
    {
        waiterQueue.RemoveImmediate(waiter);
    }

    private IEnumerator ThrowTrashes(Waiter waiter)
    {
        yield return waiter.WaitUntilReached;
        waiter.TurnTo(interactionPoint.eulerAngles.y);
        while (waiter.CoffeeStackLength > 0)
        {
            yield return trashThrowDuration;
            ThrowSingleTrash(waiter.ThrowCoffee());
        }
    }

    private void ThrowSingleTrash(Transform trash)
    {
        trash.DOMove(trashTarget.position, 0.15f).OnComplete(() =>
        {
            trash.GetComponent<Coffee>().Release();
        });
    }


}

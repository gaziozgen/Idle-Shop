using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames.Core;
using Unity.Mathematics;
using DG.Tweening;

public class CoffeeMachine : FateMonoBehaviour
{
    [SerializeField] private Transform interactionPoint = null;
    [SerializeField] private Transform coffeeSpawnPoint = null;
    [SerializeField] private float queueSpacing = 2f;
    [SerializeField] private int maxQueueLength = 5;
    [SerializeField] private float baseProduceDuration = 1.0f;
    [SerializeField] private float produceSpeedIncreaseRatioPerLevel = 1.1f;
    [SerializeField] private GameObject prefab;
    [SerializeField] private Transform mesh;
    [SerializeField] private GameObject lockedMesh;
    [SerializeField] private PersonalBubble loadingBubble = null;
    [SerializeField] private ParticleSystem smokeEffect = null;

    public bool Unlocked { get; private set; } = false;

    private FateObjectPool<Coffee> coffeePool;
    private PersonQueue<Waiter> waiterQueue;
    private WaitForSeconds produceDuration_;

    private void Awake()
    {
        coffeePool = new FateObjectPool<Coffee>(prefab, true, 10, 20);
        waiterQueue = new PersonQueue<Waiter>(interactionPoint, queueSpacing, maxQueueLength, (Waiter waiter) =>
        {
            return StartCoroutine(GetOrder(waiter));
        }, (Coroutine routine) => StopCoroutine(routine));
    }

    private IEnumerator GetOrder(Waiter waiter)
    {
        float duration = baseProduceDuration / Mathf.Pow(produceSpeedIncreaseRatioPerLevel, waiter.Level - 1);
        produceDuration_ = new(duration);
        yield return waiter.WaitUntilReached;
        waiter.TurnTo(interactionPoint.eulerAngles.y);

        while (!waiter.OrderTaken)
        {
            loadingBubble.StartLoading(duration);
            yield return produceDuration_;
            waiter.AddCoffeeToStack(ProduceCoffee());
        }
        loadingBubble.Close();

    }

    public void RemoveFromQueue(Waiter waiter)
    {
        waiterQueue.RemoveImmediate(waiter);
        loadingBubble.Close();
    }

    public void Unlock(bool effect)
    {
        if (effect)
        {
            smokeEffect.Play();
            mesh.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        }
        else mesh.localScale = Vector3.one;

        Unlocked = true;
        lockedMesh.SetActive(false);
    }

    public void JoinQueue(Waiter waiter)
    {
        waiterQueue.Enqueue(waiter);
    }

    public void Dequeue()
    {
        waiterQueue.Dequeue();
    }

    public int QueueLength()
    {
        return waiterQueue.QueueLength();
    }

    public Coffee ProduceCoffee()
    {
        return coffeePool.Get(coffeeSpawnPoint.position);
    }
}

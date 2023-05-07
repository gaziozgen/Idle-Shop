using FateGames.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class Table : FateMonoBehaviour
{
    public Transform WaiterInteractionPoint = null;
    [SerializeField] private float queueDistance = 2f;
    [SerializeField] private int maxQueueLength = 5;
    [SerializeField] private List<Seat> seats = new List<Seat>();
    [SerializeField] private GameObject lockedMesh;
    [SerializeField] private Transform table1;
    [SerializeField] private Transform table2;
    [SerializeField] private Transform secondGarbageStackStartPoint;
    [SerializeField] private UnityEvent tableIsReady;
    [SerializeField] private StackComponent garbageStack = null;
    [SerializeField] private ParticleSystem smokeEffect = null;
    [SerializeField] private ParticleSystem cleanEffect = null;

    private PersonQueue<Waiter> waiterQueue;
    private List<Waiter> servingWaiters = new List<Waiter>();

    public int UnlockedSeatCount { get; private set; } = 0;

    public int CoffeeShortage { get => TotalCoffeeNeed() - PromisedCoffees; }
    public int PromisedCoffees { get; private set; } = 0;
    public int GarbageCount { get => garbageStack.Count; }

    private bool waiterReaquested = false;

    private void Awake()
    {
        waiterQueue = new PersonQueue<Waiter>(WaiterInteractionPoint, queueDistance, maxQueueLength, (Waiter waiter) =>
        {
            return StartCoroutine(ServeCoffees(waiter));
        }, (Coroutine routine) => StopCoroutine(routine));
    }

    private IEnumerator ServeCoffees(Waiter waiter)
    {
        yield return waiter.WaitUntilReached;
        waiter.TurnTo(WaiterInteractionPoint.eulerAngles.y);

        List<Seat> fullSeats = GetSeats();
        for (int i = 0; 0 < waiter.CoffeeStackLength; i++)
        {
            Seat seat = fullSeats[i % fullSeats.Count];
            if (seat.Customer && seat.Customer.CoffeeNeed > 0)
            {
                yield return waiter.CoffeeServeDuration;
                seat.PutCoffee(waiter.ServeCoffee());
            }
            else
                fullSeats.Remove(seat);
        }
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

    public bool HasAtLeastOneEmptySeat()
    {
        for (int i = 0; i < UnlockedSeatCount; i++)
        {
            if (!seats[i].IsReserved())
                return true;
        }
        return false;
    }

    public void Unlock(bool effect)
    {
        if (effect) smokeEffect.Play();

        if (UnlockedSeatCount == 0)
        {
            lockedMesh.SetActive(false);
            table1.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
            seats[UnlockedSeatCount].Unlock();
            UnlockedSeatCount++;
            tableIsReady.Invoke();
        }
        else if (UnlockedSeatCount < 4)
        {
            if (UnlockedSeatCount == 2)
            {
                table1.DOKill();
                table1.gameObject.SetActive(false);
                table2.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
                garbageStack.UpdateStackStart(secondGarbageStackStartPoint);
            }
            seats[UnlockedSeatCount].Unlock();
            UnlockedSeatCount++;
            for (int i = 0; i < UnlockedSeatCount - 1; i++)
            {
                if (!seats[i].IsDirty())
                {
                    tableIsReady.Invoke();
                    return;
                }
            }
        }
        else
            Debug.LogError("Unlock Error");
    }

    public Seat GetSeatToSit()
    {
        if (EmptySeatCount() != 0)
        {
            for (int i = 0; i < UnlockedSeatCount; i++)
            {
                if (!seats[i].IsReserved())
                {
                    seats[i].Reserve();
                    return seats[i];
                }
            }
        }
        return null;
    }

    public bool HasUnorderedCustomer()
    {
        for (int i = 0; i < UnlockedSeatCount; i++)
            if (seats[i].Customer && !seats[i].Customer.Ordered)
                return true;

        return false;
    }

    public void GetOrderOfCurrentCustomers()
    {
        for (int i = 0; i < UnlockedSeatCount; i++)
            if (seats[i].Customer && !seats[i].Customer.Ordered)
                seats[i].Customer.UpdateOrder();
    }

    private int EmptySeatCount()
    {
        int totalEmptySeat = 0;
        for (int i = 0; i < UnlockedSeatCount; i++)
        {
            if (!seats[i].IsReserved()) totalEmptySeat++;
        }
        return totalEmptySeat;
    }

    public void CallWaiter()
    {
        if (!waiterReaquested)
        {
            waiterReaquested = true;
            ShopManager.Instance.RequestWaiterToServe(this);
        }
    }

    public void PutGarbage(Transform coffee)
    {
        garbageStack.Push(coffee);
    }

    public Transform GiveGarbage()
    {
        return garbageStack.Pop();
    }

    public int TotalCoffeeNeed()
    {
        int totalNeed = 0;
        for (int i = 0; i < UnlockedSeatCount; i++)
        {
            Customer customer = seats[i].Customer;
            if (customer)
                totalNeed += customer.CoffeeNeed;
        }
        return totalNeed;
    }

    public void PromiseForCoffee(int count)
    {
        PromisedCoffees += count;
    }

    public void CancelCoffeePromise(int count)
    {
        PromisedCoffees -= count;
    }

    public void RegisterToServingWaiters(Waiter waiter)
    {
        if (!servingWaiters.Contains(waiter))
            servingWaiters.Add(waiter);
    }

    public void CompleteWaiterInServers(Waiter waiter)
    {
        servingWaiters.Remove(waiter);

        if (servingWaiters.Count == 0)
            waiterReaquested = false;
    }

    public void RemoveWaiterFromServers(Waiter waiter)
    {
        servingWaiters.Remove(waiter);

        // görevin baþarýsýz olduðu tüm durumlarda kahveler yarým kaldýðý için
        // kesinlikle baþka garson gerekiyor
        if (servingWaiters.Count == 0)
            ShopManager.Instance.RequestWaiterToServe(this);
    }

    public void InformTableThatCoffeeGetted()
    {
        PromisedCoffees -= 1;
    }

    public void StartOrderMission(Waiter waiter)
    {
        GetOrder mission = new GetOrder();

        waiter.SetMissionAndCoroutine(StartCoroutine(mission.SetMission(waiter, this)), mission, "GetOrder");
    }

    public void StartServeMission(Waiter waiter)
    {
        ServeCoffee mission = new ServeCoffee();
        waiter.SetMissionAndCoroutine(StartCoroutine(mission.SetMission(waiter, this)), mission, "ServeCoffee");
    }

    public void CallHelpIfNeeded()
    {
        if (CoffeeShortage > 0)
        {
            List<Waiter> waiters = ShopManager.Instance.WaiterManager.GetFreeWaitersUpToCapacity(CoffeeShortage);

            for (int i = 0; i < waiters.Count; i++)
            {
                int order = Mathf.Min(waiters[i].Capacity, CoffeeShortage);
                waiters[i].SetOrder(order);
                PromiseForCoffee(order);
                StartServeMission(waiters[i]);

                if (CoffeeShortage == 0) break;
            }
        }
    }

    public void CloseWaiterRequest()
    {
        waiterReaquested = false;
    }

    public List<Seat> GetSeats()
    {
        List<Seat> fullSeats = new List<Seat>();

        for (int i = 0; i < UnlockedSeatCount; i++)
            fullSeats.Add(seats[i]);

        return fullSeats;
    }

    public void CheckTableFinish()
    {
        bool finish = true;
        for (int i = 0; i < UnlockedSeatCount; i++)
            if (!seats[i].IsDirty() && seats[i].IsReserved())
                finish = false;

        if (finish)
        {
            for (int i = 0; i < UnlockedSeatCount; i++)
                seats[i].Reserve();
            ShopManager.Instance.RequestWaiterToClean(this);
        }
    }

    public void CleanTable()
    {
        for (int i = 0; i < UnlockedSeatCount; i++)
            seats[i].CleanSeat();
        cleanEffect.Play();
        tableIsReady.Invoke();
    }
}

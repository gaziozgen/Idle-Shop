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
    [SerializeField] private List<Seat> seats = new List<Seat>();
    [SerializeField] private GameObject lockedMesh;
    [SerializeField] private Transform table1;
    [SerializeField] private Transform table2;
    [SerializeField] private Transform secondGarbageStackStartPoint;
    [SerializeField] private UnityEvent tableIsReady;
    [SerializeField] private StackComponent garbageStack = null;
    [SerializeField] private ParticleSystem smokeEffect = null;
    [SerializeField] private ParticleSystem cleanEffect = null;

    public int UnlockedSeatCount { get; private set; } = 0;
    public int GarbageCount { get => garbageStack.Count; }

    private bool empty = true;
    private bool waiterReaquested = false;

    public bool IsEmpty()
    {
        return empty;
    }

    public void Unlock()
    {
        smokeEffect.Play();
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
                table1.localScale = Vector3.zero;
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
                    if (empty) empty = false;
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

    public void StartServeMission(Waiter waiter)
    {
        ServeCoffee mission = new ServeCoffee();
        waiter.SetMissionAndCoroutine(StartCoroutine(mission.SetMission(waiter, this)), mission);
    }

    public void CloseWaiterRequest()
    {
        waiterReaquested = false;
    }

    public List<Seat> GetFullSeats()
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
        empty = true;
        for (int i = 0; i < UnlockedSeatCount; i++)
            seats[i].CleanSeat();
        cleanEffect.Play();
        tableIsReady.Invoke();
    }
}

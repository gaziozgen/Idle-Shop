using FateGames.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PersonQueue<T> where T : Person
{
    private Func<T, Coroutine> OnReach;
    private Action<Coroutine> stopCoroutine;

    private Transform startPosition = null;
    private float distance = 2;
    private List<T> list = new List<T>();

    private Coroutine lastCoroutine = null;

    public PersonQueue(Transform startPosition, float distance, Func<T, Coroutine> onReach, Action<Coroutine> stopCoroutine)
    {
        this.startPosition = startPosition;
        this.distance = distance;
        this.stopCoroutine = stopCoroutine;
        OnReach = onReach;
    }

    public void Enqueue(T person)
    {
        list.Add(person);
        person.SetDestination(startPosition.position - startPosition.forward * distance * (list.Count - 1));
        if (list.Count == 1) lastCoroutine = OnReach(person);
    }

    public T Dequeue()
    {
        T person = list[0];
        list.RemoveAt(0);
        stopCoroutine(lastCoroutine);
        AdjustPositions();
        return person;
    }

    public bool Contains(T person)
    {
        return list.Contains(person);
    }

    public void RemoveImmediate(T person)
    {
        int index = list.IndexOf(person);
        list.Remove(person);
        if (index == 0) stopCoroutine(lastCoroutine);
        AdjustPositions();
    }

    public T Peek()
    {
        return list[0];
    }

    public int QueueLength() { return list.Count; }

    private void AdjustPositions()
    {
        for (int i = 0; i < list.Count; i++)
        {
            list[i].SetDestination(startPosition.position - startPosition.forward * distance * i);
            if (i == 0) lastCoroutine = OnReach(list[0]);
        }
    }
}

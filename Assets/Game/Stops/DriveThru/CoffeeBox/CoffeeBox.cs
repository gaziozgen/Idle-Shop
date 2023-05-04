using DG.Tweening;
using FateGames.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoffeeBox : FateMonoBehaviour, IPooledObject
{
    [SerializeField] private List<Transform> coffeePositions = null;

    private List<Coffee> coffees =  new List<Coffee>();

    public int CoffeeCount { get; private set; } = 0;


    public void PutCoffee(Coffee coffee)
    {
        coffee.transform.parent = transform;
        coffee.transform.DOMove(coffeePositions[CoffeeCount].position, 0.2f);
        coffee.transform.DORotate(coffeePositions[CoffeeCount].eulerAngles, 0.2f);
        coffees.Add(coffee);
        CoffeeCount++;
    }





    public Action Release { get; set; }

    public void OnObjectSpawn()
    {

    }

    public void OnRelease()
    {
        for (int i = 0; i < coffees.Count; i++)
        {
            coffees[i].Release();
        }
        coffees.Clear();
        CoffeeCount = 0;
    }
}

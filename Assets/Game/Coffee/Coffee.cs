using FateGames.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coffee : FateMonoBehaviour, IPooledObject
{
    [SerializeField] private GameObject newCoffee = null;
    [SerializeField] private GameObject garbage = null;
    private bool notDrinked = true;

    public void Drink()
    {
        if (notDrinked)
        {
            newCoffee.SetActive(false);
            garbage.SetActive(true);
            notDrinked = false;
        }
        else
            Debug.LogError("Coffee is already drunk");
    }

    public Action Release { get; set; }

    public void OnObjectSpawn()
    {
        notDrinked = true;
        newCoffee.SetActive(true);
        garbage.SetActive(false);
    }

    public void OnRelease()
    {
        
    }
}

using FateGames.Core;
using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : FateMonoBehaviour
{
    [SerializeField] private GameObject customerPrafab = null;
    [SerializeField] private CustomerSpawnPoints customerSpawnPoints = null;
    [SerializeField] private SaveDataVariable saveData;
    [SerializeField] private float maxCoffeeNeedRatioToSeats = 0.2f;

    private FateObjectPool<Customer> pool;
    private List<Vector3> spawnPoints = null;

    private void Awake()
    {
        pool = new FateObjectPool<Customer>(customerPrafab, true, 20, 50);
        spawnPoints = customerSpawnPoints.GetSpawnPoints();
    }

    public Customer Spawn()
    {
        Customer customer = pool.Get();
        customer.SetCoffeeNeed(Random.Range(1, 1 + Mathf.CeilToInt(saveData.Value.SeatCount * maxCoffeeNeedRatioToSeats)));
        customer.SetAgentPosition(spawnPoints[Random.Range(0, spawnPoints.Count)]);
        ShopManager.Instance.Reception.JoinQueue(customer);
        return customer;
    }

    public Vector3 GetRandomExitPoint()
    {
        return spawnPoints[Random.Range(0, spawnPoints.Count)];
    }

}

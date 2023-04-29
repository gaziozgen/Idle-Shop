using FateGames.Core;
using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : FateMonoBehaviour
{
    [SerializeField] private GameObject customerPrafab = null;
    [SerializeField] private CustomerSpawnPoints customerSpawnPoints = null;

    private FateObjectPool<Customer> pool;
    private List<Vector3> spawnPoints = null;

    private void Awake()
    {
        pool = new FateObjectPool<Customer>(customerPrafab, true, 20, 50);
        spawnPoints = customerSpawnPoints.GetSpawnPoints();
    }

    public Customer Spawn()
    {
        return pool.Get(spawnPoints[Random.Range(0, spawnPoints.Count)]);
    }

    public Vector3 GetRandomExitPoint()
    {
        return spawnPoints[Random.Range(0, spawnPoints.Count)];
    }

}

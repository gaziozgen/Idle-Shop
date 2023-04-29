using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames.Core;

public class CustomerSpawnPoints : FateMonoBehaviour
{
    private List<Vector3> spawnPoints = new List<Vector3>();

    public List<Vector3> GetSpawnPoints()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            spawnPoints.Add(transform.GetChild(i).position);
        }
        return spawnPoints;
    }
}

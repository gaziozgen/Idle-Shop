using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeWaiterArea : MonoBehaviour
{
    private static FreeWaiterArea instance = null;
    public static FreeWaiterArea Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<FreeWaiterArea>();
            return instance;
        }
    }

    [SerializeField] private List<Transform> waiterPositions;
    [SerializeField] private int firstAreaRange = 10;

    private List<int> emptyPositionIndexList = new List<int>();
    private List<(int, Waiter)> fullPositionIndexList = new List<(int, Waiter)>();

    private void Awake()
    {
        for (int i = 0; i < firstAreaRange; i++)
            emptyPositionIndexList.Add(i);
    }

    public void UpgradePositions()
    {
        for (int i = firstAreaRange; i < waiterPositions.Count; i++)
            emptyPositionIndexList.Add(i);
    }

    public void Join(Waiter waiter)
    {
        int indexInEmptyPosIndexList = Random.Range(0, emptyPositionIndexList.Count);
        int indexInWaiterPositionList = emptyPositionIndexList[indexInEmptyPosIndexList];
        emptyPositionIndexList.RemoveAt(indexInEmptyPosIndexList);
        fullPositionIndexList.Add((indexInWaiterPositionList, waiter));
        waiter.SetDestination(waiterPositions[indexInWaiterPositionList].position);
    }

    public void RemoveFromArea(Waiter waiter)
    {
        int indexInFullPosIndexList = -1;
        for (int i = 0; i < fullPositionIndexList.Count; i++)
            if (fullPositionIndexList[i].Item2 == waiter)
                indexInFullPosIndexList = i;

        if (indexInFullPosIndexList != -1)
        {
            int indexInWaiterPositionList = fullPositionIndexList[indexInFullPosIndexList].Item1;
            fullPositionIndexList.RemoveAt(indexInFullPosIndexList);
            emptyPositionIndexList.Add(indexInWaiterPositionList);
        }
        else
            Debug.LogError("Waiter not in area", waiter);
    }

    public Waiter ReleaseOldest()
    {
        Waiter waiter = fullPositionIndexList[0].Item2;
        int posIndex = fullPositionIndexList[0].Item1;
        fullPositionIndexList.RemoveAt(0);
        emptyPositionIndexList.Add(posIndex);
        return waiter;
    }
}

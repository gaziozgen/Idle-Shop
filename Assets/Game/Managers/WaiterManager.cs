using DG.Tweening;
using FateGames.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaiterManager : FateMonoBehaviour
{
    [SerializeField] private int MaxWaiterCount = 10;
    [SerializeField] private GameObject waiterPrafab = null;
    [SerializeField] private FreeWaiterArea freeWaiterArea = null;
    [SerializeField] private SaveDataVariable saveData;
    [SerializeField] private ParticleSystem smokeEffect = null;

    private FateObjectPool<Waiter> pool;
    private List<Waiter> freeWaiterList = new List<Waiter>();
    private List<Waiter> busyWaiterList = new List<Waiter>();
    private Queue<Action<Waiter>> missions = new Queue<Action<Waiter>>();


    private void Awake()
    {
        pool = new FateObjectPool<Waiter>(waiterPrafab, true, 20, 50);
        missions = new Queue<Action<Waiter>>();
        LoadFromData();
    }

    private void LoadFromData()
    {
        int[] waiters = saveData.Value.waiters;
        for (int i = 0; i < waiters.Length; i++)
        {
            for (int j = 0; j < waiters[i]; j++)
            {
                AddWaiter(i + 1, Vector3.zero, false, false, false);
            }
        }
    }

    public List<Waiter> GetFreeWaitersUpToCapacity(int capacityNeeded)
    {
        List<Waiter> waiters = new List<Waiter>();
        while (0 < freeWaiterList.Count)
        {
            Waiter waiter = freeWaiterList[0];
            capacityNeeded -= waiter.Capacity;
            waiters.Add(waiter);
            freeWaiterList.Remove(waiter);
            freeWaiterArea.RemoveFromArea(waiter);
            busyWaiterList.Add(waiter);

            if (capacityNeeded <= 0) break;
        }
        return waiters;
    }

    public bool IsBuyAvaliable()
    {
        if (freeWaiterList.Count + busyWaiterList.Count < MaxWaiterCount) return true;
        else return false;
    }

    public void AddWaiter(int level, Vector3 pos, bool focus = true, bool editSave = true, bool effect = true)
    {
        if (editSave)
        {
            saveData.Value.soldierBuyLevel++;
            saveData.Value.waiters[level - 1]++;
        }
        if (effect)
        {
            smokeEffect.transform.position = pos + Vector3.up;
            smokeEffect.Play();
        }
        if (focus) FreeIdleCameraController.Instance.Focus(pos, 0.5f);
        Waiter waiter = pool.Get();
        waiter.SetAgentPosition(pos);
        waiter.SetLevel(level);
        freeWaiterList.Add(waiter);
        freeWaiterArea.Join(waiter);
        ManageWaiters();
    }

    public void AddMission(Action<Waiter> mission)
    {
        missions.Enqueue(mission);
        ManageWaiters();
    }

    public void MissionDone(Waiter waiter)
    {
        busyWaiterList.Remove(waiter);
        freeWaiterList.Add(waiter);
        freeWaiterArea.Join(waiter);
        ManageWaiters();
    }

    private void ManageWaiters()
    {
        if (freeWaiterList.Count > 0 && missions.Count > 0)
        {
            Waiter waiter = freeWaiterArea.ReleaseOldest();
            freeWaiterList.Remove(waiter);
            Action<Waiter> mission = missions.Dequeue();
            busyWaiterList.Add(waiter);
            mission(waiter);
        }
    }

    public bool IsMergeAvaliable()
    {
        int targetWaiterCountToMerge = 3;
        List<Waiter> waitersToMerge = new List<Waiter>();

        for (int currentLookingLevel = 1; currentLookingLevel <= saveData.Value.waiters.Length; currentLookingLevel++)
        {
            waitersToMerge.Clear();
            for (int i = 0; i < freeWaiterList.Count; i++)
                if (freeWaiterList[i].Level == currentLookingLevel)
                    waitersToMerge.Add(freeWaiterList[i]);

            for (int i = 0; i < busyWaiterList.Count; i++)
                if (busyWaiterList[i].Level == currentLookingLevel)
                    waitersToMerge.Add(busyWaiterList[i]);

            if (waitersToMerge.Count >= targetWaiterCountToMerge)
                return true;
        }

        return false;
    }

    public IEnumerator Merge()
    {
        float duration = 0.6f;
        int targetWaiterCountToMerge = 3;
        List<Waiter> waitersToMerge = new List<Waiter>();

        for (int currentLookingLevel = 1; currentLookingLevel <= saveData.Value.maxAchivedWaiterLevel; currentLookingLevel++)
        {
            waitersToMerge.Clear();
            for (int i = 0; i < freeWaiterList.Count; i++)
                if (freeWaiterList[i].Level == currentLookingLevel)
                    waitersToMerge.Add(freeWaiterList[i]);

            for (int i = 0; i < busyWaiterList.Count; i++)
                if (busyWaiterList[i].Level == currentLookingLevel)
                    waitersToMerge.Add(busyWaiterList[i]);

            if (waitersToMerge.Count >= targetWaiterCountToMerge)
            {
                Vector3 mergePoint = Vector3.zero;
                for (int i = 0; i < targetWaiterCountToMerge; i++)
                {
                    mergePoint += waitersToMerge[i].transform.position;
                }
                mergePoint /= targetWaiterCountToMerge;
                FreeIdleCameraController.Instance.Focus(mergePoint, 1f);

                for (int i = 0; i < targetWaiterCountToMerge; i++)
                {
                    Waiter waiter = waitersToMerge[i];
                    if (waiter.Mission != null)
                    {
                        StopCoroutine(waiter.MissionCoroutine);
                        waiter.Mission.HandleStopMission();
                        busyWaiterList.Remove(waiter);
                    }
                    else
                    {
                        freeWaiterList.Remove(waiter);
                        freeWaiterArea.RemoveFromArea(waiter);
                    }

                    waiter.SetShadow(false);
                    waiter.SetAgentEnabled(false);
                    waiter.transform.DOMove((mergePoint + waiter.transform.position) / 2 + Vector3.up * 5, duration / 2).SetEase(Ease.OutSine).OnComplete(() =>
                    {
                        waiter.transform.DOMove(mergePoint, duration / 2).SetEase(Ease.InSine).OnComplete(() =>
                        {
                            waiter.SetShadow(true);
                            waiter.SetAgentEnabled(true);
                            waiter.Release();
                        });
                    });
                }


                yield return new WaitUntil(() => waitersToMerge[0].gameObject.activeSelf == false);

                saveData.Value.waiters[currentLookingLevel - 1] -= targetWaiterCountToMerge;
                saveData.Value.waiters[currentLookingLevel]++;
                saveData.Value.soldierMergeLevel++;

                int level = currentLookingLevel + 1;
                if (saveData.Value.maxAchivedWaiterLevel < currentLookingLevel + 1)
                    saveData.Value.maxAchivedWaiterLevel = currentLookingLevel + 1;

                AddWaiter(level, mergePoint, false, false);
                UpgradeButtonsController.Instance.UpdateMergeButton();

                break;
            }
        }
    }
}

public partial class SaveData
{
    public int[] waiters = new int[6] { 0, 0, 0, 0, 0, 0 };
    public int soldierBuyLevel = 0;
    public int soldierMergeLevel = 0;
    public int maxAchivedWaiterLevel = 1;
}

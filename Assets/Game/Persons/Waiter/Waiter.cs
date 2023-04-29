using FateGames.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waiter : Person, IPooledObject
{
    [SerializeField] private int baseCapacity = 1;
    [SerializeField] private int capacityIncreaseRatio = 3;
    [SerializeField] private List<Material> levelMaterials = new List<Material>();
    [SerializeField] private SkinnedMeshRenderer mesh = null;
    [SerializeField] private float stackOperationsSpeedIncreaseRatioPerLevel = 1.2f;
    [SerializeField] private float baseCoffeeServeDuration = 1f;
    [SerializeField] private float baseTrashCollectDuration = 1f;
    [SerializeField] private float orderTakeDuration = 1f;
    [SerializeField] private float baseWalkSpeed = 1f;
    [SerializeField] private Transform stackStartPoint;
    [SerializeField] private float stackDistance;
    [SerializeField] private StackComponent coffeeStack;
    [SerializeField] private FloatReference speedMultiplier;
    [SerializeField] private Animator animator;
    [SerializeField] private PersonalBubble bubble;

    public Coroutine MissionCoroutine { get; private set; } = null;
    public Mission<Waiter> Mission { get; private set; } = null;

    public int Level { get; private set; } = 1;
    public int Capacity { get => baseCapacity * (int)MathF.Pow(capacityIncreaseRatio, Level - 1); }

    public WaitForSeconds CoffeeServeDuration { get; private set; }
    public WaitForSeconds TrashCollectDuration { get; private set; }
    public WaitForSeconds OrderTakeDuration { get; private set; }

    public WaitUntil WaitUntilOrderTaken { get; private set; }
    public WaitUntil WaitUntilCoffeeStackFinished { get; private set; }
    public bool OrderTaken { get => Order == coffeeStack.Count; }
    public int CoffeeStackLength { get => coffeeStack.Count; }
    public int Order { get; private set; } = 0;

    protected override void Awake()
    {
        base.Awake();
        OrderTakeDuration = new(orderTakeDuration);
        WaitUntilOrderTaken = new(() => OrderTaken);
        WaitUntilCoffeeStackFinished = new(() => coffeeStack.Count == 0);
    }

    public override void SetDestination(Vector3 target)
    {
        base.SetDestination(target);
        animator.SetBool("Walking", true);
        UpdateAnimatorSpeed();
        StartCoroutine(StopOnReach());
    }

    private IEnumerator StopOnReach()
    {
        yield return WaitUntilReached;
        animator.SetBool("Walking", false);
        animator.speed = 1;
    }

    public void SetLevel(int level)
    {
        Level = level;
        mesh.material = levelMaterials[level - 1];
        CoffeeServeDuration = new(baseCoffeeServeDuration / Mathf.Pow(stackOperationsSpeedIncreaseRatioPerLevel, level));
        TrashCollectDuration = new(baseTrashCollectDuration / Mathf.Pow(stackOperationsSpeedIncreaseRatioPerLevel, level));
    }

    public void SetMissionAndCoroutine(Coroutine coroutine, Mission<Waiter> mission)
    {
        MissionCoroutine = coroutine;
        Mission = mission;
    }

    public void OpenProcessAnimation()
    {
        bubble.StartLoading(orderTakeDuration);
    }

    public void CloseProcessAnimation()
    {
        bubble.Close();
    }

    public void SetOrder(int order)
    {
        Order = order;
    }

    public void CleanCoffeeStack()
    {
        while (coffeeStack.Count > 0)
        {
            coffeeStack.Pop().GetComponent<Coffee>().Release();
        }
    }

    public void AddCoffeeToStack(Coffee coffee)
    {
        if (coffeeStack.Count == 0) animator.SetBool("Carrying", true);
        coffeeStack.Push(coffee.transform);
    }

    public void AddTrashToStack(Transform coffeeTrash)
    {
        if (coffeeStack.Count == 0) animator.SetBool("Carrying", true);
        coffeeStack.Push(coffeeTrash.transform);
    }

    public Transform ServeCoffee()
    {
        if (coffeeStack.Count == 1) animator.SetBool("Carrying", false);
        return coffeeStack.Pop();
    }

    public Transform ThrowCoffee()
    {
        if (coffeeStack.Count == 1) animator.SetBool("Carrying", false);
        return coffeeStack.Pop();
    }

    public void UpdateWaiterSpeedMultiplier()
    {
        agent.speed = baseWalkSpeed * speedMultiplier.Value;
        if (animator.GetBool("Walking")) UpdateAnimatorSpeed();
    }

    private void UpdateAnimatorSpeed()
    {
        animator.speed = agent.speed / 1.75f;
    }


    public Action Release { get; set; }
    public void OnObjectSpawn()
    {
        Order = 0;
        UpdateWaiterSpeedMultiplier();
        agent.enabled = true;
    }
    public void OnRelease()
    {
        agent.enabled = false;
    }
}

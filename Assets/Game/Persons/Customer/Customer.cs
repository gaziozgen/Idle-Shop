using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.AI;
using FateGames.Core;

public class Customer : Person, IPooledObject
{
    [SerializeField] private PersonalBubble bubble;
    [SerializeField] private float baseWalkSpeed = 1f;
    [SerializeField] private float waitTimeBeforeGettingAngry = 10f;
    [SerializeField] private Animator animator;
    [SerializeField] private AnimationClip drinkAnimation;
    [SerializeField] private Transform coffeeParentOnHand;
    [SerializeField] private ParticleSystem[] sadEffects = null;
    [SerializeField] private ParticleSystem happyEffect = null;
    [SerializeField] private ParticleSystem moneyEffect = null;
    [SerializeField] private ParticleSystem leaveEffect = null;

    public int CoffeeNeed { get; private set; } = 0;
    public bool Ordered { get; private set; } = false;

    private Seat seat = null;
    private bool drinking = false;
    private bool happy = true;

    private WaitForSeconds coffeeDrinkDuration;
    private WaitForSeconds _waitTimeBeforeGettingAngry;

    private int drinkedCoffees;

    protected override void Awake()
    {
        base.Awake();
        _waitTimeBeforeGettingAngry = new(waitTimeBeforeGettingAngry);
        coffeeDrinkDuration = new(drinkAnimation.length);
        agent.speed = baseWalkSpeed;
    }

    public override void SetDestination(Vector3 target)
    {
        base.SetDestination(target);
        animator.SetTrigger("Walk");
        animator.speed = agent.speed / 1.75f;
        StartCoroutine(OnReach());
    }

    private IEnumerator OnReach()
    {
        yield return WaitUntilReached;
        animator.speed = 1;
        if (drinkedCoffees == 0 && seat) animator.SetTrigger("Sit");
        else animator.SetTrigger("Idle");
    }

    public void SetCoffeeNeed(int count)
    {
        CoffeeNeed = count;
    }

    public void SetSeat(Seat seat)
    {
        this.seat = seat;
    }

    public void StartWaiting()
    {
        IEnumerator WaitingWaiter()
        {
            yield return _waitTimeBeforeGettingAngry;
            if (!Ordered)
            {
                sadEffects[Random.Range(0, 2)].Play();
                happy = false;
            }
        }

        StartCoroutine(WaitingWaiter());
    }

    public void TakeCoffee()
    {
        CoffeeNeed--;
        seat.Table.InformTableThatCoffeeGetted();
        UpdateOrder();

        if (!drinking)
            StartCoroutine(DrinkCoffee());
    }

    public void UpdateOrder()
    {
        if (!Ordered) Ordered = true;
        if (CoffeeNeed == 0)
            bubble.Close();
        else
            bubble.SetText(CoffeeNeed.ToString());
    }

    private IEnumerator DrinkCoffee()
    {
        drinking = true;

        yield return new WaitForSeconds(1f);
        if (happy) happyEffect.Play();
        animator.SetTrigger("Drink");

        while (seat.CoffeeCount > 0)
        {
            Coffee coffee = seat.GiveCoffeeToCustomer().GetComponent<Coffee>();
            coffee.transform.parent = coffeeParentOnHand;
            coffee.transform.localPosition = Vector3.zero;
            coffee.transform.localRotation = Quaternion.identity;
            yield return coffeeDrinkDuration;
            coffee.Drink();
            UIMoney.Instance.Add(1, transform.position, happy);
            coffee.transform.parent = null;
            coffee.transform.localRotation = Quaternion.identity;
            drinkedCoffees++;
            seat.Table.PutGarbage(coffee.transform);
        }

        animator.SetTrigger("Sit");
        drinking = false;

        if (CoffeeNeed == 0)
        {
            seat.LeaveSeat();
            leaveEffect.Play();
            StartCoroutine(new Exit().SetMission(this));
            moneyEffect.Play();
        }
    }

    public System.Action Release { get; set; }

    public void OnObjectSpawn()
    {
        happy = true;
        Ordered = false;
        seat = null;
        drinkedCoffees = 0;
        drinking = false;
        agent.enabled = true;
    }

    public void OnRelease()
    {
        leaveEffect.Stop();
        agent.enabled = false;
    }
}

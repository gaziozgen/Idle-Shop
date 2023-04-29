using DG.Tweening;
using FateGames.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Reception : FateMonoBehaviour
{
    [SerializeField] private Transform startPosition = null;
    [SerializeField] private float distance = 2;
    [SerializeField] private float defaultCustomerCallInterval = 5;
    [SerializeField] private PersonalBubble bubble;
    [SerializeField] private float registerDuration = 1;
    [SerializeField] private Animator animator;

    private PersonQueue<Customer> customerQueue;
    private Customer waitingCustomer = null;
    private WaitForSeconds _defaultCustomerCallInterval;
    private WaitForSeconds _registerDuration;

    private int expectedCustomers = 0;

    private void Awake()
    {
        _registerDuration = new WaitForSeconds(registerDuration);
        _defaultCustomerCallInterval = new(defaultCustomerCallInterval);
        customerQueue = new PersonQueue<Customer>(startPosition, distance, (Customer customer) =>
        {
            return StartCoroutine(Register(customer));
        }, (Coroutine routine) => StopCoroutine(routine));
    }

    private void Start()
    {
        StartCoroutine(DefaultCustomerCall());
    }

    private IEnumerator Register(Customer customer)
    {
        yield return customer.WaitUntilReached;
        customer.TurnTo(startPosition.eulerAngles.y);
        StartCoroutine(Place(customer));
    }

    public void JoinQueue(Customer customer)
    {
        customerQueue.Enqueue(customer);
    }

    public IEnumerator Place(Customer customer)
    {
        Seat seat = ShopManager.Instance.TableManager.GetEmptySeatIfAny();
        if (seat)
        {
            animator.SetTrigger("Press");
            bubble.StartLoading(registerDuration);
            yield return _registerDuration;
            bubble.Close();

            expectedCustomers--;
            StartCoroutine(new GoToSeat().SetMission(customerQueue.Dequeue(), seat));
            if (expectedCustomers < 3) CallCustomer(1, 2);
            else if (expectedCustomers < 6) CallCustomer(1, 5);
        }
        else
            waitingCustomer = customer;
    }

    public void PlaceWaitingCustomer()
    {
        if (waitingCustomer)
        {
            StartCoroutine(Place(waitingCustomer));
            waitingCustomer = null;
        }
    }

    private IEnumerator DefaultCustomerCall()
    {
        if (expectedCustomers < 6) CallCustomer(1, 5);
        yield return _defaultCustomerCallInterval;
        yield return DefaultCustomerCall();
    }

    private void CallCustomer(float minDelay, float maxDelay)
    {
        expectedCustomers++;
        DOVirtual.DelayedCall(Random.Range(minDelay, maxDelay), () =>
        {
            ShopManager.Instance.RequestCustomer();
        });
    }
}


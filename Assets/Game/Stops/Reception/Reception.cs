using DG.Tweening;
using FateGames.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reception : FateMonoBehaviour
{
    [SerializeField] private Transform startPosition = null;
    [SerializeField] private float distance = 2;
    [SerializeField] private int maxQueueLength = 5;
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
        customerQueue = new PersonQueue<Customer>(startPosition, distance, maxQueueLength, (Customer customer) =>
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
        //Debug.Log("Place", customer);
        Seat seat = ShopManager.Instance.TableManager.GetEmptySeatIfAny();
        if (seat)
        {
            //Debug.Log("Seat found", customer);
            animator.SetTrigger("Press");
            bubble.StartLoading(registerDuration);
            yield return _registerDuration;
            bubble.Close();

            expectedCustomers--;
            StartCoroutine(new GoToSeat().SetMission(customerQueue.Dequeue(), seat));
            if (expectedCustomers < 3) CallCustomer(0.5f, 1.5f);
        }
        else if (waitingCustomer == null)
        {
            //Debug.Log("Seat not found, customer assigned to waiting customer", customer);
            waitingCustomer = customer;
        }
        /*else
        {
            Debug.LogError("Seat not found and waiting customer is full", customer);
            Debug.LogError("Waiting customer is", waitingCustomer);
        }*/

    }

    public void PlaceWaitingCustomer()
    {
        //Debug.Log("Place waiting customer");
        if (waitingCustomer)
        {
            //Debug.Log("yes waiting customer");
            Customer customer = waitingCustomer;
            waitingCustomer = null;
            StartCoroutine(Place(customer));
        }
        else
        {
            //Debug.Log("no waiting customer");
        }
    }

    private IEnumerator DefaultCustomerCall()
    {
        if (expectedCustomers < 5) CallCustomer(0, 2);
        yield return _defaultCustomerCallInterval;
        yield return DefaultCustomerCall();
    }

    private void CallCustomer(float minDelay, float maxDelay)
    {
        expectedCustomers++;
        DOVirtual.DelayedCall(Random.Range(minDelay, maxDelay), () =>
        {
            ShopManager.Instance.CustomerManager.Spawn();
        });
    }
}
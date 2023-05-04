using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames.Core;
using DG.Tweening;
using PathCreation;

public class DriveThru : FateMonoBehaviour
{
    [SerializeField] private Vector2 carRespawnDelayRange = Vector2.zero;
    [SerializeField] private float maxCoffeeNeedRatioToSeats = 1;
    [SerializeField] private SaveDataVariable saveData = null;
    [SerializeField] private Transform mesh = null;
    [SerializeField] private GameObject lockedImage = null;
    [SerializeField] private Transform carTransform = null;
    [SerializeField] private PathCreator PathCreator = null;
    [SerializeField] private PersonalBubble carBubble = null;
    [SerializeField] private StackComponent coffeeBoxStack = null;
    [SerializeField] private Transform queueStartPos = null;
    [SerializeField] private Transform stackStartPos = null;
    [SerializeField] private GameObject coffeeBoxPrefab = null;
    [SerializeField] private float distance = 2;
    [SerializeField] private int maxLength = 2;
    [SerializeField] private float coffeeDropDuration = 0.3f;
    [SerializeField] private Animator carAnimator = null;
    [SerializeField] private Animator cashierAnimator = null;
    [SerializeField] private ParticleSystem smokeEffect = null;


    public int CoffeeShortage { get => coffeeNeed - promisedCoffees; }

    private int coffeeNeed = 0;
    private int promisedCoffees = 0;
    private int dropedCoffees = 0;
    private PersonQueue<Waiter> waiterQueue = null;
    private FateObjectPool<CoffeeBox> boxPool;
    private WaitForSeconds _coffeeDropDuration;
    private CoffeeBox lastBox = null;

    private void Awake()
    {
        _coffeeDropDuration = new(coffeeDropDuration);
        boxPool = new FateObjectPool<CoffeeBox>(coffeeBoxPrefab, true, 20, 50);
        waiterQueue = new PersonQueue<Waiter>(queueStartPos, distance, maxLength, (Waiter waiter) =>
        {
            return StartCoroutine(DropCoffees(waiter));
        }, (Coroutine routine) => StopCoroutine(routine));

        if (!Unlocked)
        {
            mesh.localScale = Vector3.zero;
            lockedImage.SetActive(true);
        }
    }

    private void Start()
    {
        if (Unlocked) CallCar();
    }

    private IEnumerator DropCoffees(Waiter waiter)
    {
        yield return waiter.WaitUntilReached;
        waiter.TurnTo(queueStartPos.eulerAngles.y);
        while (waiter.CoffeeStackLength > 0)
        {
            yield return _coffeeDropDuration;
            if (dropedCoffees % 4 == 0)
            {
                lastBox = boxPool.Get(stackStartPos.position + (dropedCoffees / 4) * 0.45f * Vector3.up);
                coffeeBoxStack.Push(lastBox.transform);
            }
            lastBox.PutCoffee(waiter.ServeCoffee().GetComponent<Coffee>());
            dropedCoffees++;
            carBubble.SetText((coffeeNeed - dropedCoffees).ToString(), true);
        }

        if (dropedCoffees == coffeeNeed)
        {
            StartCoroutine(SendBoxesToCar());
        }
    }

    public void JoinQueue(Waiter waiter)
    {
        waiterQueue.Enqueue(waiter);
    }

    public void Dequeue()
    {
        waiterQueue.Dequeue();
    }

    public void RemoveFromQueue(Waiter waiter)
    {
        waiterQueue.RemoveImmediate(waiter);
    }

    public void PromiseForCoffee(int count)
    {
        promisedCoffees += count;
    }

    public void CancelCoffeePromise(int count)
    {
        promisedCoffees -= count;
    }

    private IEnumerator SendBoxesToCar()
    {
        cashierAnimator.SetTrigger("Press");
        carBubble.Close();
        while (coffeeBoxStack.Count > 0)
        {
            yield return _coffeeDropDuration;
            CoffeeBox coffeeBox = coffeeBoxStack.Pop().GetComponent<CoffeeBox>();
            carTransform.DOScale(1.2f, 0.1f).SetLoops(2, LoopType.Yoyo);
            coffeeBox.transform.DOMove(carTransform.position, 0.3f).SetEase(Ease.OutSine).OnComplete(() =>
            {
                coffeeBox.Release();
            });
        }

        yield return new WaitForSeconds(1);

        UIMoney.Instance.Add(coffeeNeed, carTransform.position + Vector3.up * 2.5f, true);
        carIsWaitingCoffee = false;
        carAnimator.SetBool("Move", true);
        orderFinished = true;
        coffeeNeed = 0;
        promisedCoffees = 0;
        dropedCoffees = 0;
    }


    #region Locked & Unlock

    public bool Unlocked { get => saveData.Value.DriveThruUnlocked; }

    public void Unlock()
    {
        saveData.Value.DriveThruUnlocked = true;
        lockedImage.SetActive(false);
        smokeEffect.Play();
        mesh.DOScale(1, 1).SetEase(Ease.OutBack).OnComplete(() =>
        {
            CallCar();
        });
    }

    #endregion


    #region Car

    private bool orderFinished = false;
    private bool carIsWaitingCoffee = false;
    private float carDistance = 0;

    private void Update()
    {
        UpdateCarPosition();
    }

    private void CallCar()
    {
        carTransform.gameObject.SetActive(true);
    }

    private void UpdateCarPosition()
    {
        if (carTransform.gameObject.activeSelf && !carIsWaitingCoffee)
        {
            Move();
            if (carDistance > 67 && !carIsWaitingCoffee && !orderFinished)
            {
                cashierAnimator.SetTrigger("Press");
                carAnimator.SetBool("Move", false);
                carIsWaitingCoffee = true;
                coffeeNeed = Random.Range(1, 1 + Mathf.CeilToInt(saveData.Value.SeatCount * maxCoffeeNeedRatioToSeats));
                carBubble.SetText(coffeeNeed.ToString(), true);
                ShopManager.Instance.WaiterManager.ManageWaitersForDriveForDriveThru();
            }
        }
    }

    private void Move()
    {
        carDistance += Time.deltaTime * 10;
        if (carDistance > PathCreator.path.length)
        {
            carDistance = 0;
            orderFinished = false;
            carTransform.gameObject.SetActive(false);
            DOVirtual.DelayedCall(Random.Range(carRespawnDelayRange.x, carRespawnDelayRange.y), () => CallCar());
        }
        carTransform.SetPositionAndRotation(PathCreator.path.GetPointAtDistance(carDistance),
            Quaternion.Lerp(carTransform.rotation, PathCreator.path.GetRotationAtDistance(carDistance), Time.deltaTime * 5));
    }
    #endregion


}


public partial class SaveData
{
    public bool DriveThruUnlocked = false;
}



using DG.Tweening;
using FateGames.Core;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Seat : FateMonoBehaviour
{
    [SerializeField] private StackComponent coffeeStack = null;
    [SerializeField] private Transform mesh = null;
    public Transform InteractionPoint = null;
    public Table Table { get; private set; }
    public Customer Customer { get; private set; }
    public int CoffeeCount { get => coffeeStack.Count; }

    private bool reserved = false;

    private bool dirty = false;

    private void Awake()
    {
        Table = transform.parent.parent.GetComponent<Table>();
    }

    public void Unlock()
    {
        mesh.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
    }

    public bool IsReserved() { return reserved; }

    public bool IsDirty() { return dirty; }

    public void Reserve() { reserved = true; }

    public void Sit(Customer customer)
    {
        Customer = customer;
        Table.CallWaiter();
    }

    public void PutCoffee(Transform coffee)
    {
        coffeeStack.Push(coffee);
        Customer.TakeCoffee();
    }

    public Transform GiveCoffeeToCustomer()
    {
        return coffeeStack.Pop();
    }

    public void LeaveSeat()
    {
        mesh.transform.DOLocalMoveX(Random.Range(-0.5f, 0.5f), 0.5f).SetEase(Ease.OutQuad);
        mesh.transform.DOLocalMoveZ(-Random.Range(0.2f, 0.5f), 0.5f).SetEase(Ease.OutQuad);
        mesh.transform.DOLocalRotate(Vector3.up * Random.Range(30, 80), 0.5f).SetEase(Ease.OutQuad);
        dirty = true;
        Customer = null;
        Table.CheckTableFinish();
    }

    public void CleanSeat()
    {
        mesh.transform.DOLocalMove(Vector3.zero, 0.5f).SetEase(Ease.InQuad);
        mesh.transform.DOLocalRotate(Vector3.zero, 0.5f).SetEase(Ease.InQuad);
        dirty = false;
        Customer = null;
        reserved = false;
    }
}

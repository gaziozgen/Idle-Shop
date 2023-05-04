using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FateGames.Core;

public class Door : FateMonoBehaviour
{
    [SerializeField] private Transform leftDoor = null;
    [SerializeField] private Transform rightDoor = null;

    private bool open = false;
    private List<Collider> persons = new List<Collider>();

    private void OnTriggerEnter(Collider person)
    {
        persons.Add(person);
        if (!open) Open();
    }

    private void OnTriggerExit(Collider person)
    {
        persons.Remove(person);
        if (persons.Count == 0) Close();
    }

    private void Open()
    {
        rightDoor.DOKill();
        leftDoor.DOKill();

        rightDoor.DOLocalRotate(Vector3.up * 90, 0.3f);
        leftDoor.DOLocalRotate(Vector3.up * -90, 0.3f);
    }

    private void Close()
    {
        rightDoor.DOKill();
        leftDoor.DOKill();

        rightDoor.DOLocalRotate(Vector3.zero, 0.3f);
        leftDoor.DOLocalRotate(Vector3.zero, 0.3f);
    }
}

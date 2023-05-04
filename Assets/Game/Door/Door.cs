using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FateGames.Core;
using UnityEngine.InputSystem;

public class Door : FateMonoBehaviour
{
    [SerializeField] private Transform leftDoor = null;
    [SerializeField] private Transform rightDoor = null;
    [SerializeField] private LayerMask mask = 0;

    private bool open = false;
    //private List<Collider> persons = new List<Collider>();

    private void Update()
    {
        int objectCount = Physics.OverlapSphere(transform.position, 1.5f, mask).Length;
        if (objectCount > 0 && !open)
        {
            Open();
        }
        else if (objectCount == 0 && open) 
        {
            Close();
        }
    }
    /*
        private void OnTriggerEnter(Collider person)
        {
            persons.Add(person);
            if (!open) Open();
        }

        private void OnTriggerExit(Collider person)
        {
            persons.Remove(person);
            if (persons.Count == 0) Close();
        }*/

    private void Open()
    {
        open = true;
        rightDoor.DOKill();
        leftDoor.DOKill();

        rightDoor.DOLocalRotate(Vector3.up * 90, 0.3f);
        leftDoor.DOLocalRotate(Vector3.up * -90, 0.3f);
    }

    private void Close()
    {
        open = false;
        rightDoor.DOKill();
        leftDoor.DOKill();

        rightDoor.DOLocalRotate(Vector3.zero, 0.3f);
        leftDoor.DOLocalRotate(Vector3.zero, 0.3f);
    }
}

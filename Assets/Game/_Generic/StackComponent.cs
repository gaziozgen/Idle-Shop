using System.Collections.Generic;
using UnityEngine;
using FateGames.Core;

public class StackComponent : FateMonoBehaviour
{
    [SerializeField] private Transform stackParentAndStart = null;
    [SerializeField] private Vector3 distance = Vector3.zero;
    [SerializeField] private float speed = 10f;
    [SerializeField] private bool additionalLatencyEffectOnMove = false;
    [SerializeField] private float additionalLatencyAmount = 0.2f;

    public int Count { get => list.Count; }

    private List<Transform> list = new List<Transform>();
    private Transform stackStart = null;
    private Vector3 lastPosisiton = Vector3.zero;
    private bool moving = false;

    private void Awake()
    {
        stackStart = stackParentAndStart;
    }

    public void Push(Transform trans)
    {
        list.Add(trans);
    }

    public Transform Pop()
    {
        if (list.Count == 0)
        {
            Debug.LogError("Stack Is Empty", this); 
            return null;
        }
        Transform last = list[list.Count - 1];
        list.Remove(last);
        return last;
    }

    public void UpdateStackStart(Transform trans)
    {
        stackStart = trans;
    }

    private void Update()
    {
        if (additionalLatencyEffectOnMove)
            DetectMoving();
        AdjustItems();
    }

    private void DetectMoving()
    {
        if (transform.position != lastPosisiton) moving = true;
        else if (moving) moving = false;
        lastPosisiton = transform.position;
    }

    private void AdjustItems()
    {
        Vector3 movingEdit = Vector3.zero;
        if (additionalLatencyEffectOnMove && moving) movingEdit = -transform.forward * additionalLatencyAmount;

        for (int i = 0; i < list.Count; i++)
        {
            if (i == 0)
                list[i].position = Vector3.Lerp(list[i].position, stackStart.position, Time.deltaTime * speed);
            else
                list[i].position = Vector3.Lerp(list[i].position, list[i - 1].position + (distance + movingEdit), Time.deltaTime * speed);
        }
    }
}

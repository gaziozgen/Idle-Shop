using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using FateGames;
using FateGames.Core;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class Person : FateMonoBehaviour
{
    protected NavMeshAgent agent;
    public WaitUntil WaitUntilReached;

    public bool Reached { get => agent.enabled && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && !agent.hasPath && agent.velocity.sqrMagnitude == 0f; }

    protected virtual void Awake()
    {
        agent= GetComponent<NavMeshAgent>();
        WaitUntilReached = new(() => Reached);
    }

    public virtual void SetDestination(Vector3 target)
    {
        if (agent.enabled == false)
            Debug.LogError("agent not active!", this);
        agent.SetDestination(target);
    }

    public void TurnTo(float rotataion_y)
    {
        Vector3 rot = transform.eulerAngles;
        rot.y = rotataion_y;
        transform.eulerAngles = rot;
    }

    public void SetAgentEnabled(bool enabled)
    {
        agent.enabled = enabled;
    }

    public void SetAgentPosition(Vector3 pos)
    {
        agent.Warp(pos);
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class RivalAi : MonoBehaviour
{
    public enum TargetSelectionMethod
    {
        None, // Idle, should never be,
        Random,
        OrderLoop, // loops when we get to max lengeth, we return back to zero,
        OrderPing, // reverses when we get to max length, we don't return back to zero
    }

    [SerializeField] private Transform[] targets;
    [SerializeField] private TargetSelectionMethod selectionMethod = TargetSelectionMethod.None;
    [SerializeField] private float nextTargetInterval = 5f; // every x seconds we want to select new target
    private NavMeshAgent _agent;
    private int _inc = 1; // onlu use for order
    private int _currentTargetIndex = 0;
    private Transform CurrentTarget => targets[_currentTargetIndex];
    private float _nextTargetTime;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _nextTargetTime = nextTargetInterval;
    }

    private void NextTarget()
    {
        switch (selectionMethod)
        {
            case TargetSelectionMethod.None: return;
            case TargetSelectionMethod.Random:
                _currentTargetIndex = Random.Range((int)0, targets.Length);
                break;
            case TargetSelectionMethod.OrderLoop:
                _currentTargetIndex += _inc;
                if (_currentTargetIndex >= targets.Length)
                    _currentTargetIndex = 0;
                break;
            case TargetSelectionMethod.OrderPing:
                if (_currentTargetIndex + _inc >= targets.Length || _currentTargetIndex + _inc < 0)
                    _inc *= -1;
                _currentTargetIndex += _inc;
                break;
            default:
                Debug.LogError("SelectionsMethod Is not implemeted", this);
                break;
        }
        _agent.SetDestination(CurrentTarget.position);
    }

    private void Update()
    {
        // Check if we've reached the destination
        // btw i have no idea how these checks work and why, i just stole them
        if (_agent.pathPending) return;
        if (_agent.remainingDistance > _agent.stoppingDistance) return;
        if (_agent.hasPath && _agent.velocity.sqrMagnitude != 0f) return;

        // Done
        _nextTargetTime -= Time.deltaTime;
        if (_nextTargetTime > 0) return;

        _nextTargetTime = nextTargetInterval;
        NextTarget();
    }
}

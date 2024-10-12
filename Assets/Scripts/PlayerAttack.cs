using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(GameplayInput))]
public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float distance = 1.5f;
    [SerializeField] private Transform orgin;
    [SerializeField] private LayerMask attackableLayer;

    private GameplayInput _input;

    private void Start()
    {
        _input = GetComponent<GameplayInput>();
    }

    private void Update()
    {
        if (_input.Attack)
            Attack();

        _input.Attack = false;
    }

    private void Attack()
    {
        // raycast
        // check hit
        // kill enemy
        float actualDistance = distance + Vector2.Distance(orgin.position, transform.position);
        if (!Physics.Raycast(orgin.position, orgin.forward, out var hitInfo, actualDistance, attackableLayer)) return;

        // we have hit somthing
        if (hitInfo.collider.TryGetComponent<NavMeshAgent>(out var agent))
            agent.enabled = false;
        
        if (hitInfo.collider.TryGetComponent<StudentsAI>(out var ai))
            ai.enabled = false;

        hitInfo.collider.AddComponent<Rigidbody>(); // adds gravity to enemy
        hitInfo.collider.GetComponent<Renderer>().material.color = Color.red;
    }

    private void OnDrawGizmosSelected()
    {
        if (orgin != null)
        {
            float actualDistance = distance + Vector2.Distance(orgin.position, transform.position);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(orgin.position, orgin.position + (orgin.forward * actualDistance));
        }
    }
}

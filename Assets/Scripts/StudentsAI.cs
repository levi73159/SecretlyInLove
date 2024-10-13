using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class StudentsAI : MonoBehaviour
{
    private NavMeshAgent agent;
    public string PlayerTag = "Player";
    public Transform[] points;
    public float DetectionRange = 5f;
    [Tooltip("The vision of ai, this will control how far it can see, like a dead body")] public float ViewRange = 5f;
    public LayerMask ViewMask;
    public float ChaseSpeed = 7f;
    public float WanderSpeed = 5f;
    public float PanicSpeed = 10f;

    public bool IsDead { get; private set; }
    public bool IsPanic { get; private set; }

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        SetWanderDestination();
    }

    public void Kill()
    {
        if (IsDead) return; // can't kill twice
        IsDead = true;

        agent.isStopped = true;

        agent.enabled = false;

        GetComponent<Renderer>().material.color = Color.red;
        gameObject.AddComponent<Rigidbody>();
    }

    // not my code so don't ask how oit works
    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {

        Vector3 randomPoint = center + Random.insideUnitSphere * range; //random point in a sphere 
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas)) //documentation: https://docs.unity3d.com/ScriptReference/AI.NavMesh.SamplePosition.html
        {
            //the 1.0f is the max distance from the random point to a point on the navmesh, might want to increase if range is big
            //or add a for loop like in the documentation
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }

    public void StartPanic()
    {
        if (IsPanic) return;
        IsPanic = true;
        agent.ResetPath();
    }

    void Update()
    {
        if (IsDead) return;
        Move();
        Sight();

    }

    // The ai vision
    private void Sight()
    {
        // check if we see any dead bodies
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, ViewRange, transform.forward, ViewRange, ViewMask);
        foreach (var hit in hits)
        {
            if (hit.collider.TryGetComponent<StudentsAI>(out var student))
            {
                if (student.IsDead || student.IsPanic) StartPanic();
            }
        }
    }

    private void Move()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag(PlayerTag);
        GameObject target = null;

        if (players.Length > 0)
        {
            float minDistance = float.MaxValue;
            foreach (GameObject player in players)
            {
                float distance = Vector3.Distance(transform.position, player.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    target = player;
                }
            }
        }

        if (target != null && Vector3.Distance(transform.position, target.transform.position) < DetectionRange)
        {
            Chase(target.transform.position);
        }
        else if (IsPanic)
        {
            Panic();
        }
        else
        {
            Wander();
        }
    }


    public void Panic()
    {
        GetComponent<Renderer>().material.color = Color.blue;
        // Get Random Point once 
        if (agent.pathPending || agent.remainingDistance >= 0.5f) return;

        agent.speed = PanicSpeed;
        Vector3 point;

        // trys 3 times to get a point, if fail try one more time without care of player if that fails move to a waypoint
        for (int trys = 0; trys < 3; trys++)
        {
            if (!RandomPoint(transform.position, 30, out point))
                continue;

            GameObject[] players = GameObject.FindGameObjectsWithTag(PlayerTag);
            foreach (var player in players)
            {
                // Make sure point is going away from player and if not make sure itn ot getting closer
                float pointToPlayer = Vector3.Distance(point, player.transform.position);
                float selfToPlayer = Vector3.Distance(transform.position, player.transform.position);
                if (pointToPlayer < selfToPlayer - 10.0f) continue; // Get new point
            }
            agent.SetDestination(point);
            return;
        }

        if (RandomPoint(transform.position, 30, out point))
        {
            agent.SetDestination(point);
        }
        else
        {
            if (points.Length == 0)
                return;

            int destPoint = Random.Range(0, points.Length);
            agent.SetDestination(points[destPoint].position);
        }
    }


    void Wander()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            SetWanderDestination();
        }
    }

    void SetWanderDestination()
    {
        if (points.Length == 0)
            return;

        int destPoint = Random.Range(0, points.Length);
        agent.destination = points[destPoint].position;
        agent.speed = WanderSpeed;
    }

    void Chase(Vector3 targetPosition)
    {
        agent.destination = targetPosition;
        agent.speed = ChaseSpeed;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, DetectionRange);
    }
}

using UnityEngine;
using UnityEngine.AI;

public class MedEnemyController : MonoBehaviour
{
    public Transform playerTransform;
    public float detectionRadius = 10f;
    public float fieldOfViewAngle = 120f;

    public bool seenByPlayer;

    private NavMeshAgent navMeshAgent;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void EnterVisionCone()
    {
        seenByPlayer = true;
    }

    public void ExitVisionCone()
    {
        seenByPlayer = false;
    }

    private void Update()
    {
        if (seenByPlayer)
        {
            navMeshAgent.SetDestination(playerTransform.position);
        }
        else
        {
            navMeshAgent.SetDestination(transform.position);
        }
    }
}
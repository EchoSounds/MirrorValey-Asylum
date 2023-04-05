using UnityEngine;
using UnityEngine.AI;

public class MedEnemyController : MonoBehaviour
{
    public Transform playerTransform;
    public float detectionRadius = 10f;
    public float fieldOfViewAngle = 120f;

    private NavMeshAgent navMeshAgent;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        // Calculate the angle between the player's forward direction and the direction to the enemy
        Vector3 directionToPlayer = playerTransform.position - transform.position;
        float angleToPlayer = Vector3.Angle(directionToPlayer, playerTransform.forward);

        // If the player is within the detection radius and within the enemy's field of view, move towards the player
        if (directionToPlayer.magnitude < detectionRadius && angleToPlayer < fieldOfViewAngle * 0.5f)
        {
            Debug.Log("see player");
            RaycastHit hitInfo;
            if (Physics.Raycast(transform.position, directionToPlayer.normalized, out hitInfo))
            {
                if (hitInfo.collider.gameObject.tag == "Player")
                {
                    navMeshAgent.SetDestination(playerTransform.position);
                }
            }
        }
        else
        {
            navMeshAgent.ResetPath();
        }
    }
}
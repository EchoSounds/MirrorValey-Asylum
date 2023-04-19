using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionCone : MonoBehaviour
{
    public float viewRadius = 10f;
    [Range(0, 360)]
    public float viewAngle = 45f;
    public LayerMask targetMask;
    public LayerMask obstacleMask;

    private void Update()
    {
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        foreach (Collider targetCollider in targetsInViewRadius)
        {
            if(targetCollider.tag == "MedEnemy")
            {
                Vector3 dirToTarget = (targetCollider.transform.position - transform.position).normalized;

                Debug.Log("InRangeofEnemy");

                if (Vector3.Angle(dirToTarget, transform.forward) < viewAngle / 2)
                {
                    float distToTarget = Vector3.Distance(targetCollider.transform.position, transform.position);

                    Debug.Log("Enemy in vision cone"); 

                    if (!Physics.Raycast(transform.position, dirToTarget, distToTarget, obstacleMask))
                    {
                        // The player can see the enemy within the vision cone
                        Debug.Log("Enemy seen");
                    }
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle / 2, 0) * transform.forward;
        Vector3 rightBoundary = Quaternion.Euler(0, viewAngle / 2, 0) * transform.forward;

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, leftBoundary * viewRadius);
        Gizmos.DrawRay(transform.position, rightBoundary * viewRadius);
    }


}
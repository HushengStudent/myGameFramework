using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class NavigationTest : MonoBehaviour
{

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        RaycastHit hit;
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
                agent.SetDestination(hit.point);
        }
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            NavMeshPath path = agent.path;
            Gizmos.color = Color.red;
            if (path.corners.Length < 2)
            {
                return;
            }
            for (int i = 1; i < path.corners.Length; i++)
            {
                Gizmos.DrawLine(path.corners[i - 1], path.corners[i]);
            }
        }
    }
}

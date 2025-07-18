using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class PlatformFollower : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform platform;
    private bool onPlatform;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void SetPlatform(Transform newPlatform)
    {
        platform = newPlatform;
        if (platform != null)
        {
            onPlatform = true;
            agent.isStopped = true;
            agent.updatePosition = false;
            transform.SetParent(platform);
        }
    }

    public void ClearPlatform()
    {
        if (!onPlatform) return;

        onPlatform = false;
        transform.SetParent(null);
        agent.updatePosition = true;

        NavMeshSurface platformNavMeshSurface = platform.GetComponentInParent<NavMeshSurface>();
        if (platformNavMeshSurface != null)
        {
            platformNavMeshSurface.BuildNavMesh();
        }

        agent.isStopped = false;
        agent.Warp(transform.position);
        platform = null;
    }
}

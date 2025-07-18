using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

[RequireComponent(typeof(NavMeshSurface))]
public class NavMeshSurfaceControl : MonoBehaviour
{
    private NavMeshSurface _navMeshSurface;

    private void Awake()
    {
        _navMeshSurface = GetComponent<NavMeshSurface>();
    }

    public void BuildNavMesh()
    {
        if (_navMeshSurface == null)
        {
            Debug.LogError("NavMeshSurface component not found.");
            return;
        }

        _navMeshSurface.BuildNavMesh();
    }
}

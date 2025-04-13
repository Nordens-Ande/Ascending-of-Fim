using NUnit.Framework;
using System.Runtime.CompilerServices;
using Unity.AI.Navigation;
using UnityEngine;

[RequireComponent(typeof(NavMeshSurface))]
public class NavMeshBuilder : MonoBehaviour
{
    [ContextMenu("BakeNavMesh")]
    void BakeNavMesh()
    {
        NavMeshSurface navMeshSurface;
        navMeshSurface = GetComponent<NavMeshSurface>();
        navMeshSurface.minRegionArea = 2f;
        navMeshSurface.BuildNavMesh();
    }

    //place this script on an empty object in hierarchy and run BakeNavMesh() after generating level
}

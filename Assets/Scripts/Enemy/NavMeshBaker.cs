using Unity.AI.Navigation;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(NavMeshSurface))]
public class NavMeshBaker : MonoBehaviour
{
    NavMeshSurface surface;

    //build the navmesh when a level is generated
    public IEnumerator BakeNavMesh()
    {
        yield return null;
        yield return null;

        surface = GetComponent<NavMeshSurface>();
        surface.BuildNavMesh();
    }
}

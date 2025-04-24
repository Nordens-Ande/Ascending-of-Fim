using Unity.AI.Navigation;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(NavMeshSurface))]
public class NavMeshBaker : MonoBehaviour
{
    //[ContextMenu("BakeNavMesh")]
    NavMeshSurface surface;

    public IEnumerator BakeNavMesh()
    {
        yield return null;
        yield return null;

        surface = GetComponent<NavMeshSurface>();
        surface.BuildNavMesh();
    }
}

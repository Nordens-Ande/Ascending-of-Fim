using Unity.AI.Navigation;
using UnityEngine;

[RequireComponent(typeof(NavMeshSurface))]
public class NavMeshBaker : MonoBehaviour
{
    [ContextMenu("BakeNavMesh")]
    public void BakeNavMesh()
    {
        NavMeshSurface surface;
        surface = GetComponent<NavMeshSurface>();
        surface.BuildNavMesh();
    }
}

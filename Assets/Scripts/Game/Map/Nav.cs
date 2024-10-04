using System.Collections;
using System.Collections.Generic;
using NavMeshPlus.Components;
using UnityEngine;

public class Nav : MonoBehaviour
{
    [SerializeField]
    private NavMeshSurface nav;

    public void buildNavMesh()
    {
        nav.BuildNavMeshAsync();
    }
}

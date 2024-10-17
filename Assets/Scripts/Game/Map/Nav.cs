using System.Collections;
using System.Collections.Generic;
using NavMeshPlus.Components;
using UnityEngine;

public class Nav : MonoBehaviour
{
    [SerializeField]
    private NavMeshSurface nav;
    
    //NavMesh 생성 (비동기)
    public void buildNavMesh()
    {
        nav.BuildNavMeshAsync();
    }
}

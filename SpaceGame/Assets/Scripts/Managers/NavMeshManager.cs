using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class NavMeshManager
{
    static List<PipeRoom> pipes = new List<PipeRoom>(); 
    public static void CalculateNavMesh(NavMeshSurface currentSurface)
    {
        currentSurface.BuildNavMesh();
        NavMeshLink[] links = currentSurface.transform.gameObject.GetComponentsInChildren<NavMeshLink>();

        for(int i = 0; i< links.Length; i++)
        {            
            links[i].UpdateLink();
        }
    }
    public static void AddPipe (PipeRoom pipe)
    {
        pipes.Add(pipe);
    }
    public static void RemovePipe(PipeRoom pipe)
    {
        pipes.Remove(pipe);
    }
    public static void CalculateOffMeshLinks()
    {
        foreach(PipeRoom pipe in pipes)
        {
            pipe.CreateLink();
        }
    }
}

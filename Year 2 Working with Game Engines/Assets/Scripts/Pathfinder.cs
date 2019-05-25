using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    public VoxelChunk voxelChunk;
    GameObject cube;
    bool traversing = false;

    Vector3 startPosition = new Vector3(8, 4, 14);
    Vector3 endPosition = new Vector3(11, 4, 2);
    Vector3 offset = new Vector3(0.5f, 0.5f, 0.5f);

    Dictionary<Vector3, int> disDictionary;

    public delegate void OnTraversal(bool traversing);
    public static event OnTraversal TraversalStarts;
    public static event OnTraversal TraversalEnds;

    private void Start()
    {
        if (voxelChunk != null)
        {
            endPosition = voxelChunk.GetEnd();
            startPosition = voxelChunk.GetStart();
        }
    }

    Stack<Vector3> DepthFirstSearch(Vector3 start, Vector3 end, VoxelChunk vc)
    {
        Stack<Vector3> waypoints = new Stack<Vector3>();

        Dictionary<Vector3, Vector3> visitedParent = new Dictionary<Vector3, Vector3>();
        Stack<Vector3> s = new Stack<Vector3>();
        bool found = false;
        Vector3 current = start;
        s.Push(start);

        while (s.Count > 0 && !found)
        {
            current = s.Pop();

            if (current != end)
            {
                List<Vector3> neighbourList = new List<Vector3>();
                neighbourList.Add(current + new Vector3(1, 0, 0));
                neighbourList.Add(current + new Vector3(-1, 0, 0));
                neighbourList.Add(current + new Vector3(0, 0, 1));
                neighbourList.Add(current + new Vector3(0, 0, -1));

                foreach (Vector3 n in neighbourList)
                {
                    if (n.x >= 0 && n.x < vc.GetChunkSize()
                        && n.z >= 0 && n.z < vc.GetChunkSize())
                    {
                        if (vc.isTraversable(n))
                        {
                            visitedParent[n] = current;
                            s.Push(n);
                        }
                    }
                }
            }
            else
            {
                found = true;
            }
        }
        if (found)
        {
            while (current != start)
            {
                waypoints.Push(current + offset);
                current = visitedParent[current];
            }
            s.Push(start + offset);
        }
        return waypoints;
    }

    Stack<Vector3> BreadthFirstSearch(Vector3 start, Vector3 end, VoxelChunk vc)
    {
        Stack<Vector3> waypoints = new Stack<Vector3>();

        Dictionary<Vector3, Vector3> visitedParent = new Dictionary<Vector3, Vector3>();
        Queue<Vector3> q = new Queue<Vector3>();
        bool found = false;
        Vector3 current = start;

        q.Enqueue(start);

        while (q.Count > 0 && !found)
        {
            current = q.Dequeue();
            if (current != end)
            {
                List<Vector3> neighbourList = new List<Vector3>();
                neighbourList.Add(current + new Vector3(1, 0, 0));
                neighbourList.Add(current + new Vector3(-1, 0, 0));
                neighbourList.Add(current + new Vector3(0, 0, 1));
                neighbourList.Add(current + new Vector3(0, 0, -1));

                foreach (Vector3 n in neighbourList)
                {
                    if (n.x >= 0 && n.x < vc.GetChunkSize()
                        && n.z >= 0 && n.z < vc.GetChunkSize())
                    {
                        if (vc.isTraversable(n))
                        {
                            if (!visitedParent.ContainsKey(n))
                            {
                                visitedParent[n] = current;
                                q.Enqueue(n);
                            }
                        }
                    }
                }
            }
            else
            {
                found = true;
            }
        }
        if (found)
        {
            while (current != start)
            {
                waypoints.Push(current + offset);
                current = visitedParent[current];
            }
            waypoints.Push(start + offset);
        }
        return waypoints;
    }

    Stack<Vector3> Dijkstra(Vector3 start, Vector3 end, VoxelChunk vc)
    {
        Stack<Vector3> waypoints = new Stack<Vector3>();
        List<Vector3> l = new List<Vector3>();
        disDictionary = new Dictionary<Vector3, int>();
        Dictionary<Vector3, Vector3> visitedParent = new Dictionary<Vector3, Vector3>();
        bool found = false;
        Vector3 current = start;
        int newDist;

        disDictionary[start] = 0;

        if (vc.isTraversable(current))
            l.Add(current);

        while (l.Count > 0 && !found)
        {

            int minDistance = disDictionary[l[0]];
            current = l[0];
            for (int i = 0; i < l.Count; i++)
            {
                if (minDistance > disDictionary[l[i]])
                {
                    current = l[i];
                    minDistance = vc.GetDistanceCost(l[i]);
                }
            }

            l.Remove(current);

            if (current != end)
            {
                List<Vector3> neighbourList = new List<Vector3>();
                neighbourList.Add(current + new Vector3(1, 0, 0));
                neighbourList.Add(current + new Vector3(-1, 0, 0));
                neighbourList.Add(current + new Vector3(0, 0, 1));
                neighbourList.Add(current + new Vector3(0, 0, -1));
                foreach (Vector3 n in neighbourList)
                {
                    if (n.x >= 0 && n.x < vc.GetChunkSize() && n.z >= 0 && n.z < vc.GetChunkSize()
                        && vc.isTraversable(n))
                    {
                        newDist = disDictionary[current] + vc.GetDistanceCost(n);

                        if (!visitedParent.ContainsKey(n) || newDist < disDictionary[n])
                        {
                            disDictionary[n] = newDist;
                            visitedParent[n] = current;
                            l.Add(n);
                        }
                    }
                }
            }
            else
            {
                found = true;
            }
        }
        if (found)
        {
            Debug.Log("The path cost is: " + disDictionary[current]);
            while (current != start)
            {
                waypoints.Push(current + offset);
                current = visitedParent[current];
            }
            waypoints.Push(start + offset);
        }
        return waypoints;
    }

    IEnumerator LerpAlongPath(Stack<Vector3> path)
    {
        traversing = true;
        // If we already have a cube destroy it
        if (cube != null)
        {
            DestroyObject(cube);
        }
        // Instantiate a cube to move
        cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        // Pop first waypoint off as the starting point
        Vector3 current = path.Pop();
        cube.transform.position = current;

        while (path.Count > 0)
        {
            Vector3 target = path.Pop();
            float lerpTime = voxelChunk.GetDistanceCost(target);
            float currentTime = 0.0f;
            //Lerp to out next waypoint until time has expired
            while (currentTime < lerpTime)
            {
                currentTime += Time.deltaTime;
                cube.transform.position = Vector3.Lerp(current, target, currentTime / lerpTime);
                yield return 0;
            }
            cube.transform.position = target;
            current = target;
        }
        traversing = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))                                            //FOR TESTING REMOVE IT!!!
            Dijkstra(new Vector3(0, 1, 0), new Vector3(0, 1, 0), voxelChunk);

        if (!traversing)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                // Find path with BFS
                Stack<Vector3> path = Dijkstra(startPosition, endPosition, voxelChunk);//BreadthFirstSearch(startPosition, endPosition, voxelChunk);
                //if we have a path
                if (path.Count > 0)
                {
                    StartCoroutine(LerpAlongPath(path));
                }
                else
                {
                    Debug.Log("no path");
                }
            }

            if (Input.GetKeyDown(KeyCode.F2))
            {
                SetStartEndPos();
            }
        }
    }

    public void SetStartEndPos()
    {
        Debug.Log(voxelChunk.GetStart());
        Vector3 s, e;
        if (voxelChunk.GetStart() != null && voxelChunk.GetEnd() != null)
        {
            startPosition = voxelChunk.GetStart();
            endPosition = voxelChunk.GetEnd();
        }
    }
}

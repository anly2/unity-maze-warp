using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class Node
{
    public Vector3 position;
    public float score;
    public Node parent;

    public Node(Vector3 position)
    {
        this.position = position;
    }

    public List<Node> Expand()
    {
        List<Node> expended = new List<Node>();
        expended.Add(new Node(new Vector3(position.x + 1, position.y, position.z)));
        expended.Add(new Node(new Vector3(position.x - 1, position.y, position.z)));
        expended.Add(new Node(new Vector3(position.x, position.y + 1, position.z)));
        expended.Add(new Node(new Vector3(position.x, position.y - 1, position.z)));
        return expended;
    }
}

public static class PathFinding
{


    public static List<Vector3> FindPath(this Movement self, Vector3 dest)
    {
        List<Vector3> path = new List<Vector3>();

        IDictionary<int, Node> open = new SortedDictionary<int, Node>();

        Node start = new Node(self.transform.position);
        open.Add(0, start);

        while(open.Count > 0)
        {
            int key = open.Keys.First();
            Node curr = open[key];
            open.Remove(key);

            if (curr.position == dest)
            {
                path = RetrievePath(start, curr);
                return path;
            }

            List<Node> expanded = curr.Expand();

            foreach (Node neighbour in expanded)
            {
                if (!self.CanMove(curr.position, neighbour.position))
                    continue;
                int score = GetDistance(neighbour.position, dest);
                neighbour.score = score;
                neighbour.parent = curr;
                if (!open.ContainsKey(score))
                    open.Add(score, neighbour);
            }
        }
        return path;  
    }

    private static List<Vector3> RetrievePath(Node start, Node target)
    {
        List<Vector3> path = new List<Vector3>();
        Node curr = target;
        while (curr != start)
        {
            path.Add(curr.position);
            curr = curr.parent;
        }
        path.Reverse();

        return path;
    }

    private static int  GetDistance(Vector3 from, Vector3 to)
    {
        return (int) (Mathf.Abs(from.x - to.x) + Mathf.Abs(from.y - to.y) + Mathf.Abs(from.z - to.z));
    }

}



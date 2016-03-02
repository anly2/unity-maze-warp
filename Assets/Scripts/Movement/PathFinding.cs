using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Node
{
    public int costSoFar;
    public int costRemain;
    public Vector3 position;
    public Node parent;

    public Node(Vector3 position)
    {
        this.position = position;
        //this.costSoFar = costSoFar;
    }

    public int util
    {
        get{
            return costSoFar + costRemain;
        }
    }

    public List<Node> Expand()
    {
        List<Node> expended = new List<Node>();
        int y = (int) position.y;
        int x = (int) position.x;
        for (int yoffset = -1; yoffset <= 1; yoffset++)
        {
            int newy = y + yoffset;
            for (int xoffset = -1; xoffset <= 1; xoffset++)
            {
                int newx = x + xoffset;
                if (xoffset * yoffset == 0 && !(xoffset == 0 && yoffset == 0))
                    expended.Add(new Node(new Vector3(newx, newy, position.z)));
            }
        }
        return expended;
    }
}


public static class PathFinding
{

    public static IEnumerator<Vector3> FindPath(this Movement self, Vector3 dest)
    {
        List<Vector3> path = new List<Vector3>();

        List<Node> open = new List<Node>();
        List<Node> closed = new List<Node>();

        Node start = new Node(self.transform.position);
        Node target = new Node(dest);
        open.Add(start);

        while (open.Count > 0)
        {
            Node curr = open[0];
            for (int i = 0; i < open.Count; i++)
            {
                if (open[i].util < curr.util || open[i].util == curr.util && open[i].costRemain < curr.costRemain)
                {
                    curr = open[i];
                }
            }
            open.Remove(curr);
            closed.Add(curr);

            if (curr.position == target.position)
            {
                path = RetrievePath(start, curr);
                return path.GetEnumerator();
            }

            List<Node> expanded = curr.Expand();
 
            foreach (Node neighbour in expanded)
            {
                bool canMove = self.CanMove(curr.position, neighbour.position);
                if (!canMove || closed.Contains(neighbour))
                    continue;
                int newPathToNeighbour = curr.costSoFar + GetDistance(curr, neighbour);
                if (!open.Contains(neighbour) || newPathToNeighbour < neighbour.costSoFar)
                {
                    neighbour.costSoFar = newPathToNeighbour;
                    neighbour.costRemain = GetDistance(neighbour, target);
                    neighbour.parent = curr;
                    if (!open.Contains(neighbour))
                        open.Add(neighbour);
                }
            }
        }

        return path.GetEnumerator();
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

    private static int GetDistance(Node node1, Node node2)
    {
        return (int) (node1.position - node2.position).magnitude;
    }

}



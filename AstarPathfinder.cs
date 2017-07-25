using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public static class PathFinder
{
    private static Coord tDest;

    public static List<Coord> GetPathList(Coord start, Coord end)
    {
        List<Coord> pathList = new List<Coord>();

        tDest = end;

        List<Coord> open = new List<Coord>();
        List<Coord> closed = new List<Coord>();

        open.Add(start);

        Coord curNode;
        Coord.PathVars neibPvs;

        while (open.Count > 0)
        {
            int lowId = 0;
            for (var i = 0; i < open.Count; i++)
            {
                int curf;

                curf = open[i].pathVars.f;

                int lowf;
                lowf = open[lowId].pathVars.f;

                if (curf < lowf) lowId = i;
            }

            curNode = open[lowId];

            open.Remove(curNode);
            closed.Add(curNode);

            if (curNode.IsTheSameCoord(end))
            {
                Coord curr = curNode;
                pathList.Clear();

                while (curr.prevCoord != null)
                {
                    pathList.Add(curr);
                    curr = curr.prevCoord;
                }
                pathList.Reverse();
            }

            List<Coord> succesors = curNode.GetSuccessors();

            for (var i = 0; i < succesors.Count; i++)
            {
                Coord neighbor = succesors[i];
               
                if (closed.Where(c => c.IsTheSameCoord(neighbor)).Count() == 0)
                {
                    int gScore = curNode.pathVars.g + 1;
                    bool gScoreIsBest = false;

                    neibPvs = neighbor.pathVars;

                    if (open.Where(c => c.IsTheSameCoord(neighbor)).Count() == 0)
                    {
                        gScoreIsBest = true;
                        neibPvs.h = Heuristic(neighbor);
                        open.Add(neighbor);
                    }
                    else if (gScore < neibPvs.g)
                    {
                        gScoreIsBest = true;
                    }

                    if (gScoreIsBest)
                    {
                        neighbor.prevCoord = curNode;
                        neibPvs.g = gScore;
                        neibPvs.f = neibPvs.g + neibPvs.h;

                    }

                    closed.Add(neighbor);
                }
            }
        }
        return pathList;
    }

    private static int Heuristic(Coord toCheck)
    {

        int dx;
        int dz;

        dx = Math.Abs(toCheck.r - tDest.r);
        dz = Math.Abs(toCheck.c - tDest.c);

        return (dx + dz);
    }
}

public class Coord
{
    public int r;
    public int c;
    public char feature;

    public double distanceFromAllExplored = 0;

    public struct PathVars
    {
        public int f;
        public int g;
        public int h;
    }

    public PathVars pathVars = new PathVars();
    public Coord prevCoord;

    public Coord(int _r, int _c, char _feature)
    {
        r = _r;
        c = _c;
        feature = _feature;

        pathVars.f = 0;
        pathVars.g = 0;
        pathVars.h = 0;
    }

    public void Reset()
    {
        pathVars.f = 0;
        pathVars.g = 0;
        pathVars.h = 0;
        prevCoord = null;
    }

    private Coord[] neighbours = new Coord[4];

    public void SetNeighbours(List<Coord> maze)
    {
        neighbours[0] = maze.Where(x => IsWalkable(x) && x.r == r - 1 && x.c == c).SingleOrDefault(); //Up
        neighbours[1] = maze.Where(x => IsWalkable(x) && x.r == r + 1 && x.c == c).SingleOrDefault(); //Down
        neighbours[2] = maze.Where(x => IsWalkable(x) && x.r == r && x.c == c - 1).SingleOrDefault(); //Left
        neighbours[3] = maze.Where(x => IsWalkable(x) && x.r == r && x.c == c + 1).SingleOrDefault(); //Right
    }

    private bool IsWalkable(Coord toTest)
    {
        return toTest.feature == '.' || toTest.feature == 'C' || toTest.feature == 'T';
    }

    public bool blockC = false;
    public List<Coord> GetSuccessors()
    {
        List<Coord> succList = neighbours.Where(x => x != null).ToList();
        if (blockC)
            succList.RemoveAll(x => x.feature == 'C');

        return succList;
    }

    public int GetUnkwownSuccesorsCount()
    {
        return GetSuccessors().Where(x => x.feature == '?').Count();
    }

    public string GetCommandToGo(Coord kPos)
    {
        if (kPos.r < r)
            return ("DOWN");
        else if (kPos.r > r)
            return ("UP");
        else if (kPos.c < c)
            return ("RIGHT");
        else
            return ("LEFT");

    }
    public bool IsTheSameCoord(Coord toCheck)
    {
        return r == toCheck.r && c == toCheck.c;
    }
}
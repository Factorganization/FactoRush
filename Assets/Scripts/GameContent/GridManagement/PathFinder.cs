using System;
using System.Collections.Generic;
using GameContent.Entities.GridEntities;
using UnityEngine;

namespace GameContent.GridManagement
{
    public class PathFinder
    {
        #region methodes

        public List<Tile> FindPath(Tile startTile, Tile endTile)
        {
            var openList = new List<Tile>();
            var closedList = new List<Tile>();
            
            openList.Add(startTile);

            while (openList.Count > 0)
            {
                openList.Sort(TileComparer);
                var currentTile = openList[0];
                
                openList.Remove(currentTile);
                closedList.Add(currentTile);

                if (currentTile == endTile)
                {
                    return GetFullList(startTile, endTile);
                }
                
                var neighbours = GetNeighboursTiles(currentTile);

                foreach (var n in neighbours)
                {
                    if (n.IsBlocked || closedList.Contains(n) || Mathf.Abs(currentTile.Index.y - n.Index.y) > 1 || Mathf.Abs(currentTile.Index.x - n.Index.x) > 1)
                        continue;

                    n.G = GetManhattanDistance(startTile, n);
                    n.H = GetManhattanDistance(endTile, n);
                    n.PreviousTile = currentTile;
                    
                    if (!openList.Contains(n))
                        openList.Add(n);
                }
            }
            return new List<Tile>();
        }

        private static int GetManhattanDistance(Tile startTile, Tile neighbourTile) => 
            Mathf.Abs(startTile.Index.x - neighbourTile.Index.x) + Mathf.Abs(startTile.Index.y - neighbourTile.Index.y);

        private static List<Tile> GetFullList(Tile startTile, Tile endTile)
        {
            var f = new List<Tile>();
            var c = endTile;

            while (c != startTile)
            {
                f.Add(c);
                c = c.PreviousTile;
            }

            f.Add(startTile);
            f.Reverse();
            return f;
        }
        
        private static List<Tile> GetNeighboursTiles(Tile currentTile)
        {
            var g = GridManager.Manager.Grid;
            var n = new List<Tile>();
            
            var topCheck = new Vector2Int(currentTile.Index.x, currentTile.Index.y + 1);
            if (g.TryGetValue(topCheck, out var t))
                n.Add(t);
            
            var leftCheck = new Vector2Int(currentTile.Index.x - 1, currentTile.Index.y);
            if (g.TryGetValue(leftCheck, out var l))
                n.Add(l);
            
            var rightCheck = new Vector2Int(currentTile.Index.x + 1, currentTile.Index.y);
            if (g.TryGetValue(rightCheck, out var r))
                n.Add(r);
            
            var bottomCheck = new Vector2Int(currentTile.Index.x, currentTile.Index.y - 1);
            if (g.TryGetValue(bottomCheck, out var b))
                n.Add(b);
            
            return n;
        }

        #endregion
        
        #region fields
        
        private static readonly Comparison<Tile> TileComparer = (a, b) => MathF.Sign(a.F - b.F);
        
        #endregion
    }
}
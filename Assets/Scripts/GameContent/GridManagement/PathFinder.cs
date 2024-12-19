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
                    //things
                }
                
                var neighbours = GetNeighboursTiles(currentTile);
            }
            return closedList;
        }

        private Tile GetNeighboursTiles(Tile currentTile)
        {
            var g = GridManager.Manager.Grid;
            var neighbours = new List<Tile>();
            
            var topCheck = new Vector2Int(currentTile.Index.x, currentTile.Index.y + 1);
            var leftCheck = new Vector2Int(currentTile.Index.x - 1, currentTile.Index.y + 1);
            var rightCheck = new Vector2Int(currentTile.Index.x + 1, currentTile.Index.y + 1);
            var bottomCheck = new Vector2Int(currentTile.Index.x, currentTile.Index.y - 1);
            return null;
        }

        #endregion
        
        #region fields
        
        private static readonly Comparison<Tile> TileComparer = (a, b) => MathF.Sign(b.F - a.F);
        
        #endregion
    }
}
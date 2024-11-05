using GameContent.Entities.OnFieldEntities;
using GameContent.GridManagement;
using UnityEngine;

namespace GameContent.Entities.GridEntities
{
    public abstract class Tile : Entity
    {
        #region properties

        public Building CurrentOnTopBuilding { get; set; }
        
        public Vector2Int Index { get; private set; }

        public GridManager Grid { get; private set; }
        
        #endregion
        
        #region methodes
        
        public void Added(GridManager grid, Vector2Int index, Vector3 pos = new())
        {
            Index = index;
            Grid = grid;
            Position = pos;
        }
        
        #endregion
    }
}
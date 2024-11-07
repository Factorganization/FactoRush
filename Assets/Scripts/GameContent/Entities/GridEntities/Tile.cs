#define INDEX_DEBUG

using GameContent.Entities.OnFieldEntities;
using GameContent.GridManagement;
using TMPro;
using UnityEngine;

namespace GameContent.Entities.GridEntities
{
    public abstract class Tile : Entity
    {
        #region properties

        public GridManager Grid { get; private set; }

        public Building CurrentBuildingRef { get; set; }
        
        public Vector2Int Index { get; private set; }
        
        public TileType Type { get; private set; }
        
        #endregion
        
        #region methodes
        
        public void Added(GridManager grid, Vector2Int index, Vector3 pos = new(), TileType type = TileType.Default)
        {
            Index = index;
            Grid = grid;
            Position = pos;
            Type = type;
            
#if INDEX_DEBUG && UNITY_EDITOR
            debugIndex.text = $"<size=1>{Type}</size> \n <size=3>({Index.x},{Index.y})</size>";
#endif
        }
        
        #endregion
        
        #region fields
        
#if INDEX_DEBUG && UNITY_EDITOR
        [SerializeField] private TMP_Text debugIndex;
#endif
        
        #endregion
    }
}
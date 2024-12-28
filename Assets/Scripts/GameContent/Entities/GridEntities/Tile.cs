#define INDEX_DEBUG

using GameContent.CraftResources;
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

        protected virtual ConveyorGroup GroupRef { get; set; }
        
        protected bool Active { get; set; }
        
        #region Path Find
        
        public abstract bool IsBlocked { get; }
        
        public int G { get; set; }
        
        public int H { get; set; }
        
        public int F => G + H;
        
        public Tile PreviousTile { get; set; }

        public abstract bool IsSelected { get; set; }
            
        #endregion
            
        #endregion
        
        #region methodes
        
        public void Added(GridManager grid, Vector2Int index, Vector3 pos = new(), TileType type = TileType.Default)
        {
            Index = index;
            Grid = grid;
            Position = pos;
            Type = type;
            Active = false;
            
#if INDEX_DEBUG && UNITY_EDITOR
            debugIndex.text = $"<size=1>{Type}</size> \n <size=3>({Index.x},{Index.y})</size>";
#endif
        }

        public virtual void SetConveyorGroup(ConveyorGroup conveyorGroup) => GroupRef = conveyorGroup;
        
        public void MarkActive(bool active) => Active = active;

        protected virtual void InstantiateResource(BaseResource resource)
        {
            GroupRef?.AddResource(resource);
        }

        protected virtual void DestroyResource(BaseResource resource)
        {
            GroupRef?.RemoveResource(resource);
        }
        
        #endregion
        
        #region fields
        
#if INDEX_DEBUG && UNITY_EDITOR
        [SerializeField] private TMP_Text debugIndex;
#endif

        #endregion
    }
}
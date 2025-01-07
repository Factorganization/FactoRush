//#define INDEX_DEBUG

using System.Collections.Generic;
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

        protected virtual List<ConveyorGroup> GroupRef { get; set; }
        
        protected bool Active { get; private set; }
        
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
            GroupRef = new List<ConveyorGroup>();
            Index = index;
            Grid = grid;
            Position = pos;
            Type = type;
            Active = false;
            
#if INDEX_DEBUG && UNITY_EDITOR
            debugIndex.text = $"<size=1>{Type}</size> \n <size=3>({Index.x},{Index.y})</size>";
#endif
        }

        public virtual void AddConveyorGroup(ConveyorGroup conveyorGroup) => GroupRef.Add(conveyorGroup);
        
        public virtual void RemoveConveyorGroup(ConveyorGroup conveyorGroup) => GroupRef.Remove(conveyorGroup);
        
        public void MarkActive(bool active) => Active = active;

        protected virtual void InstantiateResourceAt(int conveyorIndex, BaseResource resource, Vector3 pos, int pathIndex)
        {
            var r = Instantiate(resource, pos, Quaternion.identity);
            r.Created(GroupRef[conveyorIndex]);
            GroupRef[conveyorIndex]?.AddResource(r);
        }

        protected virtual void DestroyResource(int conveyorIndex, BaseResource resource)
        {
            GroupRef[conveyorIndex]?.RemoveResource(resource);
        }
        
        #endregion
        
        #region fields
        
#if INDEX_DEBUG && UNITY_EDITOR
        [SerializeField] private TMP_Text debugIndex;
#endif

        #endregion
    }
}
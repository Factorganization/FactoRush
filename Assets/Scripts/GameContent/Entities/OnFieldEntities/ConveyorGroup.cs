using System;
using System.Collections.Generic;
using GameContent.CraftResources;
using GameContent.Entities.GridEntities;
using GameContent.GridManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameContent.Entities.OnFieldEntities
{
    public class ConveyorGroup : DynamicBuildingList
    {
        #region properties
        
        public sbyte ConveyorGroupId { get; set; }
        
        public Tile FromStaticTile { get; set; }
        
        public Tile ToStaticTile { get; set; }

        #endregion

        #region constructor

        public ConveyorGroup(sbyte id, params DynamicBuilding[] dl) : base(dl)
        {
            _conveyedResources = new List<BaseResource>();
            ConveyorGroupId = id;
        }

        #endregion

        #region methodes

        public void Init()
        {
            // Ca marche mais j'ai oublié ou est le code qui fait que ca marche
            if ((this[0].TileRef is SideStaticBuildingTile && this[Count - 1].TileRef is MineTile) ||
                (this[0].TileRef is WeaponTargetTile or TransTargetTile && this[Count - 1].TileRef is SideStaticBuildingTile))
                DynamicBuildings.Reverse();

            FromStaticTile = this[0].TileRef;
            ToStaticTile = this[Count - 1].TileRef;
            
            FromStaticTile.AddConveyorGroup(this);
            ToStaticTile.AddConveyorGroup(this);
            FromStaticTile.MarkActive(true);
            ToStaticTile.MarkActive(true);
            
            if (!CheckValidity() || !CheckPathJump())
                return;
            
            GraphInit();
        }

        private void GraphInit()
        {
            for (var i = 0; i < Count; i++)
            {
                if (i >= Count - 1)
                    continue;
                
                var r = DynamicBuilding.GetRotation(this[i + 1].TileRef.Index - this[i].TileRef.Index);
                this[i].SetGraph(this[i].Position, Quaternion.Euler(0, r, 0));
            }

            if (this[Count - 1].TileRef is not SideStaticBuildingTile t)
                return;

            var s = GridManager.Manager.StaticGroups[t.StaticGroup];
            var r2 = DynamicBuilding.GetRotation(s[2].Index - this[Count - 1].TileRef.Index);
            this[Count - 1].SetGraph(this[Count - 1].Position, Quaternion.Euler(0, r2, 0));
        }
        
        private bool CheckPathJump()
        {
            var j = 0;
            
            for (var i = 0; i < Count - 1; i++)
            {
                if (Vector2Int.Distance(this[i].TileRef.Index, this[i + 1].TileRef.Index) < 1.1f)
                    j++;
            }
            
            if (j >= Count - 1)
                return true;
            
            foreach (var b in this)
            {
                GridManager.Manager.Grid[b.TileRef.Index].CurrentBuildingRef = null;
                Object.Destroy(b.gameObject);
            }
            GridManager.Manager.ConveyorGroups.Remove(ConveyorGroupId);
            UnsetSelf();
            return false;
        }
        
        private bool CheckValidity()
        {
            if (FromStaticTile is not DynamicBuildingTile && ToStaticTile is not DynamicBuildingTile && Count > 1)
                return true;

            foreach (var b in this)
            {
                GridManager.Manager.Grid[b.TileRef.Index].CurrentBuildingRef = null;
                Object.Destroy(b.gameObject);
            }
            GridManager.Manager.ConveyorGroups.Remove(ConveyorGroupId);
            UnsetSelf();
            return false;
        }
        
        public override void UpdateGroup()
        {
            
        }
        
        public void AddResource(BaseResource resource) => _conveyedResources.Add(resource);
        
        public void RemoveResource(BaseResource resource) => _conveyedResources.Remove(resource);
        
        public void DestroyGroup()
        {
            FromStaticTile.MarkActive(false);
            ToStaticTile.MarkActive(false);

            if (_conveyedResources.Count > 0)
            {
                foreach (var r in _conveyedResources)
                {
                    Object.Destroy(r.gameObject);
                }
                _conveyedResources.Clear();
            }
            
            FromStaticTile.RemoveConveyorGroup(this);
            ToStaticTile.RemoveConveyorGroup(this);
        }

        private void UnsetSelf()
        {
            FromStaticTile.MarkActive(false);
            ToStaticTile.MarkActive(false);
            FromStaticTile.RemoveConveyorGroup(this);
            ToStaticTile.RemoveConveyorGroup(this);
        }

        #endregion

        #region fields
        
        private readonly List<BaseResource> _conveyedResources;

        //C'est pas fou en fait
        private static readonly Comparison<DynamicBuilding> TypeComparer = (a, b) => (int)Mathf.Sign((byte)a.TileRef.Type - (byte)b.TileRef.Type);

        #endregion
    }
}
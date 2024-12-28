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
        
        public ConveyorGroup(params DynamicBuilding[] dl) : base(dl)
        {
            _conveyedResources = new List<BaseResource>();
        }
        
        #endregion
        
        #region methodes

        public void Init()
        {
            DynamicBuildings.Sort(TypeComparer);
            
            switch (this[0].TileRef)
            {
                // Ca marche mais j'ai oublié ou est le code qui fait que ca marche
                case SideStaticBuildingTile when this[Count - 1].TileRef is MineTile:
                    FromStaticTile = this[Count - 1].TileRef;
                    ToStaticTile = this[0].TileRef;
                    
                    this[Count - 1].SetDebugId(100);
                    this[0].SetDebugId(101);
                    break;
                
                case SideStaticBuildingTile when this[Count - 1].TileRef is WeaponTargetTile or TransTargetTile:
                    FromStaticTile = this[0].TileRef;
                    ToStaticTile = this[Count - 1].TileRef;
                    
                    this[Count - 1].SetDebugId(101);
                    this[0].SetDebugId(100);
                    break;
            }
            
            FromStaticTile.SetConveyorGroup(this);
            ToStaticTile.SetConveyorGroup(this);
            FromStaticTile.MarkActive(true);
            ToStaticTile.MarkActive(true);
            CheckValidity();
        }
        
        private void CheckValidity()
        {
            if (FromStaticTile is not DynamicBuildingTile && ToStaticTile is not DynamicBuildingTile)
                return;
            
            foreach (var b in this)
            {
                GridManager.Manager.Grid[b.TileRef.Index].CurrentBuildingRef = null;
                Object.Destroy(b.gameObject);
            }
            GridManager.Manager.ConveyorGroups.Remove(ConveyorGroupId);
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
            
            if (_conveyedResources.Count <= 0)
                return;

            foreach (var r in _conveyedResources)
            {
                Object.Destroy(r.gameObject);
            }
            _conveyedResources.Clear();
            
            FromStaticTile.SetConveyorGroup(null);
            ToStaticTile.SetConveyorGroup(null);
        }
        
        #endregion

        #region fields

        private readonly List<BaseResource> _conveyedResources;
        
        private static readonly Comparison<DynamicBuilding> TypeComparer = (a, b) => (int)Mathf.Sign((byte)a.TileRef.Type - (byte)b.TileRef.Type);
        
        #endregion
    }
}
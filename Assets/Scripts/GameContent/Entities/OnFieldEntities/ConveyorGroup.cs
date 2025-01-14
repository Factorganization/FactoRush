using System;
using System.Collections.Generic;
using GameContent.CraftResources;
using GameContent.Entities.GridEntities;
using GameContent.Entities.OnFieldEntities.Buildings;
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
            _tempPath = new List<DynamicBuilding>();
            _conveyedResources = new List<BaseResource>();
            ConveyorGroupId = id;
        }

        #endregion

        #region methodes

        public void Init()
        {
            // Ca marche mais j'ai oublié ou est le code qui fait que ca marche
            if ((this[0].TileRef is CenterStaticBuildingTile && this[Count - 1].TileRef is MineTile) ||
                (this[0].TileRef is WeaponTargetTile or TransTargetTile && this[Count - 1].TileRef is CenterStaticBuildingTile))
                DynamicBuildings.Reverse();

            FromStaticTile = this[0].TileRef;
            ToStaticTile = this[Count - 1].TileRef;
            
            if (FromStaticTile is CenterStaticBuildingTile s && ToStaticTile is UnitAssemblyTile u)
            {
                var c = GridManager.Manager.StaticGroups[s.StaticGroup].CenterRef;
                
                if (c.CurrentBuildingRef is FactoryBuilding f)
                {
                    var t = f.UnitResourceType;
                    if (u.BinTileGroupRef.TargetType != -1 && u.BinTileGroupRef.TargetType == t)
                    {
                        foreach (var b in this)
                        {
                            if (b.TileRef is CenterStaticBuildingTile c2)
                                c2.SecondaryBuildRefs.Remove(b);
                            else
                                b.TileRef.CurrentBuildingRef = null;
                            Object.Destroy(b.gameObject);
                        }
                        GridManager.Manager.ConveyorGroups.Remove(ConveyorGroupId);
                        return;
                    }
                
                    u.TargetType = f.UnitResourceType;
                    if (u.TargetType != -1)
                        u.BinTileGroupRef.TargetType = f.UnitResourceType == 0 ? 1 : 0;
                }
                
                if (c.GroupRef.Count >= 1)
                {
                    var g = c.GroupRef;
                    foreach (var cg in g)
                    {
                        if (cg.ToStaticTile is UnitAssemblyTile u2 && u2.TargetType != -1 && u2.TargetType != u.TargetType)
                        {
                            foreach (var b in this)
                            {
                                if (b.TileRef is CenterStaticBuildingTile c2)
                                    c2.SecondaryBuildRefs.Remove(b);
                                else
                                    b.TileRef.CurrentBuildingRef = null;
                                Object.Destroy(b.gameObject);
                            }
                            GridManager.Manager.ConveyorGroups.Remove(ConveyorGroupId);
                            return;
                        }
                    }
                }
            }
            
            FromStaticTile.AddConveyorGroup(this);
            if (ToStaticTile is not CenterStaticBuildingTile)
                ToStaticTile.AddConveyorGroup(this);
            
            FromStaticTile.MarkActive(true);
            ToStaticTile.MarkActive(true);
            
            if (!CheckValidity() || !CheckPathJump())
                return;
            
            GraphInit();
            //TrySetRefineryRef();
        }

        public void InitFromDynamic(sbyte index)
        {
            var c = GridManager.Manager.ConveyorGroups[index];
            var f = c[0];
            
            if (!(f.TileRef is MineTile && 
                  this[Count - 1].TileRef is CenterStaticBuildingTile) &&
                !(f.TileRef is CenterStaticBuildingTile &&
                  this[Count - 1].TileRef is TransTargetTile or WeaponTargetTile))
            {
                this[0].RemoveConveyorGroupId(ConveyorGroupId);
                
                for (var j = 1; j < Count; j++)
                {
                    var b = this[j];
                    if (b.TileRef is CenterStaticBuildingTile c2)
                        c2.SecondaryBuildRefs.Remove(b);
                    else
                        b.TileRef.CurrentBuildingRef = null;
                    Object.Destroy(b.gameObject);
                }

                GridManager.Manager.ConveyorGroups.Remove(ConveyorGroupId);
                foreach (var d in f)
                {
                    GridManager.Manager.ConveyorGroups[d].UpdateGroup();
                }
                return;
            }
            
            if (this[Count - 1].TileRef is UnitAssemblyTile u)
            {
                var ct = c.FromStaticTile as CenterStaticBuildingTile;
                
                foreach (var cg in ct!.GroupRef)
                {
                    var u2 = cg.ToStaticTile as UnitAssemblyTile;
                    
                    if ((u2!.TargetType != -1 && u.TargetType != -1 && u2.TargetType != u.TargetType) || u2.AssemblyId == u.AssemblyId)
                    {
                        this[0].RemoveConveyorGroupId(ConveyorGroupId);
                                    
                        for (var j = 1; j < Count; j++)
                        {
                            var b = this[j];
                            if (b.TileRef is CenterStaticBuildingTile c2)
                                c2.SecondaryBuildRefs.Remove(b);
                            else
                                b.TileRef.CurrentBuildingRef = null;
                            Object.Destroy(b.gameObject);
                        }
                    
                        GridManager.Manager.ConveyorGroups.Remove(ConveyorGroupId);
                        foreach (var d in f)
                        {
                            GridManager.Manager.ConveyorGroups[d].UpdateGroup();
                        }
                        return;
                    }
                    
                    u.TargetType = u2.TargetType;
                    if (u.TargetType != -1)
                        u.BinTileGroupRef.TargetType = u2.TargetType == 0 ? 1 : 0;
                }
            }
            
            foreach (var t in c)
            {
                if (t.TileRef.Index != this[0].TileRef.Index)
                {
                    _tempPath.Add(t);
                    t.AddConveyorGroupId(ConveyorGroupId);
                    t.SetDebugId(); //ConveyorGroupId
                }

                else
                    break;
            }
            
            GraphInit();
            
            DynamicBuildings.InsertRange(0, _tempPath);
            
            FromStaticTile = this[0].TileRef;
            ToStaticTile = this[Count - 1].TileRef;
            
            FromStaticTile.AddConveyorGroup(this);
            if (ToStaticTile is not CenterStaticBuildingTile)
                ToStaticTile.AddConveyorGroup(this);
            
            FromStaticTile.MarkActive(true);
            ToStaticTile.MarkActive(true);
            
            //TrySetRefineryRef();
            
            _tempPath.Clear();
        }
        
        private void GraphInit()
        {
            for (var i = 0; i < Count - 1; i++)
            {
                var r = DynamicBuilding.GetRotation(this[i + 1].TileRef.Index - this[i].TileRef.Index);
                this[i].SetGraph(this[i].Position, Quaternion.Euler(0, r, 0));
            }
            
            this[0].gameObject.SetActive(false);
            this[Count - 1].gameObject.SetActive(false);
            
            if (this[Count - 1].TileRef is not CenterStaticBuildingTile t)
                return;

            var s = GridManager.Manager.StaticGroups[t.StaticGroup];
            var r2 = DynamicBuilding.GetRotation(s[0].Index - this[Count - 1].TileRef.Index);
            this[Count - 1].SetGraph(this[Count - 1].Position, Quaternion.Euler(0, r2, 0), false);
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
                if (b.TileRef is CenterStaticBuildingTile c)
                    c.SecondaryBuildRefs.Remove(b);
                else
                    b.TileRef.CurrentBuildingRef = null;
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
                if (b.TileRef is CenterStaticBuildingTile c)
                    c.SecondaryBuildRefs.Remove(b);
                else
                    b.TileRef.CurrentBuildingRef = null;
                Object.Destroy(b.gameObject);
            }
            GridManager.Manager.ConveyorGroups.Remove(ConveyorGroupId);
            UnsetSelf();
            return false;
        }
        
        [Obsolete]
        private void TrySetRefineryRef()
        {
            if (FromStaticTile is not StaticBuildingTile s)
                return;
            
            var c = GridManager.Manager.StaticGroups[s.StaticGroup].CenterRef;
            c.AddConveyorGroup(this);
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

            if (FromStaticTile is CenterStaticBuildingTile s)
            {
                var g = s.StaticGroup;
                GridManager.Manager.StaticGroups[g].CenterRef.RemoveConveyorGroup(this);
                GridManager.Manager.StaticGroups[g].CenterRef.SecondaryBuildRefs.Remove(this[0]);
                if (ToStaticTile is UnitAssemblyTile u && u.BinTileGroupRef.GroupRef.Count <= 0)
                {
                    u.TargetType = -1;
                    u.BinTileGroupRef.TargetType = -1;
                }
            }
            
            if (ToStaticTile is CenterStaticBuildingTile s2)
            {
                var g = s2.StaticGroup;
                GridManager.Manager.StaticGroups[g].CenterRef.SecondaryBuildRefs.Remove(this[Count - 1]);
            }
            
            FromStaticTile.RemoveConveyorGroup(this);
            ToStaticTile.RemoveConveyorGroup(this);
            
            if (FromStaticTile.GroupRef.Count > 0)
                FromStaticTile.MarkActive(true);
        }

        public override void UpdateGroup()
        {
            foreach (var d in DynamicBuildings)
            {
                d.UpdateBuild();
            }
        }

        public bool CheckPathTarget(CenterStaticBuildingTile c, FactoryBuilding f)
        {
            if (c.GroupRef.Count <= 0)
                return true;
            
            foreach (var g in c.GroupRef)
            {
                var u = g.ToStaticTile as UnitAssemblyTile;
                if (u!.TargetType != -1 && u.TargetType != f.UnitResourceType)
                    return false;
            }

            return true;
        }
        
        public void MarkActive(bool active)
        {
            ToStaticTile.MarkActive(active);
            FromStaticTile.MarkActive(active);
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

        private readonly List<DynamicBuilding> _tempPath;
        
        private readonly List<BaseResource> _conveyedResources;

        //C'est pas fou en fait
        private static readonly Comparison<DynamicBuilding> typeComparer = (a, b) => (int)Mathf.Sign((byte)a.TileRef.Type - (byte)b.TileRef.Type);

        #endregion
    }
}

//Le code plus lourd que ma mere
//if (b.TileRef is CenterStaticBuildingTile c2)
//    c2.SecondaryBuildRefs.Remove(b);
//else
//    b.TileRef.CurrentBuildingRef = null;

using System;
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
        
        public Tile FromStaticBuild { get; set; }
        
        public Tile ToStaticBuild { get; set; }
        
        #endregion
        
        #region constructor
        
        public ConveyorGroup(params DynamicBuilding[] dl) : base(dl)
        {
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
                    FromStaticBuild = this[Count - 1].TileRef;
                    ToStaticBuild = this[0].TileRef;
                    this[Count - 1].SetDebugId(101);
                    this[0].SetDebugId(100);
                    break;
                
                case SideStaticBuildingTile when this[Count - 1].TileRef is WeaponTargetTile || this[Count - 1].TileRef is TransTargetTile:
                case MineTile:
                case WeaponTargetTile or TransTargetTile:
                    FromStaticBuild = this[0].TileRef;
                    ToStaticBuild = this[Count - 1].TileRef;
                    this[Count - 1].SetDebugId(101);
                    this[0].SetDebugId(100);
                    break;
            }
            
            CheckValidity();
        }
        
        private void CheckValidity()
        {
            if (FromStaticBuild is not DynamicBuildingTile && ToStaticBuild is not DynamicBuildingTile)
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

        private void OnMove()
        {
            
        }

        private void SwitchWayPoint()
        {
            
        }
        
        #endregion

        #region fields

        private static readonly Comparison<DynamicBuilding> TypeComparer = (a, b) => (int)Mathf.Sign((byte)a.TileRef.Type - (byte)b.TileRef.Type);

        #endregion
    }
}
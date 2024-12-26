using GameContent.Entities.GridEntities;
using GameContent.GridManagement;

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
            
            this[0].SetDebugId(100);
            this[Count - 1].SetDebugId(101);
            GridManager.Manager.TryRemoveDynamicBuildingAt(this[0].TileRef.Index);
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
    }
}
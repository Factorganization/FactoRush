using System.Collections.Generic;
using GameContent.Entities.GridEntities;
using GameContent.Entities.OnFieldEntities;
using GameContent.GridManagement;

namespace GameContent.CraftResources
{
    public class MiningResource : BaseResource
    {
        #region properties

        private SideStaticBuildingTile SideTileRef { get; set; }

        #endregion
        
        #region methodes

        public override void Created(ConveyorGroup conveyorRef, int pathIndex)
        {
            base.Created(conveyorRef, pathIndex);
            
            SideTileRef = conveyorRef.ToStaticTile[pathIndex] as SideStaticBuildingTile;
        }

        protected override void RemoveSelf()
        {
            GridManager.Manager.FactoryRefs[SideTileRef.StaticGroup]?.ResourceAdded(resourceType);

            base.RemoveSelf();
        }

        #endregion
        
        #region fields
        
        public MiningResourceType resourceType;
        
        #endregion
    }
}
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

        public override void Created(ConveyorGroup conveyorRef)
        {
            base.Created(conveyorRef);
            
            SideTileRef = conveyorRef[^1].TileRef as SideStaticBuildingTile;
        }

        protected override void RemoveSelf()
        {
            //GridManager.Manager.
            
            base.RemoveSelf();
        }

        #endregion
        
        #region fields
        
        public MiningResourceType resourceType;
        
        #endregion
    }
}
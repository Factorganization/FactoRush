using GameContent.CraftResources;

namespace GameContent.Entities.GridEntities
{
    public sealed class CenterStaticBuildingTile : StaticBuildingTile
    {
        #region properties

        public override bool IsBlocked => true;

        public override bool IsSelected
        {
            get;
            set;
        }

        #endregion
        
        #region methodes
        
        protected override void DestroyResource(int conveyorIndex, BaseResource resource) {}
        
        #endregion
    }
}
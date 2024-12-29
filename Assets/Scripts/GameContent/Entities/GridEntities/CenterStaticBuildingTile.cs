using UnityEngine;
using GameContent.CraftResources;
using GameContent.Entities.OnFieldEntities;

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

        protected override ConveyorGroup GroupRef
        {
            get => null;
            set { }
        }

        #endregion
        
        #region methodes
        
        public override void SetConveyorGroup(ConveyorGroup conveyorGroup) {}

        protected override void InstantiateResourceAt(BaseResource resource, Vector3 pos) {}
        
        protected override void DestroyResource(BaseResource resource) {}
        
        #endregion
    }
}
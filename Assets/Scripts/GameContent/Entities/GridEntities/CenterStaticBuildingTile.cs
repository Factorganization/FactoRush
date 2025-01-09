using System.Collections.Generic;
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

        public override List<ConveyorGroup> GroupRef
        {
            get => null;
            set { }
        }

        #endregion
        
        #region methodes
        
        public override void AddConveyorGroup(ConveyorGroup conveyorGroup) {}

        public override void RemoveConveyorGroup(ConveyorGroup conveyorGroup) {}
        
        protected override void InstantiateResourceAt(int conveyorIndex, BaseResource resource, Vector3 pos, int pathIndex) {}
        
        protected override void DestroyResource(int conveyorIndex, BaseResource resource) {}
        
        #endregion
    }
}
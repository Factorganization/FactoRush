using System.Collections.Generic;
using GameContent.CraftResources;
using GameContent.Entities.OnFieldEntities;
using GameContent.Entities.OnFieldEntities.Buildings;
using GameContent.GridManagement;

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

        public override Building CurrentBuildingRef
        {
            get => _currentBuildingRef;
            set
            {
                _currentBuildingRef = value;
                switch (_currentBuildingRef)
                {
                    case null:
                        return;
                    case DynamicBuilding d:
                        SecondaryBuildRefs.Add(d);
                        _currentBuildingRef = null;
                        return;
                    default:
                        CheckSelfSendingType();
                        break;
                }
            }
        }
        
        public List<Building> SecondaryBuildRefs { get; } = new();
        
        #endregion
        
        #region methodes
        
        protected override void DestroyResource(int conveyorIndex, BaseResource resource) {}

        private void CheckSelfSendingType()
        {
            if (_currentBuildingRef is not FactoryBuilding f)
                return;

            if (GroupRef.Count <= 0)
                return;
            
            if (!GroupRef[0].CheckPathTarget(f.UnitResourceType))
                    GridManager.Manager.TryRemoveDynamicBuildingAt(GroupRef[0].FromStaticTile.Index);
        }

        public override void RemoveConveyorGroup(ConveyorGroup conveyorGroup)
        {
            base.RemoveConveyorGroup(conveyorGroup);

            if (_currentBuildingRef is FactoryBuilding f)
                f.SetTargetIndex(0);
        }
        
        #endregion
        
        #region fields
        
        private Building _currentBuildingRef;
        
        #endregion
    }
}
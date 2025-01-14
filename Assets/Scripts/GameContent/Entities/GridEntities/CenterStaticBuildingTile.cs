using System.Collections.Generic;
using GameContent.CraftResources;
using GameContent.Entities.OnFieldEntities;
using GameContent.Entities.OnFieldEntities.Buildings;
using GameContent.GridManagement;
using UnityEngine;

namespace GameContent.Entities.GridEntities
{
    public sealed class CenterStaticBuildingTile : StaticBuildingTile
    {
        #region properties

        public override bool IsBlocked => CurrentBuildingRef is not null;

        public bool IsFull => SecondaryBuildRefs.Count >= 4;
        
        public override bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                preSelectionAvailable.SetActive(value);
            }
        }

        public override Building CurrentBuildingRef
        {
            get => _currentBuildingRef;
            set
            {
                switch (value)
                {
                    case null:
                        _currentBuildingRef = null;
                        break;
                    case DynamicBuilding d:
                        SecondaryBuildRefs.Add(d);
                        break;
                    default:
                        _currentBuildingRef = value;
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
                    GridManager.Manager.TryRemoveDynamicBuildingAt(GroupRef[0][1].TileRef.Index);
        }

        public override void RemoveConveyorGroup(ConveyorGroup conveyorGroup)
        {
            base.RemoveConveyorGroup(conveyorGroup);

            if (_currentBuildingRef is FactoryBuilding f)
                f.SetTargetIndex(0);
        }
        
        #endregion
        
        #region fields
        
        [SerializeField] private GameObject preSelectionAvailable;

        private bool _isSelected;
        
        private Building _currentBuildingRef;
        
        #endregion
    }
}
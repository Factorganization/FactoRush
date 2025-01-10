using System.Collections.Generic;
using GameContent.CraftResources;
using GameContent.Entities.OnFieldEntities;
using UnityEngine;

namespace GameContent.Entities.GridEntities
{
    public sealed class DynamicBuildingTile : Tile
    {
        #region properties

        public override bool IsBlocked => CurrentBuildingRef is not null;

        public override bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                preSelectionAvailable.SetActive(value);
            }
        }

        public override List<ConveyorGroup> GroupRef //sa race
        {
            get => null;
            protected set { }
        }

        #endregion

        #region methodes

        public override void AddConveyorGroup(ConveyorGroup conveyorGroup) {}

        public override void RemoveConveyorGroup(ConveyorGroup conveyorGroup) {}

        protected override void InstantiateResourceAt(int conveyoorIndex, BaseResource resource, Vector3 pos, int pathIndex) {}

        protected override void DestroyResource(int conveyorIndex, BaseResource resource) {}

        #endregion

        #region fields

        [SerializeField] private GameObject preSelectionAvailable;

        [SerializeField] private GameObject preSelectionUnavailable; 

        private bool _isSelected;

        #endregion
    }
}
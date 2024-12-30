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

        protected override ConveyorGroup GroupRef
        {
            get => null;
            set { }
        }

        #endregion
        
        #region methodes
        
        public override void SetConveyorGroup(ConveyorGroup conveyorGroup) {}

        protected override void InstantiateResourceAt(BaseResource resource, Vector3 pos, int pathIndex) {}
        
        protected override void DestroyResource(BaseResource resource) {}

        #endregion

        #region fields

        [SerializeField] private GameObject preSelectionAvailable;
        
        [SerializeField] private GameObject preSelectionUnavailable; 
        
        private bool _isSelected;

        #endregion
    }
}
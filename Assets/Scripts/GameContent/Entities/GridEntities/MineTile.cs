using GameContent.CraftResources;
using GameContent.Entities.OnFieldEntities;
using UnityEngine;

namespace GameContent.Entities.GridEntities
{
    public class MineTile : Tile
    {
        #region properties

        private ConveyorGroup CurrentGroup { get; set; }

        #endregion
        
        #region methodes

        protected override void OnUpdate()
        {
            base.OnUpdate();
            
            if (CurrentBuildingRef is not null && CurrentGroup is not null)
            {
                
            }
        }

        private void SetConveyorGroup(ConveyorGroup conveyorGroup) => CurrentGroup = conveyorGroup;

        private void ClearGroup() => CurrentGroup = null;

        #endregion
        
        #region fields

        [SerializeField] private MiningResource miningResource;

        #endregion
    }
}
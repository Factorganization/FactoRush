using GameContent.CraftResources;
using GameContent.Entities.OnFieldEntities;
using UnityEngine;

namespace GameContent.Entities.GridEntities
{
    public sealed class MineTile : Tile
    {
        #region properties

        public override bool IsBlocked => CurrentBuildingRef is not null;

        public override bool IsSelected
        {
            get;
            set;
        }

        #endregion
        
        #region methodes

        protected override void OnUpdate()
        {
            base.OnUpdate();
            
            if (CurrentBuildingRef is not null && GroupRef is not null && Active)
            {
                
            }
        }

        #endregion
        
        #region fields

        [SerializeField] private MiningResource miningResource;

        #endregion
    }
}
using GameContent.CraftResources;
using UnityEngine;

namespace GameContent.Entities.OnFieldEntities.Buildings
{
    public class MineBuilding : StaticBuilding
    {
        #region methodes

        protected override void OnUpdate()
        {
            base.OnUpdate();
        }

        #endregion
        
        #region fields
        
        [SerializeField] private MiningResource baseResource;
        
        #endregion
    }
}
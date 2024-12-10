using System.Collections.Generic;
using GameContent.CraftResources;
using UnityEngine;

namespace GameContent.Entities.OnFieldEntities.Buildings
{
    public class MineBuilding : StaticBuilding
    {
        #region properties

        public List<FactoryBuilding> FactoryBuildingsRef { get; set; }

        #endregion
        
        #region methodes

        protected override void OnStart()
        {
            base.OnStart();
            
            FactoryBuildingsRef = new List<FactoryBuilding>();
        }

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
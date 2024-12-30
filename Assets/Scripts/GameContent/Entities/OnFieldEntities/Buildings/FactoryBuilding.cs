using System;
using System.Collections.Generic;
using GameContent.CraftResources;

namespace GameContent.Entities.OnFieldEntities.Buildings
{
    public sealed class FactoryBuilding : StaticBuilding
    {
        #region properties
        
        public List<AssemblyBuilding> AssemblyBuildingRef { get; set; }

        #endregion
        
        #region methodes

        protected override void OnStart()
        {
            base.OnStart();
            
            AssemblyBuildingRef = new List<AssemblyBuilding>();

            _miningResources = new Dictionary<MiningResourceType, int>();
            for (var i = 0; i < Enum.GetValues(typeof(MiningResourceType)).Length; i++)
            {
                _miningResources.Add((MiningResourceType)i, 0);
            }
        }

        private void CheckMiningResources()
        {
            
        }
        
        public void ResourceAdded(MiningResourceType type)
        {
            _miningResources[type]++;
            CheckMiningResources();
        }

        public void ResourceRemoved(MiningResourceType type)
        {
            _miningResources[type]--;
        }
        
        #endregion
        
        #region fields

        public FactoryData data;
        
        private Dictionary<MiningResourceType, int> _miningResources;
        
        #endregion
    }
}
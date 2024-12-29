using System;
using System.Collections.Generic;
using GameContent.CraftResources;
using UnityEngine;

namespace GameContent.Entities.OnFieldEntities.Buildings
{
    public class FactoryBuilding : StaticBuilding
    {
        #region properties

        public List<MineBuilding> MineBuildingRef { get; set; }
        
        public List<AssemblyBuilding> AssemblyBuildingRef { get; set; }

        #endregion
        
        #region methodes

        protected override void OnStart()
        {
            base.OnStart();
            
            MineBuildingRef = new List<MineBuilding>();
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
            Debug.Log(_miningResources[type]);
            CheckMiningResources();
        }

        public void ResourceRemoved(MiningResourceType type)
        {
            _miningResources[type]--;
        }
        
        #endregion
        
        #region fields

        private Dictionary<MiningResourceType, int> _miningResources;

        #endregion
    }
}
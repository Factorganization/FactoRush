using System;
using System.Collections.Generic;
using GameContent.CraftResources;
using Unity.Burst.CompilerServices;
using UnityEngine;

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

        protected override void OnUpdate()
        {
            base.OnUpdate();
            
            //Active

            if (_spawnCounter <= Constants.SpawnInterval)
            {
                _spawnCounter += Time.deltaTime;
                return;
            }
            
            if (TileRef.GroupRef.Count <= 0)
                return;
            
            //TODO 
            //des formules ma couille
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

        private float _spawnCounter;

        private float _targetIndex;

        #endregion
    }
}
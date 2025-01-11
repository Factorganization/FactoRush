using System;
using System.Collections.Generic;
using GameContent.CraftResources;
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

            if (_miningResources[MiningResourceType.Iron] < data.recipe.iron ||
                _miningResources[MiningResourceType.Copper] < data.recipe.copper ||
                _miningResources[MiningResourceType.Gold] < data.recipe.gold)
                return;

            InstantiateResourceAt(_targetIndex, Position + Vector3.up * 0.25f);
            ResourceRemoved(MiningResourceType.Iron, data.recipe.iron);
            ResourceRemoved(MiningResourceType.Copper, data.recipe.copper);
            ResourceRemoved(MiningResourceType.Gold, data.recipe.gold);
            _spawnCounter = 0;
            _targetIndex = (_targetIndex + 1) % TileRef.GroupRef.Count;
        }
        
        public void ResourceAdded(MiningResourceType type)
        {
            _miningResources[type]++;
        }

        private void ResourceRemoved(MiningResourceType type, int amount)
        {
            _miningResources[type] -= amount;
        }

        private void InstantiateResourceAt(int conveyorIndex, Vector3 pos)
        {
            var r = Instantiate(unitComponent, pos, Quaternion.identity);
            r.Created(TileRef.GroupRef[conveyorIndex]);
            r.SetUnitComponent(data.component);
            TileRef.GroupRef[conveyorIndex].AddResource(r);
        }
        
        #endregion
        
        #region fields

        [SerializeField] private FactoryData data;

        [SerializeField] private RefinedResource unitComponent; // j'ai chié sur les noms là ...
        
        private Dictionary<MiningResourceType, int> _miningResources;

        private float _spawnCounter;

        private int _targetIndex;

        #endregion
    }
}
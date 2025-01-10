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

        protected override void OnStart()
        {
            base.OnStart();
            _spawnCounter = 0;
            _targetIndex = 0;
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            
            if (!Active)
                return;

            if (_spawnCounter <= Constants.SpawnInterval)
            {
                _spawnCounter += Time.deltaTime;
                return;
            }
            
            if (CurrentBuildingRef is null || GroupRef is null)
                return;

            foreach (var c in GroupRef)
            {
                if (c is null)
                    return;
            }
            InstantiateResourceAt(_targetIndex, miningResource, Position + Vector3.up * 0.25f, _targetIndex);
            _spawnCounter = 0;
            _targetIndex = (_targetIndex + 1) % GroupRef.Count;
        }

        public override void RemoveConveyorGroup(ConveyorGroup conveyorGroup)
        {
            _targetIndex = 0;
            base.RemoveConveyorGroup(conveyorGroup);
        }
        
        public override void MarkActive(bool active)
        {
            base.MarkActive(active);

            _spawnCounter = 0;
        }

        #endregion
        
        #region fields

        [SerializeField] private MiningResource miningResource;

        private float _spawnCounter;

        private int _targetIndex;

        #endregion
    }
}
using GameContent.CraftResources;
using GameContent.Entities.OnFieldEntities;
using GameContent.GridManagement;
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

        public override void Added(GridManager grid, Vector2Int index, Vector3 pos = new Vector3(), TileType type = TileType.Default)
        {
            base.Added(grid, index, pos, type);

            ETransform.rotation = (index.x, index.y) switch
            {
                (0, 0) => Quaternion.Euler(0, 90, 0),
                (0, > 0) => Quaternion.Euler(0, 180, 0),
                (> 0, 0) => Quaternion.Euler(0, 0, 0),
                (> 0, > 0) => Quaternion.Euler(0, -90, 0),
                _ => ETransform.rotation
            };
        }

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
            InstantiateResourceAt(_targetIndex, _miningResource, Position + Vector3.up * 0.25f, _targetIndex);
            _spawnCounter = 0;
            _targetIndex = (_targetIndex + 1) % GroupRef.Count;
        }
        
        public void SetResource(MiningResource resource) => _miningResource = resource;
        
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

        private MiningResource _miningResource;

        private float _spawnCounter;

        private int _targetIndex;

        #endregion
    }
}
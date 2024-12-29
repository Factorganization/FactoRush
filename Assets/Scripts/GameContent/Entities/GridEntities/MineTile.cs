using GameContent.CraftResources;
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

            InstantiateResourceAt(miningResource, Position + Vector3.up * 0.25f);
            _spawnCounter = 0;
        }

        #endregion
        
        #region fields

        [SerializeField] private MiningResource miningResource;

        private float _spawnCounter;

        #endregion
    }
}
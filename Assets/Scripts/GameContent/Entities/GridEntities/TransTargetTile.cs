using GameContent.GridManagement;
using UnityEngine;

namespace GameContent.Entities.GridEntities
{
    /// <summary>
    /// used to type comparison
    /// manage the update of the group (arbitrary choice)
    /// </summary>
    public sealed class TransTargetTile : UnitAssemblyTile
    {
        #region methodes

        protected override void OnUpdate()
        {
            base.OnUpdate();
            
            if (_spawnCounter <= Constants.SpawnInterval)
            {
                _spawnCounter += Time.deltaTime;
                return;
            }
            
            if (RefinedResources.Count > 0 && GridManager.Manager.AssemblyTileGroups[AssemblyId].WeaponTile.RefinedResources.Count > 0)
            {
                //UnitsManager.Instance.SpawnUnit();
            }
        }

        #endregion
        
        #region fields

        private float _spawnCounter;

        #endregion
    }
}
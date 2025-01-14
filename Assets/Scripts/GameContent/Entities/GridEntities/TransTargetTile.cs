using GameContent.Entities.UnmanagedEntities;
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
            if (_spawnCounter <= Constants.UnitSpawnInterval)
            {
                _spawnCounter += Time.deltaTime;
                return;
            }
            
            if (RefinedResources.Count <= 0 && _localWeaponTileRef.RefinedResources.Count <= 0)
                return;

            if (RefinedResources.Count >= 2 && _localWeaponTileRef.RefinedResources.Count <= 0)
            {
                if (RefinedResources.Peek() is TransportComponent)
                    UnitsManager.Instance.SpawnUnit(true, RemoveResource() as TransportComponent);
                else if (RefinedResources.Peek() is WeaponComponent)
                    UnitsManager.Instance.SpawnUnit(true, null, RemoveResource() as WeaponComponent);
                return;
            }

            if (_localWeaponTileRef.RefinedResources.Count >= 2 && RefinedResources.Count <= 0)
            {
                if (_localWeaponTileRef.RefinedResources.Peek() is TransportComponent)
                    UnitsManager.Instance.SpawnUnit(true, _localWeaponTileRef.RemoveResource() as TransportComponent);
                else if (_localWeaponTileRef.RefinedResources.Peek() is WeaponComponent)
                    UnitsManager.Instance.SpawnUnit(true, null, _localWeaponTileRef.RemoveResource() as WeaponComponent);
                return;
            }

            if (RefinedResources.Count >= 1 && _localWeaponTileRef.RefinedResources.Count >= 1)
            {
                if (RefinedResources.Peek() is TransportComponent)
                    UnitsManager.Instance.SpawnUnit(true, RemoveResource() as TransportComponent, _localWeaponTileRef.RemoveResource() as WeaponComponent);
                else if (RefinedResources.Peek() is WeaponComponent)
                    UnitsManager.Instance.SpawnUnit(true, _localWeaponTileRef.RemoveResource() as TransportComponent, RemoveResource() as WeaponComponent);
            }
            
            _spawnCounter = 0;
        }

        public void SetWeaponRef(WeaponTargetTile w) => _localWeaponTileRef = w;

        public override void SetBinTileRef(UnitAssemblyTile unitAssemblyTile)
        {
            BinTileGroupRef = unitAssemblyTile;
            BinTileGroupRef.SetBinTileRef(this);
        }

        #endregion
        
        #region fields

        private float _spawnCounter;

        private WeaponTargetTile _localWeaponTileRef;

        #endregion
    }
}
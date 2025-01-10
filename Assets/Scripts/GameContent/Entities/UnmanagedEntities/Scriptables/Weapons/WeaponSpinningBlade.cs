using System.Collections.Generic;
using UnityEngine;

namespace GameContent.Entities.UnmanagedEntities.Scriptables.Weapons
{
    [CreateAssetMenu(fileName = "WeaponSpinningBlade", menuName = "Components/WeaponComponents/WeaponSpinningBlade")]
    public sealed class WeaponSpinningBlade : WeaponComponent
    {
        
        
        #region Unique Effects Handlers
        protected override void HandleUniqueEffect(Unit attacker, Unit target, List<Unit> allUnitsInRange)
        {
            // Damage all units in range
            foreach (var unit in allUnitsInRange)
            {
                unit.ApplyDamage(Damage);
            }
        }
        
        #endregion
        
    }
    
}
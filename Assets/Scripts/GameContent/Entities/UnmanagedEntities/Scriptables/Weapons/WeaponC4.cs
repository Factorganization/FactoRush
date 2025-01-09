using System.Collections.Generic;
using UnityEngine;

namespace GameContent.Entities.UnmanagedEntities.Scriptables.Weapons
{
    [CreateAssetMenu(fileName = "WeaponC4", menuName = "Components/WeaponComponents/WeaponC4")]
    public sealed class WeaponC4 : WeaponComponent
    {
        
        #region Unique Effects Handlers
        protected override void HandleUniqueEffect(Unit attacker, Unit target, List<Unit> allUnitsInRange)
        {
            // Apply damage to all units in range and lose 33% of max health
            foreach (var unit in allUnitsInRange)
            {
                unit.ApplyDamage(Damage);
            }
            attacker.ApplyDamage(attacker.currentHealth / 3);
        }
        
        #endregion
        
    }
    
}
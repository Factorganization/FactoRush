using System.Collections.Generic;
using UnityEngine;

namespace GameContent.Entities.UnmanagedEntities.Scriptables.Weapons
{
    [CreateAssetMenu(fileName = "WeaponSymbioticRifle", menuName = "Components/WeaponComponents/WeaponSymbioticRifle")]
    public sealed class WeaponSymbioticRifle : WeaponComponent
    {
        
        #region Unique Effects Handlers
        protected override void HandleUniqueEffect(Unit attacker, Unit target, List<Unit> allUnitsInRange)
        {
            // Target the lowest health unit in range 
            Unit lowestHealthUnit = null;
            float lowestHealth = float.MaxValue;
            foreach (var unit in allUnitsInRange)
            {
                if (unit.currentHealth < lowestHealth)
                {
                    lowestHealth = unit.currentHealth;
                    lowestHealthUnit = unit;
                }
            }
            if (lowestHealthUnit != null)
            {
                lowestHealthUnit.ApplyDamage(Damage);
            }
        }
        
        #endregion
        
    }
    
}
using System.Collections.Generic;
using UnityEngine;

namespace GameContent.Entities.UnmanagedEntities.Scriptables.Weapons
{
    [CreateAssetMenu(fileName = "WeaponSpear", menuName = "Components/WeaponComponents/WeaponSpear")]
    public sealed class WeaponSpear : WeaponComponent
    {
        
        [Header("Effect Parameters")]
        public float SpeedDamageMultiplier = 2f; // Damage multiplier based on velocity
        
        #region Unique Effects Handlers
        protected override void HandleUniqueEffect(Unit attacker, Unit target, List<Unit> allUnitsInRange)
        {
            // Gain a damage multiplier if speed velocity isnt zero
            float damageMultiplier = attacker.velocity.magnitude > 0 ? SpeedDamageMultiplier : 1f;
            
            Unit priorityTarget = null;
            foreach (var unit in allUnitsInRange)
            {
                if (unit.weaponComponent != null && unit.weaponComponent.isAPriorityTarget)
                {
                    priorityTarget = unit;
                    break;
                }
            }
            
            if (priorityTarget is null) // if no priority target is found, target the last target
            {
                foreach (var unit in allUnitsInRange)
                {
                    if (unit == attacker.lastTarget)
                    {
                        priorityTarget = unit;
                        break;
                    }
                }
            }
            
            priorityTarget ??= target; // if no priority target is found, target the default target
            
            priorityTarget.ApplyDamage(attacker.damage);
            
        }
        
        #endregion
        
    }
    
}
using System.Collections.Generic;
using UnityEngine;

namespace GameContent.Entities.UnmanagedEntities.Scriptables.Weapons
{
    [CreateAssetMenu(fileName = "WeaponMinigun", menuName = "Components/WeaponComponents/WeaponMinigun")]
    public sealed class WeaponMinigun : WeaponComponent
    {
        
        [Header("Effect Parameters")]
        public float MinigunSpeedMultiplier = 0.9f; // % speed multiplier per attack
        public float MinigunMinimumAttackSpeed = 0.5f; // Minimum attack speed
        
        #region Unique Effects Handlers
        protected override void HandleUniqueEffect(Unit attacker, Unit target, List<Unit> allUnitsInRange)
        {
            // Gain 10% attack speed per attack, up to 50%. Resets on change position
            
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
            
            attacker.attackSpeed = Mathf.Max(MinigunMinimumAttackSpeed, attacker.attackSpeed * MinigunSpeedMultiplier); // Increase attack speed
            
        }
        
        #endregion
        
    }
    
}
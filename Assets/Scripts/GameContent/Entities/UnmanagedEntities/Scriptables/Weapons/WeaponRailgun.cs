using System.Collections.Generic;
using UnityEngine;

namespace GameContent.Entities.UnmanagedEntities.Scriptables.Weapons
{
    [CreateAssetMenu(fileName = "WeaponRailgun", menuName = "Components/WeaponComponents/WeaponRailgun")]
    public sealed class WeaponRailgun : WeaponComponent
    {
        
        [Header("Unique Effect Parameters")]
        public float EffectRange = 5f; // Range of the effect
        
        
        #region Unique Effects Handlers
        protected override void HandleUniqueEffect(Unit attacker, Unit target, List<Unit> allUnitsInRange)
        {
            Unit priorityTarget = null;
            
            // Priorité aux cibles prioritaires
            foreach (var unit in allUnitsInRange)
            {
                if (unit.weaponComponent != null && unit.weaponComponent.isAPriorityTarget)
                {
                    priorityTarget = unit;
                    break; // Trouve la première cible prioritaire dans la portée
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
            

            // Appliquer les dégâts à la cible préférée et aux autres unités du même type dans la portée
            
            foreach (var unit in allUnitsInRange)
            {
                if (unit.isAirUnit == priorityTarget.isAirUnit)
                {
                    unit.ApplyDamage(attacker.damage);
                }
            }
            
        }
        
        #endregion
        
    }
    
}
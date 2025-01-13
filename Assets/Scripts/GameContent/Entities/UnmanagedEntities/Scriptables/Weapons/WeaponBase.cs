using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GameContent.Entities.UnmanagedEntities.Scriptables.Weapons
{
    [CreateAssetMenu(fileName = "WeaponBase", menuName = "Components/WeaponComponents/WeaponBase")]
    public sealed class WeaponBase : WeaponComponent
    {
        protected override void HandleUniqueEffect(Unit attacker, Unit target, List<Unit> allUnitsInRange)
        {
            // check for a priority target, otherwise target the default target
            Unit priorityTarget = null;
            foreach (var unit in allUnitsInRange)
            {
                if (unit.weaponComponent && unit.weaponComponent.isAPriorityTarget)
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
    }
    
}
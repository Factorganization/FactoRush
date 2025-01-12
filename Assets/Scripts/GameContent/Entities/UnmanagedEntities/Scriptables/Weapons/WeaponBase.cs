using System.Collections.Generic;
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
                if (unit.weaponComponent != null && unit.weaponComponent.isAPriorityTarget)
                {
                    priorityTarget = unit;
                    break;
                }
            }
            if (priorityTarget != null)
            {
                priorityTarget.ApplyDamage(attacker.damage);
            }
            else
            {
                target.ApplyDamage(attacker.damage);
            }
        }
    }
    
}
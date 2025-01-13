using System.Collections.Generic;
using UnityEngine;

namespace GameContent.Entities.UnmanagedEntities.Scriptables.Weapons
{
    [CreateAssetMenu(fileName = "WeaponCanon", menuName = "Components/WeaponComponents/WeaponCanon")]
    public sealed class WeaponCanon : WeaponComponent
    {
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

            if (priorityTarget is null)
            {
                // Priorité aux cibles au sol
                foreach (var unit in allUnitsInRange)
                {
                    if (!unit.isAirUnit && Vector3.Distance(target.transform.position, unit.transform.position) <= Range)
                    {
                        priorityTarget = unit;
                        break; // Trouve la première cible au sol dans la portée
                    }
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


            // Appliquer les dégâts à la cible préférée
            float damageMultiplier = !priorityTarget.isAirUnit ? 2f : 1f;
            priorityTarget.ApplyDamage(attacker.damage * damageMultiplier);
            
        }
        
        #endregion
        
    }
    
}
using System.Collections.Generic;
using UnityEngine;

namespace GameContent.Entities.UnmanagedEntities.Scriptables.Weapons
{
    [CreateAssetMenu(fileName = "WeaponWarhammer", menuName = "Components/WeaponComponents/WeaponWarhammer")]
    public sealed class WeaponWarhammer : WeaponComponent
    {
        
        [Header("Effect Parameters")]
        public float StunDuration;
        
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
            
            

            // Si aucune cible prioritaire n'est trouvée, default à la target
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

            // Appliquer les dégâts à la cible préférée et aux stun les autres unités du même type dans la portée
            
            priorityTarget.ApplyDamage(attacker.damage);
            foreach (var unit in allUnitsInRange)
            {
                if (unit.isAirUnit == priorityTarget.isAirUnit)
                {
                    //if (unit != priorityTarget) divide stun duration by 2
                    unit.Stun(unit == priorityTarget ? StunDuration : StunDuration / 2);
                }
            }
            
        }
        
        #endregion
        
    }
    
}
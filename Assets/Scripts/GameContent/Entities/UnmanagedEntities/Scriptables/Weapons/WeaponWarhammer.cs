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
            if (priorityTarget == null)
            {
                priorityTarget = target;
            }

            // Appliquer les dégâts à la cible préférée et aux stun les autres unités du même type dans la portée
            if (priorityTarget != null)
            {
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
        }
        
        #endregion
        
    }
    
}
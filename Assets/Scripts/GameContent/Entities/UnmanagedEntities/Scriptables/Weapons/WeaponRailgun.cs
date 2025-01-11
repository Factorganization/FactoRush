using System.Collections.Generic;
using UnityEngine;

namespace GameContent.Entities.UnmanagedEntities.Scriptables.Weapons
{
    [CreateAssetMenu(fileName = "WeaponRailgun", menuName = "Components/WeaponComponents/WeaponRailgun")]
    public sealed class WeaponRailgun : WeaponComponent
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

            // Si aucune cible prioritaire n'est trouvée, chercher une cible 
            if (priorityTarget == null)
            {
                foreach (var unit in allUnitsInRange)
                {
                    if (unit.isAirUnit && Vector3.Distance(target.transform.position, unit.transform.position) <= Range)
                    {
                        priorityTarget = unit;
                        break; // Trouve la première cible dans la portée
                    }
                }
            }

            // Appliquer les dégâts à la cible préférée et aux autres unités du même type dans la portée
            if (priorityTarget != null)
            {
                foreach (var unit in allUnitsInRange)
                {
                    if (unit.GetType() == priorityTarget.GetType())
                    {
                        unit.ApplyDamage(attacker.damage);
                    }
                }
            }
        }
        
        #endregion
        
    }
    
}
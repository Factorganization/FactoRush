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

            // Si aucune cible prioritaire n'est trouvée, chercher une cible 
            if (priorityTarget == null)
            {
               priorityTarget = target;
            }

            // Appliquer les dégâts à la cible préférée et aux autres unités du même type dans la portée
            if (priorityTarget != null)
            {
                foreach (var unit in allUnitsInRange)
                {
                    Debug.Log("unit.isAirUnit: " + unit.isAirUnit + " priorityTarget.isAirUnit: " + priorityTarget.isAirUnit);
                    if (unit.isAirUnit == priorityTarget.isAirUnit)
                    {
                        unit.ApplyDamage(attacker.damage);
                    }
                }
            }
        }
        
        #endregion
        
    }
    
}
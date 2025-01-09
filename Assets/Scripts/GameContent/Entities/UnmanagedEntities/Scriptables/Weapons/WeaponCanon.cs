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
            Unit preferredTarget = null;

            // Priorité aux cibles au sol
            foreach (var unit in allUnitsInRange)
            {
                if (!unit.isAirUnit && Vector3.Distance(target.transform.position, unit.transform.position) <= Range)
                {
                    preferredTarget = unit;
                    break; // Trouve la première cible au sol dans la portée
                }
            }

            // Si aucune cible au sol n'est trouvée, chercher une cible aérienne
            if (preferredTarget == null)
            {
                foreach (var unit in allUnitsInRange)
                {
                    if (unit.isAirUnit && Vector3.Distance(target.transform.position, unit.transform.position) <= Range)
                    {
                        preferredTarget = unit;
                        break; // Trouve la première cible aérienne dans la portée
                    }
                }
            }

            // Appliquer les dégâts à la cible préférée
            if (preferredTarget != null)
            {
                float damageMultiplier = !preferredTarget.isAirUnit ? 2f : 1f;
                preferredTarget.ApplyDamage(Damage * damageMultiplier);
            }
        }
        
        #endregion
        
    }
    
}
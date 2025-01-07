using System.Collections.Generic;
using UnityEngine;

namespace GameContent.Entities.UnmanagedEntities
{
    [CreateAssetMenu(fileName = "WeaponCanon", menuName = "Components/WeaponComponents/WeaponCanon")]
    public sealed class WeaponCanon : WeaponComponent
    {
        public override void Attack(Unit attacker, Unit target, List<Unit> allUnitsInRange)
        {
            if (!CanAttack) return;
            
            HandleCannonEffect(target, allUnitsInRange);

            attackCooldown = AttackSpeed; // Reset cooldown
        }
        
        #region Unique Effects Handlers
        private void HandleCannonEffect(Unit target, List<Unit> allUnitsInRange)
        {
            Unit preferredTarget = null;

            // Priorité aux cibles au sol
            foreach (var unit in allUnitsInRange)
            {
                if (unit.isGroundUnit && Vector3.Distance(target.transform.position, unit.transform.position) <= Range)
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
                float damageMultiplier = preferredTarget.isGroundUnit ? 2f : 1f;
                preferredTarget.ApplyDamage(Damage * damageMultiplier);
            }
        }
        
        #endregion
        
        
        protected override void HandleUniqueEffectExemple(Unit _)
        {
            
        }
    }
    
}
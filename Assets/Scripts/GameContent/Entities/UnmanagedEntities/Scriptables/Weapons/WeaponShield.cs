using System.Collections.Generic;
using UnityEngine;

namespace GameContent.Entities.UnmanagedEntities.Scriptables.Weapons
{
    [CreateAssetMenu(fileName = "WeaponShield", menuName = "Components/WeaponComponents/WeaponShield")]
    public sealed class WeaponShield : WeaponComponent
    {
        [Header("Effect Parameters")]
        public float ShieldAmount = 100f; // Shield amount
        
        # region Start
        
        public override void Initialize(Unit unit)
        {
            isAPriorityTarget = true;
            unit.AddShield(100f);
        }
        
        #endregion
        
        #region Unique Effects Handlers
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
        
        #endregion
        
    }
    
}
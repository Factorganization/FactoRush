using System.Collections.Generic;
using UnityEngine;

namespace GameContent.Entities.UnmanagedEntities.Scriptables.Weapons
{
    [CreateAssetMenu(fileName = "WeaponArtillery", menuName = "Components/WeaponComponents/WeaponArtillery")]
    public sealed class WeaponArtillery : WeaponComponent
    {
        #region Unique Effects Handlers
        protected override void HandleUniqueEffect(Unit attacker, Unit target, List<Unit> allUnitsInRange)
        {
            // Target Air in priority, Deals damage to all air units in range if there is any, else deals damage to one ground unit in range
            bool targetFound = false;
            foreach (var unit in allUnitsInRange)
            {
                if (unit.isAirUnit && Vector3.Distance(target.transform.position, unit.transform.position) <= Range)
                {
                    unit.ApplyDamage(attacker.damage);
                    targetFound = true;
                }
            }
            if (!targetFound)
            {
                foreach (var unit in allUnitsInRange)
                {
                    if (!unit.isAirUnit && Vector3.Distance(target.transform.position, unit.transform.position) <= Range)
                    {
                        unit.ApplyDamage(attacker.damage);
                        break;
                    }
                }
            }
            
        }
        
        #endregion
        
    }
    
}
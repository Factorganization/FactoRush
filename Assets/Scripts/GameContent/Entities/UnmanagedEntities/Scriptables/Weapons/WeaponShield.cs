using System.Collections.Generic;
using UnityEngine;

namespace GameContent.Entities.UnmanagedEntities.Scriptables.Weapons
{
    [CreateAssetMenu(fileName = "WeaponShield", menuName = "Components/WeaponComponents/WeaponShield")]
    public sealed class WeaponShield : WeaponComponent
    {
        
        #region Unique Effects Handlers
        protected override void HandleUniqueEffect(Unit attacker, Unit target, List<Unit> allUnitsInRange)
        {
            // 
        }
        
        #endregion
        
    }
    
}
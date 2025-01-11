using System.Collections.Generic;
using UnityEngine;

namespace GameContent.Entities.UnmanagedEntities.Scriptables.Weapons
{
    [CreateAssetMenu(fileName = "WeaponBase", menuName = "Components/WeaponComponents/WeaponBase")]
    public sealed class WeaponBase : WeaponComponent
    {
        protected override void HandleUniqueEffect(Unit unit, Unit target = null, List<Unit> allUnitsInRange = null)
        {
            // Do nothing
        }
    }
    
}
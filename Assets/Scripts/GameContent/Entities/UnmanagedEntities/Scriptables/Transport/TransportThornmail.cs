using System.Collections.Generic;
using UnityEngine;

namespace GameContent.Entities.UnmanagedEntities
{
    [CreateAssetMenu(fileName = "TransportThornmail", menuName = "Components/TransportsComponent/TransportThornmail")]
    public class TransportThornmail : TransportComponent
    {
        
        [Header("Effect Parameters")]
        public float range = 5f; // Range of the effect
        public float damagePerLightning = 10f; // Damage to deal
        
        public override void UniqueBehavior(Unit unit, Unit target = null, List<Unit> allUnitsInRange = null)
        {
            if (target != null) target.ApplyDamageRaw(damagePerLightning);
        }
    }
}
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameContent.Entities.UnmanagedEntities
{
    [CreateAssetMenu(fileName = "TransportSlider", menuName = "Components/TransportsComponent/TransportSlider")]
    public class TransportSlider : TransportComponent
    {
        
        [Header("Effect Parameters")]
        public float dashRange = 6f; // Dash range
        public override void UniqueBehavior(Unit unit, Unit target = null, List<Unit> allUnitsInRange = null)
        {
            if (target != null)
            {
                if (unit.Range <= 1f)
                {
                    unit.Dash(unit.ETransform.forward, dashRange - 1);
                }
                
            }
            if (unit.Range > 1)
            {
                unit.Dash( - unit.ETransform.forward, dashRange - 3);
            }
            
        }
    }
}
using UnityEngine;

namespace GameContent.Entities.UnmanagedEntities.Scriptables.Transport
{
    [CreateAssetMenu(fileName = "TransportSlider", menuName = "Components/TransportsComponent/TransportSlider")]
    public class TransportSlider : TransportComponent
    {
        [Header("Effect Parameters")]
        public float dashRange = 6f; // Dash range
        
        public override void UniqueBehavior(Unit unit, Unit target = null)
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
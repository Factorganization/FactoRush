using UnityEngine;

namespace GameContent.Entities.UnmanagedEntities.Scriptables.Transport
{
    [CreateAssetMenu(fileName = "TransportAccumulator", menuName = "Components/TransportsComponent/TransportAccumulator")]
    public class TransportAccumulator : TransportComponent
    {
        [Header("Effect Parameters")]
        public float speedPerStack = 10f; // Damage to deal
        
        public override void UniqueBehavior(Unit unit, Unit target = null)
        {
            // Increase the speed of the unit by the amount of stacks
            unit.moveSpeed += speedPerStack;
        }
    }
}